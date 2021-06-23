using System.Collections.Generic;

namespace ChimpanzeeMemoryTest.Game
{
    public class GameStatistics
    {
        public int RoundsCompleted { get; set; }

        public int RoundsFailed { get; set; }

        public readonly List<float> MemorizationTimes = new();

        public readonly List<float> ActionTimes = new();

        public SettingsStatistics Settings { get; set; } = new();
    }
}
