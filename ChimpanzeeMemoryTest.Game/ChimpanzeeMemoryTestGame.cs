using ChimpanzeeMemoryTest.Game.Screens;
using osu.Framework.Screens;

namespace ChimpanzeeMemoryTest.Game
{
    public class ChimpanzeeMemoryTestGame : osu.Framework.Game
    {
        private readonly ScreenStack screenStack;

        public ChimpanzeeMemoryTestGame()
        {
            Add(screenStack = new ScreenStack { RelativeSizeAxes = osu.Framework.Graphics.Axes.Both });
            screenStack.Push(new PlayScreen());
        }
    }
}
