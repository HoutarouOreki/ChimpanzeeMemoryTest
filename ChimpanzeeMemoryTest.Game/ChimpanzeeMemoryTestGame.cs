using ChimpanzeeMemoryTest.Game.Screens;
using osu.Framework.Graphics.Primitives;
using osu.Framework.MathUtils;
using osu.Framework.Screens;
using System.Collections.Generic;

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
