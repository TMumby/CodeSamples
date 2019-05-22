
namespace TouristTalk.Models
{
    /// <summary>
    /// Model for Message View
    /// Acts as conduit between view and Message model.
    /// </summary>
    public class MessageViewModel
    {
        public MessageModel Message { get; protected set; }
        public string Align { get; protected set; }

        public string UserName { get; protected set; } 
        public string BackgroundColour { get; protected set; }

        /// <summary>
        /// Initialises class, sets whether messages should be displayed on
        /// left or right depending on if current user
        /// </summary>
        /// <param name="message">message details</param>
        /// <param name="conversation">conversation details</param>
        public MessageViewModel(MessageModel message, ConversationViewModel conversation)
        {
            Message = message;
            if (conversation.CurrentUserID == message.SenderID)
            {
                Align = "margin-left:0; margin-right:auto;";
                UserName = conversation.CurrentUser;
                BackgroundColour = "gainsboro";
            }
            else
            {
                Align = "margin-left:auto; margin-right:0;";
                UserName = conversation.OtherUser;
                BackgroundColour = "cornsilk";
            }
        }
    }
}