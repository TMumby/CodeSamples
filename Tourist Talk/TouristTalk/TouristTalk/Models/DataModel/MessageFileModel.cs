using System;
using TouristTalk.Exceptions;

namespace TouristTalk.Models
{
    /// <summary>
    /// Model for adding and retrieving file data
    /// </summary>
    public class MessageFileModel : BaseDataModel
    {
        public int FileID { get; protected set; }
        public int MessageID { get; protected set; }
        public string FileType {get; protected set; }
        public Byte[] Data { get; protected set; } //for file

        protected int _userID;
        
        /// <summary>
        ///  Initialises class - gets a file from data source 
        ///  and checks it is valid for user
        /// </summary>
        /// <param name="fileID">File ID </param>
        /// <param name="userID">user ID to validate against</param>
        public MessageFileModel(int fileID, int userID)
        {
            try
            {
                _userID = userID;
                ReadDataByID(fileID, "usp_GetFile", "FileID");
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error initialising message file with fileID and userID", e);
            }
        }

        /// <summary>
        /// Valisdates file against user information.
        /// User must be either reciever or sender of message
        /// otherwise an exception is thrown
        /// </summary>
        protected override void Validate()
        {
            try
            {
                if (_userID != GetIntDataItem("SenderID", true) && _userID != GetIntDataItem("RecipientID", true))
                {
                    throw new Exception("Not authorised to view this content");
                }
                base.Validate();
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error Validating Message File", e);
            }
        }

        /// <summary>
        /// Populates class properties from ID, 
        /// i.e. file information
        /// </summary>
        /// <param name="itemID">file ID</param>
        protected override void PopulateProperties(int itemID)
        {
            try
            {
                if (_valid)
                {
                    FileID = GetIntDataItem("MessageFileID", true);
                    MessageID = GetIntDataItem("MessageID", true);
                    FileType = GetStringDataItem("MessageFileType", true);
                    Data = (byte[])_querryResults[0]["MessageFile"];
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error populating message file details", e);
            }
        }      
    }
}