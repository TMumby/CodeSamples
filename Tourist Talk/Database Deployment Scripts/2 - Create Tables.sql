USE TouristMessenger;
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

--Users - Stores user login data and if they are a Tour Agent
IF NOT EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'Users')  AND type = N'U')
BEGIN
	CREATE TABLE dbo.Users
	(
		UserID INT IDENTITY(1,1) NOT NULL,
		UserName NVARCHAR(50) NOT NULL,
		Password NVARCHAR(50) NOT NULL,
		TourAgent BIT NOT NULL,
		Email NVARCHAR(50) NOT NULL,
		TelNo NVARCHAR(20) NOT NULL,
		Valid BIT NOT NULL
		CONSTRAINT PK_User PRIMARY KEY(UserID),
		CONSTRAINT UC_User_UserName UNIQUE (UserName)		
	);
END
GO

--TourConversation - Holds details of all conversations (a conversation conceptionaly is a collection of messages)
IF NOT EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'TourConversation')  AND type = N'U')
BEGIN
	CREATE TABLE dbo.TourConversation
	(
		TourConversationID INT IDENTITY(1,1) NOT NULL,
		UserID INT NOT NULL,
		TourAgentID INT NOT NULL,
		Title NVARCHAR(100) NOT NULL,
		AddedTime DATETIME2		
		CONSTRAINT PK_TourConversation PRIMARY KEY(TourConversationID),
		CONSTRAINT FK_TourConversation_UserID FOREIGN KEY (UserID)
		REFERENCES dbo.Users (UserID),
		CONSTRAINT FK_TourConversation_TourAgentID FOREIGN KEY (TourAgentID)
		REFERENCES dbo.Users (UserID),
	);
END
GO

--TourMessage - Holds Message details
IF NOT EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'TourMessage')  AND type = N'U')
BEGIN
	CREATE TABLE dbo.TourMessage
	(
		TourMessageID INT IDENTITY(1,1) NOT NULL,
		TourConversationID INT NOT NULL,
		SenderID INT NOT NULL,
		RecipientID INT NOT NULL,
		MessageText NVARCHAR (1000),
		AddedTime DATETIME2 		
		CONSTRAINT PK_TourMessage PRIMARY KEY(TourMessageID),
		CONSTRAINT FK_TourMessage_TourConversationID FOREIGN KEY (TourConversationID)
		REFERENCES dbo.TourConversation (TourConversationID)
	);
END
GO

--MessageFile - Holds file data and details for a particular message
IF NOT EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'MessageFile')  AND type = N'U')
BEGIN
	CREATE TABLE dbo.MessageFile
	(
		MessageFileID INT IDENTITY(1,1) NOT NULL,
		MessageID INT NOT NULL,
		MessageFileType NVARCHAR(10),
		MessageFile VARBINARY(MAX)			
		CONSTRAINT PK_MessageFile PRIMARY KEY(MessageFileID),
		CONSTRAINT FK_MessageFile_MessageID FOREIGN KEY (MessageID)
		REFERENCES dbo.TourMessage (TourMessageID)
	);
END
GO



