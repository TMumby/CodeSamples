using System;
using System.Text;
using System.Net;
using System.IO;

namespace JunctionController
{
    /// <summary>
    /// Class for orchestrating sendig message to 
    /// the infastructure controller to change the light phase
    /// </summary>
    public class PhasePost
    
    {
        /// <summary>
        /// Initialises class and sends post
        /// </summary>
        /// <param name="trafficDetector">traffic detector that sensed vehicle</param>
        public PhasePost(CurrentState currentState)
        {
            try
            {
                string requestXml = CreateSignalRequest(currentState.Phase);
                HttpWebRequest request = CreatePost(requestXml, GetURL());
                SendPost(request);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error initialising vehicle post", ex);
            }
        }

        /// <summary>
        /// retrieves the infastrucure controller url
        /// </summary>
        /// <returns>infastructure url</returns>
        private string GetURL()
        {
            try
            {
                return ConfigReader.GetConnectionString("InfastructureControllerURL");
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error retrieving infastructure controller", ex);
            }            
        }

        /// <summary>
        /// Creates the signal/phase change request body
        /// </summary>
        /// <param name="phaseNo">the phase/state number to change the lights to</param>
        /// <returns>Phase request as string</returns>
        private string CreateSignalRequest(Phase phase)
        {
            try
            {
                string trafficLightNo = GetTrafficLightNo();

                string request = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\""
                    + " xmlns:traf=\"http://Traffic.org/\"><soapenv:Header/>"
                    + "<soapenv:Body >"
                    + "<traf:SignalState>" + phase.Definition + "</traf:SignalState >"
                    + "<traf:JunctionNo>" + trafficLightNo + "</traf:JunctionNo>"
                    + "</soapenv:Body ></soapenv:Envelope>";

                return request;
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error creating phase post request as string", ex);
            }
        }

        /// <summary>
        /// retrieves traffic light number to define which traffic light in the infastructure controller
        /// </summary>
        /// <returns>traffic light number as string</returns>
        private string GetTrafficLightNo()
        {
            try
            {
                return ConfigReader.GetAppSettingString("LightNo");
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error getting trffic light n umber from config", ex);
            }
            
        }

        /// <summary>
        /// Creates HTTP Request from string
        /// </summary>
        /// <param name="requestXml">SOAP request</param>
        /// <param name="url">Junction to send to</param>
        /// <returns>Http request with request body added</returns>
        private HttpWebRequest CreatePost(string requestXml, string url)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(requestXml);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                request = WriteToDataStream(data, request);

                return request;
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error creating post", ex);
            }
        }

        /// <summary>
        ///  writes http request to stream 
        /// </summary>
        /// <param name="data">encocded request data</param>
        /// <param name="request">http request</param>
        /// <returns></returns>
        private HttpWebRequest WriteToDataStream(byte[] data, HttpWebRequest request)
        {
            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                return request;
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error writing to data stream", ex);
            }
        }


        /// <summary>
        /// sends the request
        /// </summary>
        /// <param name="request">http request</param>
        private async void SendPost(HttpWebRequest request)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error sending vehicle post", ex);
            }
        }
    }


}


    

