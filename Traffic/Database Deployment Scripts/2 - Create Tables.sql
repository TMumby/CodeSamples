USE VehicleData;
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO


IF NOT EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'VehicleData')  AND type = N'U')
BEGIN
	CREATE TABLE dbo.VehicleData
	(
		VehicleID INT IDENTITY(1,1) NOT NULL,
		Registration NVARCHAR(15) NOT NULL,
		Pollution FLOAT NOT NULL
		CONSTRAINT PK_VehicleData PRIMARY KEY(VehicleID),
		CONSTRAINT UC_VehicleData_Registration UNIQUE(Registration)		
	);
END
GO



IF NOT EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'VehicleLog')  AND type = N'U')
BEGIN
	CREATE TABLE dbo.VehicleLog
	(
		JourneyID INT IDENTITY(1,1) NOT NULL,
		VehicleID INT NOT NULL,
		RouteNo INT NOT NULL,
		SectionNo INT NOT NULL,
		Direction NVARCHAR(10) NOT NULL,
		Pollution FLOAT NOT NULL,
		SensorTime DATETIME2 NOT NULL
		CONSTRAINT PK_VehicleLog PRIMARY KEY(JourneyID)
		CONSTRAINT FK_VehicleLog_VehicleID FOREIGN KEY (VehicleID)
		REFERENCES dbo.VehicleData (VehicleID)
				
	);

END
GO





