USE [SuperSportDataEngine_SystemSportData]
GO

BEGIN TRY
	BEGIN TRAN
		PRINT 'Attempting to clean-up System Data...'

		DELETE FROM [SuperSportDataEngine_SystemSportData].[dbo].[__MigrationHistory]

		DELETE FROM [SuperSportDataEngine_SystemSportData].[dbo].[LegacyAccessItems]

		DELETE FROM [SuperSportDataEngine_SystemSportData].[dbo].[LegacyMethodAccesses]

		DELETE FROM [SuperSportDataEngine_SystemSportData].[dbo].[LegacyZoneSites]

		DELETE FROM [SuperSportDataEngine_SystemSportData].[dbo].[LegacyZoneSites]

		DELETE FROM [SuperSportDataEngine_SystemSportData].[dbo].[SchedulerTrackingRugbyFixtures]

		DELETE FROM [SuperSportDataEngine_SystemSportData].[dbo].[SchedulerTrackingRugbySeasons]

		DELETE FROM [SuperSportDataEngine_SystemSportData].[dbo].[SchedulerTrackingRugbyTournaments]
	ROLLBACK
	PRINT 'Completed clean-up for System Data!'
END TRY
BEGIN CATCH
PRINT 'ERROR: Unable to clean-up System Data!!!!!'
	ROLLBACK
END CATCH

GO