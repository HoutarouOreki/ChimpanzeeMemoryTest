namespace ChimpanzeeMemoryTest.Game
{
    internal class Program
    {
        private static void Main()
        {
            using (var host = osu.Framework.Host.GetSuitableHost(@"Chimpanzee Memory Test"))
            {
                host.Run(new ChimpanzeeMemoryTestGame());
            }
        }
    }
}
