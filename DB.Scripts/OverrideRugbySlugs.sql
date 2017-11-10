USE [SuperSportDataEngine_PublicSportData]

GO

BEGIN TRY
	BEGIN TRAN
		PRINT 'Attempting to update a few tournaments slugs...'
		UPDATE RugbyTournaments SET Slug = 'british-lions'		WHERE ProviderTournamentId = 761
		UPDATE RugbyTournaments SET Slug = 'champions-cup'		WHERE ProviderTournamentId = 291
		UPDATE RugbyTournaments SET Slug = 'craven-week'		WHERE ProviderTournamentId = 129
		UPDATE RugbyTournaments SET Slug = 'currie-cup'			WHERE ProviderTournamentId = 121
		UPDATE RugbyTournaments SET Slug = 'international-rugby'WHERE ProviderTournamentId = 810
		UPDATE RugbyTournaments SET Slug = 'pro14'				WHERE ProviderTournamentId = 293
		UPDATE RugbyTournaments SET Slug = 'pro-d2'				WHERE ProviderTournamentId = 242
		UPDATE RugbyTournaments SET Slug = 'rugby-championship' WHERE ProviderTournamentId = 117
		UPDATE RugbyTournaments SET Slug = 'sevens'				WHERE ProviderTournamentId = 831
		UPDATE RugbyTournaments SET Slug = 'six-nations'		WHERE ProviderTournamentId = 301
		UPDATE RugbyTournaments SET Slug = 'super-rugby'		WHERE ProviderTournamentId = 181
		UPDATE RugbyTournaments SET Slug = 'top14'				WHERE ProviderTournamentId = 241
	COMMIT
	PRINT 'Tournaments slugs updated.'
END TRY
BEGIN CATCH
    PRINT 'Unable to update tournaments slugs.'
    IF(@@TRANCOUNT > 0)
        ROLLBACK TRAN
END CATCH

GO