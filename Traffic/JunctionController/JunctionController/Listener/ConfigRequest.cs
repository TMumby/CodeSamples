using System;
using System.Collections.Generic;

namespace JunctionController
{
    /// <summary>
    /// Processes an incoming configuration change request
    /// </summary>
    public class ConfigRequest : BaseIncomingRequest
    {
        /// <summary>
        /// Initialises class. Retrieves config data items from xml and places in the Items dictionary
        /// by utilising base class
        /// </summary>
        public ConfigRequest(string body):base(body)
        {

        }

        /// <summary>
        /// Sets the parent element name that (config) data resides within
        /// </summary>
        protected override void SetElementName()
        {
            try
            {
                _elementName = "Config";
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error setting parent element name for config", ex);
            }            
        }

                /// <summary>
        /// Sets the config data items to retrieve
        /// </summary>
        /// <returns>A list of the names of config data items to be retrieved</returns>
        protected override List<string> ItemsToRetrieve()
        {
            try
            {
                List<string> itemList = new List<string>()
            {
                {"CycleTime"},
                {"Route0Section0Factor"},
                {"Route0Section1Factor"},
                {"Route1Section0Factor"},
                {"Route1Section1Factor"}
            };
                return itemList;
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieving config data items", ex);
            }
        }       
    }
}