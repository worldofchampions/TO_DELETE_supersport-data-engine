namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class LeaderboardItemModel
    {
        public string PositionText { get; set; }

        public int Position { get; set; }

        public int PlayerID { get; set; }

        public string PlayerName { get; set; }

        public string PlayerSurname { get; set; }

        public string PlayerFullName { get; set; }

        public string PlayerKnownAs { get; set; }

        public string PlayerCountry { get; set; }

        public bool PlayerIsSouthAfrican { get; set; }

        public string Score { get; set; }

        public int Hole { get; set; }

        public string Today { get; set; }

        public string Rnd1 { get; set; }

        public string Rnd2 { get; set; }

        public string Rnd3 { get; set; }

        public string Rnd4 { get; set; }

        public string Rnd5 { get; set; }

        public string Rnd6 { get; set; }

        public int TotalStrokes { get; set; }

        public bool Started { get; set; }

        public int CurRound { get; set; }
    }
}