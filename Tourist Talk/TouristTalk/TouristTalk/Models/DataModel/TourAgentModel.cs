using System.Collections.Generic;
using System;
using TouristTalk.Exceptions;

namespace TouristTalk.Models
{
    /// <summary>
    /// Model for retrieving all tour agents and checking that a user is a tour agent
    /// </summary>
    public class TourAgentModel : BaseDataModel
    {            
        public int ID { get; protected set; }
        public string Name { get; protected set; }
        public bool ValidAgent { get; protected set; }

        
        protected Dictionary<int, string> _allAgents;
        /// <summary>
        /// Gets All valid TourAgents
        /// </summary>
        public Dictionary<int, string> AllAgents
        {
            get
            {
                GetAllAgents();
                return _allAgents;
            }
            protected set { _allAgents = value; }
        }

        public TourAgentModel() { }

        /// <summary>
        /// Initialises class with TourAgent ID. 
        /// Makes sure user is a tour agent and gets there details
        /// </summary>
        /// <param name="id">TourAgent ID</param>
        public TourAgentModel(int id)
        {
            try
            {
                ReadDataByID(id, "usp_GetTourAgents", null);
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error initilising TourAgent Model with id", e);
            }            
        }

        /// <summary>
        /// If not already populated gets all Tour Agents
        /// </summary>
        protected void GetAllAgents()
        {
            try
            {
                if (_allAgents == null || _allAgents.Count == 0)
                {
                    _dataRequestParameters = null;
                    SendDataRequest("usp_GetTourAgents");
                    _allAgents = GetTourAgentsFromQuerryResponse();
                }
            }
            catch (Exception e)
            {
                throw new TimeoutException("Error retriving Tour Agents", e);
            }
        }
        /// <summary>
        /// Retrieves Tour Agents from querry results
        /// </summary>
        /// <returns></returns>
        protected Dictionary<int, string> GetTourAgentsFromQuerryResponse ()
        {
            try
            {
                Dictionary<int, string> allAgents = new Dictionary<int, string>();
                foreach (Dictionary<string, object> agent in _querryResults)
                {
                    allAgents.Add((int)agent["TourAgentID"], agent["TourAgentName"].ToString());
                }
                return allAgents;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error extracting agents from querry response", e);
            }         
        }

        /// <summary>
        /// Sets the data parameter for request for Tour Agents to Null
        /// as no required parameters
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="itemName"></param>
        protected override void BuildReadDataParameters(int itemID, string itemName)
        {
            _dataRequestParameters = null;
        }

        /// <summary>
        /// Validates a Tour Agent by 
        /// ensuring in Tour Agent list
        /// </summary>
        /// <param name="id">User ID</param>
        protected void ValidateAgent(int id)
        {
            try
            {
                if (AllAgents.ContainsKey(id))
                {
                    base.Validate();
                }
                _valid = false;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error Validating Agent", e);
            }
        }
        /// <summary>
        /// populates classes properties - Agent details
        /// </summary>
        /// <param name="id">User ID</param>
        protected override void PopulateProperties(int id)
        {
            try
            {
                if (_valid)
                {
                    ID = id;
                    Name = AllAgents[id];
                    ValidAgent = true;
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error populating TourAgent properties", e);
            }
        }
    }
}