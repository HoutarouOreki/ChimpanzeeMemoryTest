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
        private int size;

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

        public bool IsReady { get; private set; }

        public bool IsStarted { get; private set; }

        public float CellSize => 1 / totalSize;

        public int Size
        {
            get => size;
            private set
            {
                size = value;
                UpdateLayout();
            }
        }

        /// <summary>
        /// The size of space between cells relative to the size of a cell.
        /// </summary>
        public float Spacing = 0.1f;

        public Drawable Drawable => grid;

        private Cell GetCell(Vector2I coordinates) => gridCells[(coordinates.Y * 2) - 1][(coordinates.X * 2) - 1];

        private Cell GetCell(int row, int column) => gridCells[(row * 2) - 1][(column * 2) - 1];

        public void UpdateLayout()
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
            if (!IsStarted)
                Start(true);
            if (cell.Number == expectedNumber)
            {
                cell.Show(false, true);
                expectedNumber++;
            }
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

        private void ShowFull(bool onlyWithNumbers)
        {
            foreach (var cell in allCells)
                cell.Show(onlyWithNumbers, true);
        }

        private void Start(bool onlyLeaveBackgroundIfNumber)
        {
            IsStarted = true;
            expectedNumber = 1;
            foreach (var cell in allCells)
                cell.Hide(onlyLeaveBackgroundIfNumber);
        }

        public void Proceed()
        {
            if (!IsReady)
            {
                var coordinates = new List<Vector2I>();
                for (var i = 0; i < 4; i++)
                {
                    Vector2I coordinate;
                    do
                        coordinate = new Vector2I(RNG.Next(1, 6), RNG.Next(1, 6));
                    while (coordinates.Contains(coordinate));
                    coordinates.Add(coordinate);
                }
                SetNumbers(coordinates, 5);
                ShowFull(true);
                IsReady = true;
            }
            else if (!IsStarted)
                Start(true);
        }
    }
}
