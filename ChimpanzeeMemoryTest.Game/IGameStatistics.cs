using System.Collections.Generic;

namespace ChimpanzeeMemoryTest.Game
{
    public interface IGameStatistics
    {
        int RoundsCompleted { get; }
        int RoundsFailed { get; }

        IList<float> MemorizationTimes { get; }
        IList<float> ActionTimes { get; }

        ISettingsStatistics Settings { get; }
    }
}
