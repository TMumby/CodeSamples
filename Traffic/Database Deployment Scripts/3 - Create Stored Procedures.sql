USE VehicleData;
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
--Title: usp_GetVehicleData
--Description: 
--Parameter @Registration: 		
--Returns:   
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_GetVehicleData')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_GetVehicleData;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_GetVehicleData
(
	@Registration NVARCHAR(15) 
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		--validate input
		IF @Registration  IS NULL
		BEGIN
			RAISERROR(N'@Registration Parameter is Null', 16, 11)			
		END
		IF @Registration = '' 
		BEGIN
			RAISERROR(N'@Registration is Empty', 16, 12)			
		END			

		--return result		
		SELECT VehicleID, 
		       Registration,
			   Pollution	
		FROM VehicleData
		WHERE Registration = @Registration
						
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
--Title: usp_InsertJourneyLog
--Description: 
--Parameter 	
--Returns:   
--**************************************************************************
--drop if already exists
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE object_id = OBJECT_ID(N'usp_InsertJourneyLog')
           AND type IN ( N'P', N'PC' ))
BEGIN
	DROP PROCEDURE dbo.usp_InsertJourneyLog;
END
GO
--create procedure	
CREATE PROCEDURE dbo.usp_InsertJourneyLog
(
	@ID INT,
	@RouteNo INT,
	@SectionNo INT,
	@Direction NVARCHAR(10),
	@Pollution FLOAT	
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		--validate input
		IF @ID   IS NULL
		BEGIN
			RAISERROR(N'@ID Parameter is Null', 16, 11)			
		END
		IF @RouteNo   IS NULL
		BEGIN
			RAISERROR(N'@RouteNo Parameter is Null', 16, 12)			
		END
		IF @SectionNo  IS NULL
		BEGIN
			RAISERROR(N'@SectionNo Parameter is Null', 16, 13)			
		END
		
		IF @Direction   IS NULL
		BEGIN
			RAISERROR(N'@Direction Parameter is Null', 16, 14)			
		END
		IF @Direction  = '' 
		BEGIN
			RAISERROR(N'@Direction is Empty', 16, 15)			
		END
		IF @Pollution   IS NULL
		BEGIN
			RAISERROR(N'@Pollution Parameter is Null', 16, 16)			
		END	

		--Insert Record
		INSERT INTO VehicleLog
			(VehicleID, RouteNo, SectionNo, Direction, Pollution, SensorTime)
		VALUES (@ID, @RouteNo, @SectionNo, @Direction, @Pollution, CURRENT_TIMESTAMP);
								
		RETURN 0;

	END TRY
	BEGIN CATCH
		--call error handler
		EXEC dbo.usp_ErrorHandler;		
		RETURN -1;
	END CATCH
END
GO
