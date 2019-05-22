using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace JunctionController
{
    public class HttpPostListener
    {
        private static readonly HttpListener _listener = new HttpListener();


        public HttpPostListener()
        {
            StartListener();
        }

        /// <summary>
        /// Start the listener
        /// </summary>
        private static void StartListener()
        {
            try
            {
                SetListenerURL();
                _listener.Start();
                Listen();
                Console.WriteLine("Listening for vehicle posts...");
                Console.WriteLine("Press any key to stop junction...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error starting traffic light listener", ex);
            }

        }

        /// <summary>
        /// retrieve the url from config and set listener url
        /// </summary>
        private static void SetListenerURL()
        {
            try
            {
                string uRL = ConfigReader.GetConnectionString("HttpListenerURL");
                _listener.Prefixes.Add(uRL);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error setting listener URL", ex);
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
                Console.WriteLine("Recieved Post");
                Task.Factory.StartNew(() => ProcessRequest(context));
            }
        }


        /// <summary>
        /// Processes incoming vehicle request
        /// </summary>
        /// <param name="context">http listener context</param>
        private static void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                string body = new StreamReader(context.Request.InputStream).ReadToEnd().Trim();
                RetrieveDataFromRequest(body);
                SendResponse(context);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error processsing request", ex);
            }

        }

        /// <summary>
        /// Retrieves data items from request
        /// Senses if config change or vehicle request
        /// </summary>
        /// <param name="body">request body as string</param>
        private static void RetrieveDataFromRequest(string body)
        {
            try
            {
                if (ConfigChange(body))
                {
                    Console.WriteLine("Recieved Config Change Request");
                    ProcessConfigChange(body);
                }
                else
                {
                    Console.WriteLine("Recieved vehicle sensor request");
                    ProcessANR(body);
                }
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error Retrieving data from request", ex);
            }
        }
        /// <summary>
        /// Checks to see if request s a config change request        /// 
        /// </summary>
        /// <param name="body">request body as string</param>
        /// <returns></returns>
        private static bool ConfigChange(string body)
        {
            try
            {
                return body.Contains("Config");
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error establishing if request is a config change", ex);
            }            
        }

        /// <summary>
        /// Process config change request 
        /// </summary>
        /// <param name="body">request body as string</param>
        private static void ProcessConfigChange(string body)
        {
            try
            {
                ConfigRequest configRequest = new ConfigRequest(body);
                Junction.AdjustConfig(configRequest.Items);
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error processing config change request", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body">request body as string</param>
        private static void ProcessANR(string body)
        {
            try
            {
                ANRRequest aNRRequest = new ANRRequest(body);
                VehicleData vehicleData = new VehicleData(aNRRequest.Items);
                Console.WriteLine("Vehicle: " + vehicleData.NumberPlate
                                  + " RouteNo: " + vehicleData.RouteNo.ToString()
                                  + " SectionNo: " + vehicleData.SectionNo.ToString()
                                  + " Direction: " + vehicleData.Direction
                                  + " Pollution: " + vehicleData.Pollution);
                Junction.AdjustTiming(vehicleData);
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error processing ANR request", ex);
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
                throw new JunctionException("Error sending response", ex);
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
                throw new JunctionException("Error creating response message", ex);
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
                throw new JunctionException("Error creating Listenser response", ex);
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
                throw new JunctionException("Error writing Listener response to stream", ex);
            }
        }

    }
}
