using System.Text;
using System.Net;
using System.IO;
using System;

namespace SumoController
{
    /// <summary>
    /// Posts(http) to Junction to state vehicle detected
    /// </summary>
    public class VehiclePost
    {
        /// <summary>
        /// Initialises class and sends post
        /// </summary>
        /// <param name="trafficDetector">traffic detector that sensed vehicle</param>
        public VehiclePost(TrafficDetector trafficDetector)
        {
            try
            {
                string requestXml = CreateVehicleRequest(trafficDetector);
                HttpWebRequest request = CreatePost(requestXml, trafficDetector.Url);
                SendPost(request);
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error initialising vehicle post", ex);
            }            
        }

        /// <summary>
        /// Creates SOAP vehicle request as string
        /// </summary>
        /// <param name="trafficDetector">traffic detector that sensed vehicle</param>
        /// <returns>SOAP message as string</returns>
        private string CreateVehicleRequest(TrafficDetector trafficDetector)
        {
            try
            {
                string request = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\""
                + " xmlns:traf=\"http://Traffic.org/\"><soapenv:Header/><soapenv:Body ><traf:CarDetected>"
                + "<NoPlate>" + trafficDetector.LastVehicle + "</NoPlate>"
                + "<DetectorNo>" + trafficDetector.DetectorID + "</DetectorNo>"
                + "<RouteNo>" + trafficDetector.RouteNo + "</RouteNo>"
                + "<SectionNo>" + trafficDetector.SectionNo + "</SectionNo>"
                + "<Direction>" + trafficDetector.Direction + "</Direction>"
                + "</traf:CarDetected ></soapenv:Body ></soapenv:Envelope>";

                return request;
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error creating vehicle request", ex);
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
                throw new SumoControllerException("Error creating post", ex);
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
            catch(Exception ex)
            {
                throw new SumoControllerException("Error writing to data stream", ex);
            }

        }

        /// <summary>
        /// sends the request
        /// </summary>
        /// <param name="request"></param>
        private void SendPost(HttpWebRequest request)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error sending vehicle post", ex);
            }
        }
    }
}
