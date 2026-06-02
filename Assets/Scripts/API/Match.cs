using System;

namespace WarOfTanks.API
{
    [System.Serializable]
    class Match
    {
        public string playerA;
        public string playerB;

        public string winner;

        public string scoreTeamA;
        public string scoreTeamB;

        public int durationSeconds;

        public DateTime playedAt;
    }
}