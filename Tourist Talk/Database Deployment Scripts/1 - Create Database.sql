SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


IF NOT EXISTS (SELECT name FROM master.sys.databases WHERE name = N'TouristMessenge')
BEGIN
	CREATE DATABASE TouristMessenger
	COLLATE Latin1_General_CS_AS;
END
GO