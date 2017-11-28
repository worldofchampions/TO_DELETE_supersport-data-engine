USE [SuperSportDataEngine_PublicSportData]

GO

BEGIN TRY
	BEGIN TRAN
		PRINT 'Attempting to mark Tournaments that Has Logs...'
		UPDATE [SuperSportDataEngine_PublicSportData].[dbo].[RugbyTournaments]
		SET HasLogs = 1
		WHERE ProviderTournamentId NOT IN
				(
					810  -- international
				)
	COMMIT
	PRINT 'Tournaments that have logs have been marked.'
END TRY
BEGIN CATCH
    PRINT 'Unable to update tournaments that have logs.'
    IF(@@TRANCOUNT > 0)
        ROLLBACK TRAN
END CATCH

GO