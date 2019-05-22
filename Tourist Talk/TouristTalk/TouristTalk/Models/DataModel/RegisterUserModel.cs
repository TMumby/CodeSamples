using System;
using System.Collections.Generic;
using TouristTalk.Exceptions;


namespace TouristTalk.Models
{
    /// <summary>
    /// Model for adding user to database
    /// </summary>
    public class RegisterUserModel : BaseDataModel
    {
        
        public string UserName { get; protected set; }
        public string Password { get; protected set; }
        public bool Success { get; protected set;  }

        /// <summary>
        /// Intialises class - adds user to data repository
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="password">User Password</param>
        public RegisterUserModel(string userName, string password, string email, string telNo)
        {
            try
            {
                BuildDataParameters(userName, password, email, telNo);
                SendDataRequest("usp_AddUser");
                Validate();
                PopulateProperties(userName, password);
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error Initialising RegisterUser- To add User", e);
            }
        }

        /// <summary>
        /// Build parameters for adding user to data repository
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="password">User Password</param>
        protected void BuildDataParameters(string userName, string password, string email, string telNo)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    {"UserName", userName},
                    {"Password", password},
                    {"Email", email},
                    {"TelNo", telNo},
                };

                _dataRequestParameters = parameters;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error building data request parameters for registering user", e);
            }
        }

        /// <summary>
        ///  Validates - if not existing user then valid
        /// </summary>
        protected override void Validate()
        {
            try
            {
                if (GetBoolDataItem("OtherUser", true))
                {
                    _valid = false;
                    Success = false;
                }
                else
                {
                    Success = true;
                    base.Validate();
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error Validating if user was registered", e);
            }

        }

        /// <summary>
        /// populates classes properties
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="password">User Password</param>
        protected void PopulateProperties(string userName, string password)
        {
            try
            {
                if (_valid)
                {
                    UserName = userName;
                    Password = password;
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error setting RegisterUser properties", e);
            }         
        }
    }
}