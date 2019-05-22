using System;
using System.Collections.Generic;
using TouristTalk.Exceptions;

namespace TouristTalk.Models
{
    /// <summary>
    /// Model for retrieving, adding and authorisng user details
    /// </summary>
    public class UserModel : BaseDataModel
    {
        public int UserID { get; protected set; }
        public string UserName { get; protected set; }
        public string Password { get; protected set; }
        public bool TourAgent { get; protected set; }                
        public bool Authenticated { get; protected set; }  

        /// <summary>
        /// Initialises user with credentials - Authorises user
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="password">Password</param>
        public UserModel(string userName, string password)
        {
            try
            {
                BuildDataParameters(userName, password);
                SendDataRequest("usp_ValidateUser");
                Validate();
                PopulateProperties(userName, password);
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error inititialising UserModel with username and password", e);                
            }      
        }

        /// <summary>
        ///  Intialises class - gets User details with user ID
        /// </summary>
        /// <param name="userID">User ID</param>
        public UserModel(int userID)
        {
            try
            {
                ReadDataByID(userID, "usp_GetUserByID", "UserID");
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error initialiaing User Model with ID", e);
            }
        }

        /// <summary>
        /// Builds Parameters for authorisation request using suppled credentials
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="password">Password</param>
        protected void BuildDataParameters(string userName, string password)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    {"UserName", userName},
                    {"Password", password}
                };

                _dataRequestParameters = parameters;
            }
            catch (Exception e)
            {
                throw new TouristTalkException( "Error building parameter dictionary using username and password", e);
            }
        }

        /// <summary>
        /// Validates user - if only one record in result set then valid user and passed authorisation
        /// </summary>
        protected override void Validate()
        {
            try
            {                
                if (_querryResults.Count == 1) 
                {
                    _valid = true;
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error whilst validating user from querry data", e);
            }
        }

        /// <summary>
        /// Populates User properties if authorised
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="password">Password</param>
        protected void PopulateProperties(string userName, string password)
        {
            try
            {
                if (_valid)
                {
                    UserID = GetIntDataItem("UserID", true);
                    UserName = userName;
                    Password = password;
                    TourAgent = GetBoolDataItem("TourAgent", false);
                    Authenticated = true;
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Exception Populating User Properties", e);
            }
        }

        /// <summary>
        ///  populates user properties when class has been initiated by ID 
        ///  (therefore no authorisation)
        /// </summary>
        /// <param name="userID"></param>
        protected override void PopulateProperties(int userID)
        {
            try
            {
                UserID = userID;
                UserName = GetStringDataItem("UserName", true);
                TourAgent = GetBoolDataItem("TourAgent", false);
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Exception Populating User Properties", e);
            }
        }
    }
}