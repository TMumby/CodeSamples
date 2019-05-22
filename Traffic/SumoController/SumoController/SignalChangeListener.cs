using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace SumoController
{
    /// <summary>
    /// class for listening for signal change requests from Junction agents
    /// </summary>
    public class SignalChangeListener
    {
        private static readonly HttpListener _listener = new HttpListener();

        private static List<TrafficLight> _trafficLights;

        /// <summary>
        /// Initialises listener class and starts listener
        /// </summary>
        /// <param name="trafficLights">list of traffic lights</param>
        public SignalChangeListener(List<TrafficLight> trafficLights)
        {
            try
            {
                _trafficLights = trafficLights;
                StartListener();
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error initialising traffic light listener", ex);
            }
        }

        /// <summary>
        /// Start the listenr
        /// </summary>
        private static void StartListener()
        {
            try
            {
                Console.WriteLine("Listening for traffic light change requests");
                SetListenerURL();
                _listener.Start();
                Listen();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error starting traffic light listener", ex);
            }

        }

        /// <summary>
        /// retrieve the url from config and set listener url
        /// </summary>
        private static void SetListenerURL()
        {
            try
            {
                string uRL = ConfigReader.GetConnectionString("LightListener");
                _listener.Prefixes.Add(uRL);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error getting Light listener Url", ex);
            }
        }

        /// <summary>
        /// Indefinately listen for a http post
        /// </summary>
        private static async void Listen()
        {
            while (true)
            {                
                HttpListenerContext context = await _listener.GetContextAsync();                
                Task.Factory.StartNew(() => ProcessRequest(context));
            }
        }


        /// <summary>
        /// Process the light change request, then update light and send response
        /// </summary>
        /// <param name="context">the recieved post</param>
        private static void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                string body = new StreamReader(context.Request.InputStream).ReadToEnd().Trim();
                Tuple<int, string> signalData = ProcessSoapMessage(body);
                Console.WriteLine("Recieved Signal Change Request, change light " 
                                   + signalData.Item1.ToString() + " to phase " + signalData.Item2);
                UpdateLight(signalData);
                SendResponse(context);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Processing received post", ex);
            }
        }

        /// <summary>
        /// Retrieves required data from SOAP message
        /// </summary>
        /// <param name="body">change light post as string</param>
        /// <returns>the light to change and the phase to change it too</returns>
        private static Tuple<int, string>ProcessSoapMessage(string body)
        {
            try
            {
                XElement soapBody = GetMessageBody(body);
                int junctionNo = GetJunctionlNo(soapBody);
                string signalState = GetSignalState(soapBody);
                return new Tuple<int, string>(junctionNo, signalState);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Processing Soap Message", ex);
            }           
        }

        /// <summary>
        /// Retrieves the messsage body from SOAP message
        /// </summary>
        /// <param name="body">change light post as string</param>
        /// <returns></returns>
        private static XElement GetMessageBody(string body)
        {
            try
            {
                XDocument request = XDocument.Parse(body);
                XElement envelope = request.Elements().Where(item => item.Name.LocalName == "Envelope").First();
                return envelope.Elements().Where(item => item.Name.LocalName == "Body").First();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Getting Message Body", ex);
            }

        }

        /// <summary>
        /// Retrieves the signal no from xml
        /// </summary>
        /// <param name="soapBody">XML Soap body</param>
        /// <returns>the signal number</returns>
        private static int GetJunctionlNo(XElement soapBody)
        {
            try
            {
                XElement junctionNo = soapBody.Elements().Where(item => item.Name.LocalName == "JunctionNo").First();
                return Int32.Parse(junctionNo.Value);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error retrieving Signal No from message Body", ex);
            }
        }


        /// <summary>
        /// Retrieve the signal state to change to from SOAP body
        /// </summary>
        /// <param name="soapBody">XML Soap body</param>
        /// <returns>The new signal state</returns>
        private static string GetSignalState(XElement soapBody)
        {  
            try
            {
                XElement signalState = soapBody.Elements().Where(item => item.Name.LocalName == "SignalState").First();
                return signalState.Value;
            }   
            catch (Exception ex)
            {
                throw new SumoControllerException("Error retrieving Signal State from message Body", ex);
            }    
        }

        /// <summary>
        /// Send message to tell Sumo to updat phase of a light
        /// </summary>
        /// <param name="signalData">Tuple consiting of light to change and what to change it to</param>
        private static void UpdateLight(Tuple<int, string> signalData)
        {
            try
            {
                _trafficLights[signalData.Item1].SetPhase(signalData.Item2);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Updating Light", ex);
            }            
        }

        /// <summary>
        /// Sends response to Juncion to acknowledge post
        /// </summary>
        /// <param name="context">http listener context</param>
        private static void SendResponse(HttpListenerContext context)
        {
            try
            {
                byte[] message = CreateAcknowledgementMessage();
                context = CreateResponse(context, message);
                WriteResponse(context, message);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error sending response", ex);
            }
        }

        /// <summary>
        /// Creates the acknowledgement message
        /// </summary>
        /// <returns>acknowledgement message in bytes</returns>
        private static byte[] CreateAcknowledgementMessage()
        {
            try
            {
                return Encoding.UTF8.GetBytes("ACK");
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error creating response message", ex);
            }
            
        }

        /// <summary>
        /// Creates Response message to http post
        /// </summary>
        /// <param name="context">http listener context</param>
        /// <param name="message">acknowledgement message</param>
        /// <returns>updated context</returns>
        private static HttpListenerContext CreateResponse(HttpListenerContext context, byte[] message)
        {
            try
            {
                context.Response.StatusCode = 200;
                context.Response.KeepAlive = false;
                context.Response.ContentLength64 = message.Length;
                return context;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error creating Listenser response", ex);
            }
        }
        /// <summary>
        /// Sends the http response(writes to stream)
        /// </summary>
        /// <param name="context">http listener context</param>
        /// <param name="message">message to send in bytes</param>
        private static void WriteResponse(HttpListenerContext context, byte[] message)
        {
            try
            {
                Stream output = context.Response.OutputStream;
                output.Write(message, 0, message.Length);
                context.Response.Close();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error writing Listener response to stream", ex);
            }
        }
    }
}
