using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TouristTalk.Models
{
    /// <summary>
    /// Model for MyConversations View
    /// </summary>
    public class MyConversationsViewModel
    {
        [ScaffoldColumn(false)]
        public List<ConversationModel> ConversationList { get; protected set; }

        /// <summary>
        /// Initialises class
        /// Gets all conversation details by invoking conversationList
        /// </summary>
        public MyConversationsViewModel()
        {
            ConversationListModel conversationList = new ConversationListModel();
            ConversationList = conversationList.ConversationList;
        }
    }
}