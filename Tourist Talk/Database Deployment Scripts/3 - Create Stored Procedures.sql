USE TouristMessenger;
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

--**************************************************************************
--Title: usp_ErrorHandler
--Description: Produces standardised error message
--Parameter: uses in built sql server error functions to acquire details,
-- so no parameters needed
--Returns:Re raises error with standard error format
--**************************************************************************
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_ErrorHandler')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_ErrorHandler;
END
GO
CREATE PROCEDURE dbo.usp_ErrorHandler
AS
BEGIN
		--construct error message
		DECLARE @Message NVARCHAR(4000);
		DECLARE @ErrorNumber INT = ERROR_NUMBER();
		DECLARE @ErrorLine INT = ERROR_LINE();
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		DECLARE @ErrorProcedure nvarchar(200) = ERROR_PROCEDURE();

		SET @Message = N'Error Occured in '
		                + COALESCE(ERROR_PROCEDURE(),'') 
						+ N': Error Message: '+ COALESCE(ERROR_MESSAGE(),'') 
						+ N', Error Number: ' + COALESCE(CAST(ERROR_NUMBER() AS NVARCHAR(10)),'') 
						+ N', Error Severity: ' + COALESCE(CAST(ERROR_SEVERITY() AS NVARCHAR(4)),'')  
						+ N', Error State: ' + COALESCE(CAST(ERROR_STATE() AS NVARCHAR(4)),'')  
						+ N', Error Line: ' + COALESCE(CAST(ERROR_LINE() AS NVARCHAR(4)),'');
					 
		--Raise Error
		RAISERROR(@Message, 11, @ErrorSeverity);
END
GO

--**************************************************************************
--Title: usp_ValidateUser
--Description:For Authorising user  
--Parameter @UserName: User Name		
--Parameter @Password: Users password attempt
--Returns: If User Exists and password correct returns whever the User is
--             Tour Agent, otherwise no records returned       
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_ValidateUser')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_ValidateUser;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_ValidateUser
(
	@UserName nvarchar(50),
	@Password nvarchar(50)
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		--validate input
		IF @UserName  IS NULL
		BEGIN
			RAISERROR(N'@UserName Parameter is Null', 16, 11)			
		END
		IF @UserName  = '' 
		BEGIN
			RAISERROR(N'@ Parameter is Empty', 16, 12)			
		END			
		IF @Password IS NULL
		BEGIN
			RAISERROR(N'@Password Parameter is Null', 16, 13)			
		END
		IF @Password  = '' 
		BEGIN
			RAISERROR(N'@Password Parameter is Empty', 16, 14)			
		END	

		--return result		
		SELECT UserID, 
		       TourAgent				   
		FROM Users 
		WHERE UserName = @UserName
        AND Password = @Password
		AND Valid = 'True';
						
		RETURN 0;

	END TRY
	BEGIN CATCH
		--call error handler
		EXEC dbo.usp_ErrorHandler;		
		RETURN -1;
	END CATCH
END
GO
--**************************************************************************
--Title: usp_GetUserByID 
--Description: Gets a User by there ID
--Parameter @UserID: User ID 
--Returns: User Details         
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_GetUserByID')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_GetUserByID;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_GetUserByID
(
	@UserID INT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;		
		
		--validate input
		IF @UserID  IS NULL
		BEGIN
			RAISERROR(N'@UserID is Null', 16, 11)			
		END

		--Get user details
		SELECT UserName,
		       TourAgent
		FROM dbo.Users
		WHERE UserID = @UserID
			AND Valid = 'true';
						
		RETURN 0;		

	END TRY
	BEGIN CATCH
		--call error handler
		EXEC dbo.usp_ErrorHandler;
		RETURN -1;
	END CATCH
END
GO

--**************************************************************************
--Title: usp_AddUser
--Description: Adds a user, note not as Tour Agent							
--Parameter @UserName: User to be added
--Parameter @UserName: Password of user
--Returns: If there is another user with that name returns true
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_AddUser')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_AddUser;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_AddUser
(
	@UserName NVARCHAR(50),
	@Password NVARCHAR(50),
	@Email NVARCHAR(50),
	@TelNo NVARCHAR(20)
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		--validate input
		IF @UserName  IS NULL
		BEGIN
			RAISERROR(N'@UserName Parameter is Null', 16, 11)			
		END

		IF @UserName  = '' 
		BEGIN
			RAISERROR(N'@UserName Parameter is Empty', 16, 12)			
		END	
		
		IF @Password IS NULL
		BEGIN
			RAISERROR(N'@Password Parameter is Null', 16, 13)			
		END

		IF @Password  = '' 
		BEGIN
			RAISERROR(N'@Password Parameter is Empty', 16, 14)			
		END	

		IF @Email IS NULL
		BEGIN
			RAISERROR(N'@Email  Parameter is Null', 16, 15)			
		END

		IF @Email  = '' 
		BEGIN
			RAISERROR(N'@Email  Parameter is Empty', 16, 16)			
		END	

		IF @TelNo IS NULL
		BEGIN
			RAISERROR(N'@TelNo Parameter is Null', 16, 17)			
		END

		IF @TelNo  = '' 
		BEGIN
			RAISERROR(N'@TelNo Parameter is Empty', 16, 18)			
		END	
				
		--check for other user
		DECLARE @OtherUser BIT = 'false'
		IF EXISTS (SELECT 1 From Users WHERE UserName = @UserName)
		BEGIN
			  SET @OtherUser = 'true'
		END

		--Insert user
		IF @OtherUser = 'false'
		BEGIN
		BEGIN TRANSACTION
			INSERT INTO dbo.Users			
				   (UserName, [Password], TourAgent, Email, TelNo, Valid)
			VALUES (@UserName, @Password, 'false', @Email, @TelNo, 'true');			      
		COMMIT TRANSACTION
		END

		SELECT @OtherUser AS "OtherUser";
				
		RETURN 0;		

	END TRY
	BEGIN CATCH
		--roll back transaction if necessary
		IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
		--call error handler
		EXEC dbo.usp_ErrorHandler;
		RETURN -1;
	END CATCH
END
GO

--**************************************************************************
--Title: usp_GetTourAgents
--Description: Gets all valid TourAgents
--Returns: Returns UserID and UserName for all valid TourAgents         
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_GetTourAgents')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_GetTourAgents;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_GetTourAgents
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
						
		--return result		
		SELECT UserID   'TourAgentID', 
		       UserName 'TourAgentName'			  
		FROM Users		
		WHERE TourAgent = 'true'
		AND Valid = 'true';
				
		RETURN 0;

	END TRY
	BEGIN CATCH
		--call error handler
		EXEC dbo.usp_ErrorHandler;
		RETURN -1;
	END CATCH
END
GO

--**************************************************************************
--Title: usp_GetConversationsForUser
--Description: Gets all the Conversations for a particular user
--Parameter @UserID: User ID
--Returns: All ConversationIDs in user in ascending order         
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_GetConversationsForUser')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_GetConversationsForUser;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_GetConversationsForUser
(
	@UserID INT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		--validate input
		IF @UserID  IS NULL
		BEGIN
			RAISERROR(N'@UserID is Null', 16, 11)			
		END
				
		--return result		
		SELECT TourConversationID
		FROM TourConversation 
		INNER JOIN Users ON TourConversation.UserID = Users.UserID 
		WHERE TourConversation.UserID = @UserID
			OR TourAgentID = @UserID
		ORDER BY TourConversationID;
				
		RETURN 0;

	END TRY
	BEGIN CATCH
		--call error handler
		EXEC dbo.usp_ErrorHandler;
		RETURN -1;
	END CATCH
END
GO

--**************************************************************************
--Title: usp_AddConversation
--Description: Adds conversation using given details
--Parameter @UserID: The ID of instigating user	
--Parameter @TourAgentID: Tour Agent ID who conversation is with	
--Parameter @Title: Title of conversation
--Returns: Returns details of added conversation         
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_AddConversation')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_AddConversation;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_AddConversation
(
	@UserID INT,
	@TourAgentID INT,
	@Title NVARCHAR(100)
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;				

		--validate input
		IF @UserID  IS NULL
		BEGIN
			RAISERROR(N'@UserID is Null', 16, 11)			
		END
		IF @TourAgentID  IS NULL
		BEGIN
			RAISERROR(N'@TourAgentID is Null', 16, 12)			
		END
		IF @Title IS NULL
		BEGIN
			RAISERROR(N'@Title Parameter is Null', 16, 13)			
		END

		IF @Title  = '' 
		BEGIN
			RAISERROR(N'@Title Parameter is Empty', 16, 14)			
		END	

		--Insert conversation
		BEGIN TRANSACTION
			INSERT INTO dbo.TourConversation			
				   (UserID, TourAgentID, Title, AddedTime)
			VALUES (@UserID, @TourAgentID, @Title, CURRENT_TIMESTAMP);					
		
		COMMIT TRANSACTION

		--Return Conversation details
		SELECT TourConversationID,
			   UserID,
		       TourAgentID,
			   Title,
			   AddedTime
		FROM dbo.TourConversation
		WHERE TourConversationID = @@Identity;		
				
		RETURN 0;	

	END TRY
	BEGIN CATCH
		--roll back transaction if necessary
		IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
		--call error handler
		EXEC dbo.usp_ErrorHandler;
		RETURN -1;
	END CATCH
END
GO

--**************************************************************************
--Title: usp_GetConversation
--Description: Gets a conversation by Conversation ID
--Parameter @ConversationID: Conversation
--Returns: Conversation details         
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_GetConversation')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_GetConversation;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_GetConversation
(
	@ConversationID INT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		--validate input
		IF @ConversationID IS NULL
		BEGIN
			RAISERROR(N'@UserID is Null', 16, 11)			
		END

		--Get Conversation details
		SELECT TourConversationID,
			   UserID,
		       TourAgentID,
			   Title,
			   AddedTime
		FROM dbo.TourConversation
		WHERE TourConversationID = @ConversationID;
						
		RETURN 0;		

	END TRY
	BEGIN CATCH
		--call error handler
		EXEC dbo.usp_ErrorHandler;
		RETURN -1;
	END CATCH
END
GO

--**************************************************************************
--Title: usp_GetMessageIDs
--Description: Gets all Message Ids for a conversation
--Parameter @ConversationID: conversation ID
--Returns: Message IDs--         
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_GetMessageIDs')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_GetMessageIDs;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_GetMessageIDs
(
	@ConversationID INT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		
		--validate input
		IF @ConversationID  IS NULL
		BEGIN
			RAISERROR(N'@ConversationID is Null', 16, 11)			
		END

		--Get Message IDs
		SELECT TourMessageID		       
		FROM dbo.TourMessage
		WHERE TourConversationID = @ConversationID
		ORDER By TourMessageID;
						
		RETURN 0;		

	END TRY
	BEGIN CATCH
		--call error handler
		EXEC dbo.usp_ErrorHandler;
		RETURN -1;
	END CATCH
END
GO

--**************************************************************************
--Title: usp_GetMessage
--Description: Get message details		
--Parameter @MessageID: ID of message
--Returns:Message and message File details         
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_GetMessage')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_GetMessage;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_GetMessage
(
	@MessageID INT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		
		--validate input
		IF @MessageID   IS NULL
		BEGIN
			RAISERROR(N'@MessageID is Null', 16, 11)			
		END

		--Get Message 
		SELECT TourMessageID,
		       TourConversationID,
			   SenderID,
			   RecipientID,
			   MessageText,
			   MessageFileID,
			   MessageFileType,
			   AddedTime		       
		FROM TourMessage
			LEFT JOIN MessageFile ON TourMessage.TourMessageID = MessageFile.MessageID
		WHERE TourMessageID = @MessageID;
				
		RETURN 0;		

	END TRY
	BEGIN CATCH
		--call error handler
		EXEC dbo.usp_ErrorHandler;
		RETURN -1;
	END CATCH
END
GO

--**************************************************************************
--Title: usp_AddMessage
--Description: Adds Message
--Parameter @ConversationID: Conversation Id
--Parameter @SenderID: Sender ID (Who sent Message)
--Parameter @RecipientID: Recipient ID (Who recieved Message)
--Parameter @MessageText: Message Text	
--Parameter @MessageFile: Message File
--Parameter @MessageFileType: Message File Type (MIME)
--Returns: Message ID of added message         
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_AddMessage')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_AddMessage;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_AddMessage
(
	@ConversationID INT,
	@SenderID INT,
	@RecipientID INT,
	@MessageText NVARCHAR(1000),
	@MessageFile VARBINARY(MAX),
	@MessageFileType NVARCHAR(10)
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;			
		
		--validate input
		IF @ConversationID IS NULL
		BEGIN
			RAISERROR(N'@ConversationID is Null', 16, 11)			
		END
		IF @SenderID IS NULL
		BEGIN
			RAISERROR(N'@SenderID is Null', 16, 12)			
		END
		IF @RecipientID IS NULL
		BEGIN
			RAISERROR(N'@RecipientID is Null', 16, 13)			
		END

		DECLARE @MessageID INT;

		--Insert conversation
		BEGIN TRANSACTION
			INSERT INTO dbo.TourMessage			
				   (TourConversationID, SenderID, 
				   RecipientID, MessageText, AddedTime)
			VALUES (@ConversationID, @SenderID, 
			        @RecipientID, @MessageText, CURRENT_TIMESTAMP);	
			
			SET @MessageID = @@Identity;

			IF @MessageFile IS NOT NULL
			BEGIN
				INSERT INTO dbo.MessageFile			
					   (MessageID, MessageFileType, MessageFile)
				VALUES (@MessageID, @MessageFileType, @MessageFile);				
			END
		COMMIT TRANSACTION		

		--return conversation ID
		SELECT @MessageID AS 'MessageID';
				
		RETURN 0;		

	END TRY
	BEGIN CATCH
		--roll back transaction if necessary
		IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
		--call error handler
		EXEC dbo.usp_ErrorHandler;
		RETURN -1;
	END CATCH
END
GO
--**************************************************************************
--Title: usp_GetFile
--Description:Gets File blob and its details
--Parameter @FileID: The Files ID
--Returns: File Blob and details         
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_GetFile')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_GetFile;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_GetFile
(
	@FileID INT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		--validate input
		IF @FileID  IS NULL
		BEGIN
			RAISERROR(N'@FileID  is Null', 16, 11)			
		END

		--Get Message 
		SELECT MessageFileID,
		       MessageID,
			   MessageFileType,
			   MessageFile,
			   SenderID,
			   RecipientID			   		       
		FROM MessageFile
			INNER JOIN TourMessage ON TourMessage.TourMessageID = MessageFile.MessageID			
		WHERE MessageFileID = @FileID;
				
		RETURN 0;		

	END TRY
	BEGIN CATCH
		--call error handler
		EXEC dbo.usp_ErrorHandler;
		RETURN -1;
	END CATCH
END
GO


