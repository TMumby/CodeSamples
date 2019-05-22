USE TouristMessenger;
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

INSERT INTO dbo.Users			
		(UserName, [Password], TourAgent, Valid, Email, TelNo)
VALUES ('Mr Happy', 'pass1234', 'true', 'true', 'MR_HAPPY@TourTalk.COM', '0115 9664045'),
       ('Mr Small', 'pass1234', 'true', 'true', 'MR_SMALL@TourTalk.COM', '01636 615750'),
	   ('Little Miss Sunshine', 'pass1234', 'true', 'true', 'SUNSHINE@TourTalk.COM', '0115 9663212'),
	   ('Timothy', 'pass1234', 'false', 'true', 'Tim@YAHOO.COM', '07860 232323');