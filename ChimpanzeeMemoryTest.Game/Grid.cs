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


        private bool isComplete => expectedNumber > AmountOfNumbers;

        private readonly Bindable<GridState> state = new Bindable<GridState>(GridState.NotReady);
        public IBindable<GridState> State => state;

        public int AmountOfNumbers { get; set; } = 5;

        public VisibleBoxes VisibleBoxes { get; set; } = VisibleBoxes.WithNumbers;

        public float CellSize => 1 / totalSize;

        public int Size { get; set; } = 5;

        /// <summary>
        /// The size of space between cells relative to the size of a cell.
        /// </summary>
        public float Spacing = 0.1f;

        public Drawable Drawable => grid;

        private Cell GetCell(Vector2I coordinates) => gridCells[(coordinates.Y * 2) - 1][(coordinates.X * 2) - 1];

        //private Cell GetCell(int row, int column) => gridCells[(row * 2) - 1][(column * 2) - 1];

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
            if (VisibleBoxes == VisibleBoxes.WithNumbers && !cell.Number.HasValue)
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
                expectedNumber++;
            }
            else
                OnFail();
            if (isComplete)
                OnWin();
        }

        private void OnWin() => state.Value = GridState.Completed;

        private void OnFail()
        {
            state.Value = GridState.Failed;
            ShowFull();
        }

        private void SetNumbers(IList<Vector2I> placesArray, int size)
        {
            var usedCoordinates = new List<Vector2I>();
            var currentNumber = 0;

            Size = size;

            foreach (var place in placesArray)
            {
                currentNumber++;
                if (usedCoordinates.Contains(place))
                    throw new ArgumentException("This coordinate is already used");
                if (place.X <= 0 || place.Y <= 0)
                    throw new ArgumentOutOfRangeException("The coordinates must be above 0");

                GetCell(place).Number = currentNumber;
            }
        }

        private void ShowFull()
        {
            foreach (var cell in allCells)
            {
                if (VisibleBoxes == VisibleBoxes.All || cell.Number.HasValue)
                {
                    cell.ShowBackground();
                    cell.ShowNumber();
                }
            }
        }

        private void Start()
        {
            foreach (var cell in allCells)
            {
                if (VisibleBoxes == VisibleBoxes.WithNumbers && !cell.Number.HasValue)
                    cell.HideBackground();
                cell.HideNumber();
            }
            state.Value = GridState.Playing;
        }

        public void Proceed()
        {
            if (State.Value == GridState.NotReady)
            {
                var coordinates = new List<Vector2I>();
                if (AmountOfNumbers > Size * Size)
                    throw new Exception($"Amount of numbers ({AmountOfNumbers}) is impossible to fit into the grid of size {Size}x{Size}");
                for (var i = 0; i < AmountOfNumbers; i++)
                {
                    Vector2I coordinate;
                    do
                        coordinate = new Vector2I(RNG.Next(1, 6), RNG.Next(1, 6));
                    while (coordinates.Contains(coordinate));
                    coordinates.Add(coordinate);
                }
                GenerateLayout();
                SetNumbers(coordinates, Size);
                ShowFull();
                state.Value = GridState.GeneratedAndWaiting;
                expectedNumber = 1;
            }
            else if (State.Value == GridState.GeneratedAndWaiting)
                Start();
            else if (State.Value == GridState.Completed || State.Value == GridState.Failed)
            {
                GenerateLayout();
                state.Value = GridState.NotReady;
            }
        }
    }

    public enum VisibleBoxes
    {
        WithNumbers = 0,
        All = 1,
    }

    public enum GridState
    {
        NotReady = 0,
        GeneratedAndWaiting = 1,
        Playing = 2,
        Completed = 3,
        Failed = 4,
    }
}
