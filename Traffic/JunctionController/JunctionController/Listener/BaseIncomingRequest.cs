using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace JunctionController
{   
    /// <summary>
    /// Base class for incoming data requests
    /// </summary>
    public abstract class BaseIncomingRequest
    {
        protected string _elementName;

        public Dictionary<string, string> Items { get; private set; }

        /// <summary>
        /// Initialises class. Retrieves data items from xml and places in the Items dictionary
        /// </summary>
        /// <param name="body">request body</param>
        public BaseIncomingRequest(string body)
        {
            try
            {
                SetElementName();
                List<string> itemsToRetrieve = ItemsToRetrieve();
                XElement element = GetElement(body);
                GetItems(element, itemsToRetrieve);
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error initialising incoming request", ex);
            }
        }

        /// <summary>
        /// Set the name of the parent element containing required data
        /// </summary>
        protected abstract void SetElementName();

        /// <summary>
        /// Sets the list of data items to retrieve
        /// </summary>
        /// <returns>list of names of required data items</returns>
        protected abstract List<string> ItemsToRetrieve();

        /// <summary>
        /// Retrieves the XML Element containing the required data from a SOAP message
        /// </summary>
        /// <param name="body">the request body</param>
        /// <returns>XML Element containing the required data</returns>
        private XElement GetElement(string body)
        {
            try
            {
                XDocument request = XDocument.Parse(body);
                XElement envelope = GetXElement(request, "Envelope");
                XElement soapBody = GetXElement(envelope, "Body");
                return GetXElement(soapBody, _elementName);
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error getting XML parent element from SOAP message", ex);
            }
        }

        /// <summary>
        /// get an xml element from xml
        /// </summary>
        /// <param name="xContainer">Xml object</param>
        /// <param name="elementName">element to extract</param>
        /// <returns>the specified xml element</returns>
        private XElement GetXElement(XContainer xContainer, string elementName)
        {
            try
            {
                return xContainer.Elements().Where(item => item.Name.LocalName == elementName).First();
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error getting XML element", ex);
            }
        }

        /// <summary>
        /// Gets each item specified from the xml and loads it into the Items dictionary
        /// </summary>
        /// <param name="xml">xml containing data items</param>
        /// <param name="itemsToRetrieve">list of data items to retrieve</param>
        private void GetItems(XElement xml, List<string> itemsToRetrieve)
        {
            try
            {
                Items = new Dictionary<string, string>();
                foreach (string item in itemsToRetrieve)
                {
                    Items.Add(item, GetStringFromXml(xml, item));
                }
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error retriveing data items from xml element", ex);
            }
        }

        /// <summary>
        /// Retrieves value of xml item as a string
        /// </summary>
        /// <param name="xElement">Element that item is within</param>
        /// <param name="elementName">name of item to retrieve value for</param>
        /// <returns>value of xml item as a string</returns>
        private string GetStringFromXml(XElement xElement, string elementName)
        {
            try
            {
                return GetXElement(xElement, elementName).Value;
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error getting string value from xml element");
            }            
        }
    }
}
