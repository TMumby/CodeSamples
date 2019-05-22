using System;
using System.Collections.Generic;


namespace JunctionController
{
    /// <summary>
    /// Processes an incoming request giving details of vehicle data
    /// </summary>
    public class ANRRequest : BaseIncomingRequest
    {

        /// <summary>
        /// Initialises class. Retrieves vehicle data items from xml and places in the Items dictionary
        /// by utilising base class
        /// </summary>
        /// <param name="body">request body</param>
        public ANRRequest(string body):base(body)
        {

        }
        
        /// <summary>
        /// Sets the parent element name that (vehicle) data resides within
        /// </summary>
        protected override void SetElementName()
        {
            try
            {
                _elementName = "CarDetected";
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error setting parent element name", ex);
            }            
        }

        /// <summary>
        /// Sets the vehicle data items to retrieve
        /// </summary>
        /// <returns>A list of the names of vehicle data items to be retrieved</returns>
        protected override List<string> ItemsToRetrieve()
        {
            try
            {
                List<string> itemList = new List<string>()
                {
                    {"NoPlate"},
                    {"RouteNo"},
                    {"SectionNo"},
                    {"Direction"}
                };
                return itemList;
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error retrieving vehicle data items", ex);
            }

        }
    }
}
