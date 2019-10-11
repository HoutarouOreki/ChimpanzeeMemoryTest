using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChimpanzeeMemoryTest.Game
{
    public class Grid
    {
        private readonly GridContainer grid = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            FillMode = FillMode.Fit,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };

        private Cell[][] gridCells;

        private readonly List<List<Cell>> cells = new List<List<Cell>>();
        private IEnumerable<Cell> allCells => cells.SelectMany(l => l);

        private int amountOfSpacings => Size + 1;

        private float additionalSpacingSpace => Spacing * (Size + 1);

        private float totalSize => Size + additionalSpacingSpace;

        private float spacingSize => Spacing / totalSize;

        private int expectedNumber;

        private bool isCurrentRoundComplete => expectedNumber > AmountOfNumbers.Value;

        private DateTime timerStart;

        private GameStatistics gameStatistics = new GameStatistics();

        private readonly Bindable<GridState> state = new Bindable<GridState>(GridState.NotReady);
        public IBindable<GridState> State => state;

        public IGameStatistics GameStatistics => gameStatistics;

        public BindableInt AmountOfNumbers { get; } = new BindableInt(5)
        {
            MinValue = 3,
            MaxValue = 50,
        };

        public BindableFloat TimeToMemorize { get; } = new BindableFloat(0)
        {
            MinValue = 0,
            MaxValue = 7,
            Precision = 0.1f,
        };

        public Bindable<BoxVisibility> VisibleBoxes { get; } = new Bindable<BoxVisibility>(BoxVisibility.WithNumbers);

        public float CellSize => 1 / totalSize;

        public BindableInt SizeBindable { get; } = new BindableInt(5)
        {
            MaxValue = 20,
            MinValue = 3,
            Precision = 1
        };

        public int Size => SizeBindable.Value;

        /// <summary>
        /// The size of space between cells relative to the size of a cell.
        /// </summary>
        public float Spacing = 0.1f;

        public BindableInt RoundsAmount { get; } = new BindableInt(1)
        {
            MinValue = 1,
            MaxValue = 20,
        };

        public int CurrentRound { get; private set; }

        public Drawable Drawable => grid;

        public DateTime memorizationStart;

        public Grid()
        {
            SizeBindable.BindValueChanged(OnSizeChanged, true);
            SizeBindable.BindValueChanged(v => OnSettingsChanged(), true);
            AmountOfNumbers.BindValueChanged(v => OnSettingsChanged());
            VisibleBoxes.BindValueChanged(v => OnSettingsChanged());
            // RoundsAmount.BindValueChanged(v => OnSettingsChanged()); // doesn't change the preview at all
            State.BindValueChanged(OnStateChanged, true);
            Drawable.OnUpdate += OnDrawableUpdate;
        }

        private void OnDrawableUpdate(Drawable obj)
        {
            if (TimeToMemorize.Value > 0 && State.Value == GridState.GeneratedAndWaiting && (DateTime.UtcNow - memorizationStart).TotalSeconds >= TimeToMemorize.Value)
                Start();
        }

        private void OnStateChanged(ValueChangedEvent<GridState> obj)
        {
            if (State.Value == GridState.GeneratedAndWaiting && TimeToMemorize.Value > 0)
                memorizationStart = DateTime.UtcNow;
        }

        private void OnSettingsChanged()
        {
            GenerateLayout();
            SetNumbers(GenerateRandomCoordinates());
            ShowFull();
        }

        private void OnSizeChanged(ValueChangedEvent<int> vc)
        {
            AmountOfNumbers.MaxValue = vc.NewValue * vc.NewValue;
            AmountOfNumbers.Value = Math.Min(AmountOfNumbers.Value, AmountOfNumbers.MaxValue);
        }

        private Cell GetCell(Vector2I coordinates) => gridCells[(coordinates.Y * 2) - 1][(coordinates.X * 2) - 1];

        public void GenerateLayout()
        {
            var dimensions = new Dimension[Size + amountOfSpacings];
            gridCells = new Cell[Size + amountOfSpacings][];
            cells.Clear();
            for (var i = 0; i < Size + amountOfSpacings; i++)
            {
                if (i % 2 == 0) // if row or cell of spacing
                    dimensions[i] = new Dimension(GridSizeMode.Relative, spacingSize);
                else
                {
                    dimensions[i] = new Dimension(GridSizeMode.Relative, CellSize);
                    var gridRow = new Cell[Size + amountOfSpacings];
                    var cellRow = new List<Cell>();
                    cells.Add(cellRow);
                    for (var j = 0; j < Size + amountOfSpacings; j++)
                    {
                        if (j % 2 != 0)
                        {
                            var cell = new Cell();
                            cell.Clicked += OnCellClicked;
                            gridRow[j] = cell;
                            cellRow.Add(cell);
                        }
                    }
                    gridCells[i] = gridRow;
                }
            }
            grid.RowDimensions = dimensions;
            grid.ColumnDimensions = dimensions;
            grid.Content = gridCells;
        }

        private void OnCellClicked(Cell cell)
        {
            if (VisibleBoxes.Value == BoxVisibility.WithNumbers && !cell.Number.HasValue)
                return;
            if (!(State.Value == GridState.Playing || (State.Value == GridState.GeneratedAndWaiting && cell.Number == 1)))
                return;
            if (cell.Number < expectedNumber)
                return;
            if (state.Value == GridState.GeneratedAndWaiting)
                Start();
            if (cell.Number == expectedNumber)
            {
                cell.ShowNumber();
                cell.ShowBackground();
                expectedNumber++;
            }
            else
                OnFail();
            if (isCurrentRoundComplete)
                OnWin();
        }

        private void OnWin()
        {
            gameStatistics.actionTimes.Add((float)(DateTime.UtcNow - timerStart).TotalSeconds);
            gameStatistics.RoundsCompleted++;
            state.Value = GridState.RoundCompleted;
        }

        private void OnFail()
        {
            gameStatistics.memorizationTimes.Remove(gameStatistics.memorizationTimes.Last());
            gameStatistics.RoundsFailed++;
            state.Value = GridState.RoundFailed;
            ShowFull();
        }

        private void SetNumbers(IList<Vector2I> placesArray)
        {
            var usedCoordinates = new List<Vector2I>();
            var currentNumber = 0;

            if (placesArray.Count > Size * Size)
                throw new Exception($"Too many numbers ({placesArray.Count}) for the {Size}x{Size} grid");

            foreach (var place in placesArray)
            {
                currentNumber++;
                if (usedCoordinates.Contains(place))
                    throw new ArgumentException("This coordinate is already used");
                if (place.X <= 0 || place.Y <= 0)
                    throw new ArgumentOutOfRangeException("The coordinates must be above 0");

                GetCell(place).Number = currentNumber;
            }
            timerStart = DateTime.UtcNow;
        }

        private void ShowFull()
        {
            foreach (var cell in allCells)
            {
                if (VisibleBoxes.Value == BoxVisibility.All || cell.Number.HasValue)
                {
                    cell.ShowBackground();
                    cell.ShowNumber();
                }
            }
        }

        public void Retry()
        {
            state.Value = GridState.NotReady;
            Proceed();
        }

        public void Start()
        {
            foreach (var cell in allCells)
            {
                if ((VisibleBoxes.Value == BoxVisibility.WithNumbers && !cell.Number.HasValue)
                    || VisibleBoxes.Value == BoxVisibility.None)
                    cell.HideBackground();
                cell.HideNumber();
            }
            CurrentRound++;
            state.Value = GridState.Playing;
            gameStatistics.memorizationTimes.Add((float)(DateTime.UtcNow - timerStart).TotalSeconds);
            timerStart = DateTime.UtcNow;
        }

        public void NextRound()
        {
            if (CurrentRound == RoundsAmount.Value)
            {
                GameFinished();
                return;
            }
            GenerateLayout();
            SetNumbers(GenerateRandomCoordinates());
            ShowFull();
            state.Value = GridState.GeneratedAndWaiting;
            expectedNumber = 1;
        }

        private void GameFinished() => state.Value = GridState.GameFinished;

        public void Proceed()
        {
            if (State.Value == GridState.NotReady)
            {
                if (AmountOfNumbers.Value > Size * Size)
                    throw new Exception($"Amount of numbers ({AmountOfNumbers}) is impossible to fit into the grid of size {Size}x{Size}");
                GenerateLayout();
                SetNumbers(GenerateRandomCoordinates());
                ShowFull();
                state.Value = GridState.GeneratedAndWaiting;
                expectedNumber = 1;
                CurrentRound = 0;
                InitializeStatistics();
            }
            else if (State.Value == GridState.GeneratedAndWaiting)
                Start();
            else if (State.Value == GridState.GameFinished)
            {
                OnSettingsChanged();
                state.Value = GridState.NotReady;
            }
        }

        private void InitializeStatistics() => gameStatistics = new GameStatistics
        {
            Settings = new SettingsStatistics
            {
                BoardSize = Size,
                BoxVisibility = VisibleBoxes.Value,
                NumbersAmount = AmountOfNumbers.Value
            }
        };

        public IList<Vector2I> GenerateRandomCoordinates()
        {
            var coordinates = new List<Vector2I>();
            for (var i = 0; i < AmountOfNumbers.Value; i++)
            {
                Vector2I coordinate;
                do
                    coordinate = new Vector2I(RNG.Next(Size), RNG.Next(Size)) + Vector2I.One;
                while (coordinates.Contains(coordinate));
                coordinates.Add(coordinate);
            }
            return coordinates;
        }
    }

    public enum BoxVisibility
    {
        None = 0,
        WithNumbers = 1,
        All = 2,
    }

    public enum GridState
    {
        NotReady = 0,
        GeneratedAndWaiting = 1,
        Playing = 2,
        RoundCompleted = 3,
        RoundFailed = 4,
        GameFinished = 5,
    }
}
