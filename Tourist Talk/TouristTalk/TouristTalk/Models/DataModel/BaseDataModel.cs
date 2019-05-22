using System;
using System.Collections.Generic;
using TouristTalk.Dependency;
using TouristTalk.Exceptions;

namespace TouristTalk.Models
{
    /// <summary>
    /// used for common data processing, including resolving dependency for data connection
    /// and requesting and recieving data.
    /// All data is placed in _querryResults.
    /// </summary>
    public abstract class BaseDataModel
    {
        protected IDataConnect _dataConnect;

        protected Dictionary<string, object> _dataRequestParameters; //parameters needed to request specific data

        protected List<Dictionary<string, object>> _querryResults = new List<Dictionary<string, object>>();

        public bool Valid { get; protected set; }
        

        /// <summary>
        /// Reads a data item from the data source by its id. 
        /// It then validates and populates the classes properties from the querry result.
        /// </summary>
        /// <param name="itemID">ID of item to be requested</param>
        /// <param name="dataResourceName">the resource name (stored procedure name if database used)</param>
        /// <param name="itemName">The item name within the data soure (parameter name if  database and stored procedure used)</param>
        protected virtual void ReadDataByID(int itemID, string dataResourceName, string itemName)
        {
            try
            {
                BuildReadDataParameters(itemID, itemName);
                SendDataRequest(dataResourceName);
                Validate();
                PopulateProperties(itemID);
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error Reading data (by ID)", e);
            }
        }

        /// <summary>
        /// Builds the parameter details for a data request by an ID.
        /// </summary>
        /// <param name="itemID">ID of item to be requested</param>
        /// <param name="itemName">The item name within the data soure (parameter name if  database and stored procedure used)</param>
        protected virtual void BuildReadDataParameters(int itemID, string itemName)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    {itemName, itemID}
                };

                _dataRequestParameters = parameters;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error building parameter dictionary for ID read", e);
            }
        }

        /// <summary>
        /// Sends the data request (in case of database calls stored procedure)
        /// </summary>
        /// <param name="dataResourceName">the resource name (stored procedure name if database used)</param>
        protected virtual void SendDataRequest(string dataResourceName)
        {
            try
            {
                BuildDependencies();
                _querryResults = _dataConnect.SendDataRequest(dataResourceName, _dataRequestParameters);
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error requesting data", e);
            }
        }

        /// <summary>
        /// Builds dependencies. I.E. Binds the desired data request class to IDataConnect 
        /// </summary>
        protected void BuildDependencies()
        {
            try
            {
                _dataConnect = DependencyFactory.ResolveDataConnect();
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error creating Dependencies", e);
            }
        }

        /// <summary>
        /// For validating returned data, set to true, to be overriden in iherrited classes
        /// </summary>
        protected virtual void Validate()
        {
            _valid = true;
        }

        /// <summary>
        /// Populates class' properties. To be overriden in inherrited class.
        /// Not abstract as not all inherrited classes will use.
        /// </summary>
        /// <param name="itemID">ID of item that was requested</param>
        protected virtual void PopulateProperties(int itemID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an integer data item from querry results. 
        /// sets item to 0 if null
        /// </summary>
        /// <param name="itemName">Name of date item</param>
        /// <param name="required">should item should always be in results</param>
        /// <returns></returns>
        protected int GetIntDataItem(string itemName, bool required)
        {
            try
            {
                object dataItem = GetDataItem(itemName, required);
                if (dataItem == null)
                {
                    return 0;
                }

                return (int)dataItem;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error getting integer data item from querry dictionary", e);
            }
        }

        /// <summary>
        /// Gets a string data item from querry results. 
        /// </summary>
        /// <param name="itemName">Name of date item</param>
        /// <param name="required">should item should always be in results</param>
        /// <returns></returns>
        protected string GetStringDataItem(string itemName, bool required)
        {
            try
            {
                object dataItem = GetDataItem(itemName, required);
                if (dataItem == null)
                {
                    return string.Empty;
                }

                return (string)dataItem;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error getting string data item from querry dictionary", e);
            }
        }

        /// <summary>
        /// Gets a boolean data item from querry results. 
        /// </summary>
        /// <param name="itemName">Name of date item</param>
        /// <param name="required">should item should always be in results</param>
        /// <returns></returns>
        protected bool GetBoolDataItem(string itemName, bool required)
        {
            try
            {
                object dataItem = GetDataItem(itemName, required);
                if (dataItem == null)
                {
                    return false;
                }

                return Convert.ToBoolean(dataItem);
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error getting boolean data item from querry dictionary", e);
            }
        }

        /// <summary>
        /// Gets a data item from querry results. 
        /// If data item is required and missing throws an exception.
        /// </summary>
        /// <param name="itemName">Name of date item</param>
        /// <param name="required">should item should always be in results</param>
        /// <returns></returns>
        protected object GetDataItem(string itemName, bool required)
        {
            try
            {
                if (_querryResults[0].ContainsKey(itemName))
                {
                    return _querryResults[0][itemName];
                }
                if (required)
                {
                    throw new Exception("querry results missing " + itemName + "data");
                }

                return null;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error Getting data Item from querry", e);
            }           
        } 
    }
}