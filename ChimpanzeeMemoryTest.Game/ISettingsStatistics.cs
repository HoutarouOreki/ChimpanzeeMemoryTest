namespace ChimpanzeeMemoryTest.Game
{
    public interface ISettingsStatistics
    {
        int BoardSize { get; }
        int NumbersAmount { get; }
        BoxVisibility BoxVisibility { get; }
    }
}
