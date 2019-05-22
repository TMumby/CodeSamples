using System;
using System.Collections.Generic;
using System.Web;
using TouristTalk.Exceptions;

namespace TouristTalk.Models
{
    /// <summary>
    /// Holds details of a single conversation. 
    /// A conversation abstractly is a collection of messages between two users
    /// </summary>
    public class ConversationModel : BaseDataModel
    {        
        public int ID { get; protected set; } //Conversation ID
        public string Title { get; protected set; }
        public DateTime StartTime { get; protected set; }
        public UserModel CurrentUser { get; protected set; }
        public UserModel OtherUser { get; protected set; }  //Other user that conversation is with
        public string InstigatorName { get; protected set; }  //who started the conversation
        public List<MessageModel> Messages { get; protected set; }                

        /// <summary>
        /// Initialises conversation by reading a conversation from the data source by its ID.
        /// </summary>
        /// <param name="conversationID"> Conversation ID</param>
        public ConversationModel(int conversationID)
        {
            try
            {
                CurrentUser = SessionHandler.UserDetails;
                ReadDataByID(conversationID, "usp_GetConversation", "ConversationID");
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error while initialising Conversation Model using an id", e);
            }
        }

        /// <summary>
        /// Initialises conversation - adds a conversation to the data repository.
        /// </summary>
        /// <param name="tourAgent">Who is the conversation to be started with</param>
        /// <param name="title">short description of conversation</param>
        public ConversationModel(TourAgentModel tourAgent, string title)
        {
            try
            {
                CurrentUser = SessionHandler.UserDetails;
                BuildDataParameters(tourAgent, title);
                SendDataRequest("usp_AddConversation");
                Validate();
                PopulateProperties();
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error initialising conversation using details", e);
            }
        }

        /// <summary>
        /// Creates a message, this adds message to data repository
        /// </summary>
        /// <param name="text">message text</param>
        /// <param name="httpFile">message file, null is allowed</param>
        public void AddMessage(string text, HttpPostedFileBase httpFile)
        {
            try
            {
                MessageModel message = new MessageModel(ID, CurrentUser.UserID, OtherUser.UserID, text, httpFile);
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error adding message", e);
            }
        }

        /// <summary>
        ///  Builds data paramenters for sending data request for adding Conversation
        /// </summary>
        /// <param name="tourAgent">Who is the conversation to be started with</param>
        /// <param name="title">short description of conversation</param>
        protected void BuildDataParameters(TourAgentModel tourAgent, string title)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    {"UserID", CurrentUser.UserID},
                    {"TourAgentID ", tourAgent.ID},
                    {"Title", title},
                };
                _dataRequestParameters = parameters;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Exception trying to build parameters for adding a conversation", e);
            }
        }

        /// <summary>
        /// Validates a conversation. A user must be someone within conversation.
        /// Throws an exception if they are not.
        /// </summary>
        protected override void Validate()
        {
            try
            {
                if (CurrentUser.UserID != GetIntDataItem("UserID", true)
                    && CurrentUser.UserID != GetIntDataItem("TourAgentID", true))
                {
                    throw new TouristTalkException("Conversation not valid for user");
                }

                base.Validate();
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Exception encountered validating conversation", e);
            }      
        }

        /// <summary>
        /// Populates properties including message list
        /// </summary>
        /// <param name="itemID">Conversation ID</param>
        protected override void PopulateProperties(int itemID)
        {
            try
            {
                PopulateProperties();
                GetMessageListData();
                PopulateMessageList();
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Exception populating conversation model properties using ID", e);
            }
        }

        /// <summary>
        /// Populates properties for conversation
        /// </summary>
        protected void PopulateProperties()
        {
            try
            {
                ID = GetIntDataItem("TourConversationID", true);                
                OtherUser = GetOtherUser();
                InstigatorName = GetInstigatorName();
                Title = GetStringDataItem("Title", false);
                StartTime = (DateTime)_querryResults[0]["AddedTime"];
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Exception populating conversation model properties", e);
            }
        }

        /// <summary>
        /// Gets who started the conversation, can be current user or another user
        /// </summary>
        /// <returns>the instigators name</returns>
        protected string GetInstigatorName()
        {
            try
            {
                if (GetIntDataItem("UserID", true) == CurrentUser.UserID)
                {
                    return CurrentUser.UserName;
                }
                else
                {
                    return OtherUser.UserName;
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Exception gettting conversation instigator data", e);
            }
        }

        /// <summary>
        /// Gets the other user's (the othe person in the conversation) details.
        /// If the current user is not the touragent get the touragent details
        /// othewise get the current user details
        /// </summary>
        /// <param name="tourAgentID"></param>
        /// <returns></returns>
        protected UserModel GetOtherUser()
        {
            try
            {
                int tourAgentID = GetIntDataItem("TourAgentID", true);

                if (CurrentUser.UserID != tourAgentID)
                {
                    return new UserModel(tourAgentID);
                }
                else
                {
                    return new UserModel(GetIntDataItem("UserID", true));
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Exception gettting other user data", e);
            }
        }

        /// <summary>
        /// Gets the list of messages in a conversation
        /// </summary>
        protected void GetMessageListData()
        {
            try
            {
                BuildReadDataParameters(ID, "ConversationID");
                SendDataRequest("usp_GetMessageIDs");
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Exception aquiring List of Messages", e);
            }         
        }

        /// <summary>
        ///  Adds message details to this classes message list (Messages)
        ///  for every message
        /// </summary>
        protected void PopulateMessageList()
        {
            try
            {
                Messages = new List<MessageModel>();

                foreach (Dictionary<string, object> messageResult in _querryResults)
                {
                    //Get message ID 
                    int messageID = (int)messageResult["TourMessageID"];

                    //Get message details
                    MessageModel message = new MessageModel(messageID);

                    //Add to this classes messages
                    Messages.Add(message);
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Exception populating Messages from querry data", e);
            }            
        }
    }
}