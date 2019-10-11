namespace ChimpanzeeMemoryTest.Game
{
    public class SettingsStatistics : ISettingsStatistics
    {
        public int BoardSize { get; set; }

        public int NumbersAmount { get; set; }

        public BoxVisibility BoxVisibility { get; set; }
    }
}
