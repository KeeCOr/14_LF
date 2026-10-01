using UnityEngine;

namespace SlotDefense
{
    public static class RecordSystem
    {
        private const string KeyWins   = "record_wins";
        private const string KeyLosses = "record_losses";
        private const string KeyStreak = "record_streak";

        public static int Wins    => PlayerPrefs.GetInt(KeyWins,   0);
        public static int Losses  => PlayerPrefs.GetInt(KeyLosses, 0);
        public static int Streak  => PlayerPrefs.GetInt(KeyStreak, 0);

        public static void RecordWin()
        {
            PlayerPrefs.SetInt(KeyWins,   Wins + 1);
            PlayerPrefs.SetInt(KeyStreak, Streak + 1);
            PlayerPrefs.Save();
        }

        public static void RecordLoss()
        {
            PlayerPrefs.SetInt(KeyLosses, Losses + 1);
            PlayerPrefs.SetInt(KeyStreak, 0);
            PlayerPrefs.Save();
        }

        public static void RecordDraw()
        {
            PlayerPrefs.Save();
        }

        public static string Summary() =>
            $"{Wins}승 {Losses}패  연승 {Streak}";
    }
}
