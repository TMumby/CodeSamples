using System.Collections.Generic;
using System;
using System.Web;
using System.ComponentModel.DataAnnotations;
using TouristTalk.Validation;
using TouristTalk.Exceptions;

namespace TouristTalk.Models
{
    /// <summary>
    /// Used to hold display data items for conversation view
    /// Acts as conduit between conversation view and conversation model.
    /// </summary>
    public class ConversationViewModel
    {
        public int ConversationID { get; set; }
        public string Title { get; set; }
        public string OtherUser { get; set; }
        public string CurrentUser { get; set; }
        public DateTime StartTime { get; set; }
        public string InstigatorName { get; set; }  //who started the conversation
        [ScaffoldColumn(false)]
        public List<MessageModel> Messages { get; set; }
        public int CurrentUserID { get; protected set; }

        [Display(Name = "Message")]
        public string NewMessageText { get; set; }       
        
        [AllowedFileType] //limits file types, custom attribute
        public HttpPostedFileBase File { get; set; }
       
        //Initialiser can not be deleted as used in binding model to view
        public ConversationViewModel() { }

        /// <summary>
        ///  Initialises class, gets conversation data to be displayed in view
        /// </summary>
        /// <param name="conversation"></param>
        public ConversationViewModel(ConversationModel conversation)
        {
            try
            {                
                ConversationID = conversation.ID;
                Title = conversation.Title;
                OtherUser = conversation.OtherUser.UserName;
                CurrentUser = conversation.CurrentUser.UserName;
                StartTime = conversation.StartTime;
                InstigatorName = conversation.InstigatorName;
                Messages = conversation.Messages;
                CurrentUserID = conversation.CurrentUser.UserID;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error while initialising ConversationViewModel", e);
            }
        }
    }
}