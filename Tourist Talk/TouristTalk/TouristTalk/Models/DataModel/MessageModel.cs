using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using TouristTalk.Exceptions;

namespace TouristTalk.Models
{
    /// <summary>
    /// Defines, retieves and adds Message data
    /// </summary>
    public class MessageModel : BaseDataModel
    {
        public int ID { get; protected set; }
        public int SenderID { get; protected set; }
        public int RecipientID { get; protected set; }
        public string Text { get; protected set; }
        public int FileID { get; protected set; }
        public string FileType { get; protected set; }
        public DateTime MessageTime { get; protected set; }        
       
        /// <summary>
        ///  Intialises class with message ID. Gets data for message by id.
        /// </summary>
        /// <param name="messageID">message ID</param>
        public MessageModel(int messageID)
        {
            try
            {
                ReadDataByID(messageID, "usp_GetMessage", "MessageID");
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error initialising message using message ID", e);
            }
        }

        /// <summary>
        /// Initialises class with message details - adds a message to data repository
        /// </summary>
        /// <param name="conversationID">Conversation ID</param>
        /// <param name="senderID">ID of who sent the message</param>
        /// <param name="recipientID">ID of who recieved the message</param>
        /// <param name="text">the message text</param>
        /// <param name="httpFile">file data in httpFile form</param>
        public MessageModel(int conversationID, int senderID, int recipientID, string text, HttpPostedFileBase httpFile)
        {
            try
            {
                byte[] file = ConvertHttpFileToByte(httpFile);
                string fileType = GetFileType(httpFile);
                BuildDataParameters(conversationID, senderID, recipientID, text, file, fileType);
                SendDataRequest("usp_AddMessage");
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error initialising message using details (adding message)", e);
            }
        }

        /// <summary>
        /// Builds Parameters for adding message to data repository
        /// </summary>
        /// <param name="conversationID">Conversation ID</param>
        /// <param name="senderID">ID of who sent the message</param>
        /// <param name="recipientID">ID of who recieved the message</param>
        /// <param name="text">the message text</param>
        /// <param name="file">file data in ByteForm</param>
        /// <param name="fileType">file type (MIME)</param>
        protected void BuildDataParameters(int conversationID, int senderID, int recipientID, string text, byte[] file, string fileType)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    {"ConversationID", conversationID},
                    {"SenderID ", senderID},
                    {"RecipientID", recipientID},
                    {"MessageText", text},
                    {"MessageFile", file},
                    {"MessageFileType", fileType}
                };

                _dataRequestParameters = parameters;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error building parameters for adding Message data request", e);
            }
        }

        /// <summary>
        /// Populate message details from querry result
        /// </summary>
        /// <param name="itemID">message id</param>
        protected override void PopulateProperties(int itemID)
        {
            try
            {
                ID = itemID;
                SenderID = GetIntDataItem("SenderID", true);
                RecipientID = GetIntDataItem("RecipientID", true);
                Text = GetStringDataItem("MessageText", false);
                FileID = GetIntDataItem("MessageFileID", false);
                FileType = GetStringDataItem("MessageFileType", false);
                MessageTime = (DateTime)_querryResults[0]["AddedTime"];
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error populating message properties from querry data", e);
            }
        }

        /// <summary>
        /// Converts the http file to a byte file
        /// </summary>
        /// <param name="httpFile">The http file</param>
        /// <returns>a Byte file</returns>
        protected byte[] ConvertHttpFileToByte(HttpPostedFileBase httpFile)
        {
            try
            {
                if (httpFile == null)
                {
                    return null;
                }
                MemoryStream memoryStream = new MemoryStream();
                httpFile.InputStream.CopyTo(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                return byteFile;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error converting http file to byte", e);
            }
        }

        /// <summary>
        /// Gets the file type (MIME) from a http file
        /// </summary>
        /// <param name="httpFile"></param>
        /// <returns>file type (MIME) as string</returns>
        protected string GetFileType(HttpPostedFileBase httpFile)
        {
            try
            {
                if (httpFile == null)
                {
                    return null;
                }
                return httpFile.ContentType;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error getting file type from http file", e);
            }
        }
    }
}