using System;
using System.Collections.Generic;
using TouristTalk.Exceptions;

namespace TouristTalk.Models
{
    /// <summary>
    /// List of all conversation for a particular user
    /// </summary>
    public class ConversationListModel:BaseDataModel
    {
        public int UserID { get; protected set;  }
        public List<ConversationModel> ConversationList { get; protected set; }

        /// <summary>
        /// Initialises ConversationList. Gets all conversations for user.
        /// </summary>
        public ConversationListModel()
        {
            try
            {
                int userID = SessionHandler.UserDetails.UserID;
                ReadDataByID(userID, "usp_GetConversationsForUser", "UserID");
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error Inititialising converation list", e);
            }
        }

        /// <summary>
        /// Populates conversation list with conversations
        /// </summary>
        /// <param name="userID">ID  of user</param>
        protected override void PopulateProperties(int userID)
        {
            try
            {
                UserID = userID;
                ConversationList = new List<ConversationModel>();
                foreach (Dictionary<string, object> conversation in _querryResults)
                {
                    ConversationList.Add(new ConversationModel((int)conversation["TourConversationID"]));
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error populating conversation list", e);
            }
        }
    }
}