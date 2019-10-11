using System.Collections.Generic;

namespace ChimpanzeeMemoryTest.Game
{
    public class GameStatistics : IGameStatistics
    {
        public int RoundsCompleted { get; set; }

        public int RoundsFailed { get; set; }

        public IList<float> MemorizationTimes => memorizationTimes;

        public readonly List<float> memorizationTimes = new List<float>();

        public IList<float> ActionTimes => actionTimes;

        public readonly List<float> actionTimes = new List<float>();

        public ISettingsStatistics Settings { get; set; }
    }
}
