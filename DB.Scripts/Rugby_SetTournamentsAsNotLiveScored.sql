USE [SuperSportDataEngine_PublicSportData]

GO

BEGIN TRY
	BEGIN TRAN
		PRINT 'Attempting to mark Tier 6 tournaments as Live Scored...'
		UPDATE [SuperSportDataEngine_PublicSportData].[dbo].[RugbyTournaments]
		SET IsLiveScored = 0
		WHERE ProviderTournamentId IN
				(
					810  -- international
				)
	COMMIT
	PRINT 'Tier 6 tournaments are now marked as Live Scored.'
END TRY
BEGIN CATCH
    PRINT 'Unable to update tournaments IsLiveScored flag.'
    IF(@@TRANCOUNT > 0)
        ROLLBACK TRAN
END CATCH

GO