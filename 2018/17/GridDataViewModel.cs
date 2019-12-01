using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace _17
{
    public class GridDataViewModel : INotifyPropertyChanged
    {
        public GridDataViewModel(int wellLocationX, GridEntryViewModel[][] grid)
        {
            this.WellLocationX = wellLocationX;
            this.ViewBoxXSize = Math.Min(wellLocationX + 100, grid[0].Length) - Math.Max(0, wellLocationX - 100);
            this.ViewBoxYSize = Math.Min(100, grid.Length);
            this.ViewBox =
                Enumerable.Range(0, Math.Min(100, grid.Length))
                    .SelectMany(y =>
                        Enumerable.Range(Math.Max(0, wellLocationX - 100), ViewBoxXSize)
                            .Select(x =>
                            {
                                var cell = grid[y][x];
                                cell.ViewBoxX = x;
                                cell.ViewBoxY = y;
                                return cell;
                            })
                    ).ToArray();
            this.Cells = grid;
            this.YLength = grid.Length;
            this.XLength = grid.Length > 0 ? grid[0].Length : 0;
        }

        public int WellLocationX { get; }
        public GridEntryViewModel[] ViewBox { get; }
        public int ViewBoxXSize { get; }
        public int ViewBoxYSize { get; }
        public GridEntryViewModel[][] Cells { get; }
        public int YLength { get; }
        public int XLength { get; }
        private bool _hasWellStartedRunning = false;
        public int WaterCellCount => _waterCells.Count;
        private List<GridEntryViewModel> _cellsWithRunningWater = new List<GridEntryViewModel>();
        private List<GridEntryViewModel> _waterCells = new List<GridEntryViewModel>();

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;

        private void MakeWaterFlow(GridEntryViewModel cell)
        {
            cell.Type = GridEntryType.RunningWater;
            _waterCells.Add(cell);
            _cellsWithRunningWater.Add(cell);
        }

        private void MakeWaterStill(GridEntryViewModel cell)
        {
            cell.Type = GridEntryType.StillWater;
            _cellsWithRunningWater.Remove(cell);
        }

        public void LetWaterTakeOneStep()
        {
            if (!_hasWellStartedRunning)
            {
                MakeWaterFlow(this.Cells[0][WellLocationX]);
                _hasWellStartedRunning = true;
                OnPropertyChanged(nameof(WaterCellCount));
                return;
            }
            var flowingWaterEntries = _cellsWithRunningWater.ToArray();
            var waterThatDidNotFlowDown = new List<GridEntryViewModel>();
            foreach (var entry in flowingWaterEntries)
            {
                if (!CouldWaterFlowDown(entry))
                {
                    waterThatDidNotFlowDown.Add(entry);
                }
            }
            LetWaterFlowHorizontally(waterThatDidNotFlowDown);
            OnPropertyChanged(nameof(WaterCellCount));
        }

        private void LetWaterFlowHorizontally(List<GridEntryViewModel> waterThatDidNotFlowDown)
        {
            //This might/will find duplicate bodies of water, but that doesn't matter
            var nextWaterBodyEdges =
                waterThatDidNotFlowDown
                    .Select(entry =>
                    {
                        //This will be left as null if we come to the edge or any along the path can flow down
                        GridEntryViewModel nextOnLeftSide = null;
                        for (var x = entry.X; x >= 0; x--)
                        {
                            //If it's not running water, it should be sand or clay, so this is what we're looking for.
                            if (Cells[entry.Y][x].Type != GridEntryType.RunningWater)
                            {
                                nextOnLeftSide = Cells[entry.Y][x];
                                break;
                            }
                            GridEntryViewModel cellRightBelow = Cells[entry.Y + 1][x];
                            if (cellRightBelow.Type == GridEntryType.RunningWater || cellRightBelow.Type == GridEntryType.Sand)
                            {
                                break;
                            }
                        }
                        //This will be left as null if we come to the edge or any along the path can flow down
                        GridEntryViewModel nextOnRightSide = null;
                        for (var x = entry.X; x < XLength; x++)
                        {
                            //If it's not running water, it should be sand or clay, so this is what we're looking for.
                            if (Cells[entry.Y][x].Type != GridEntryType.RunningWater)
                            {
                                nextOnRightSide = Cells[entry.Y][x];
                                break;
                            }
                            GridEntryViewModel cellRightBelow = Cells[entry.Y + 1][x];
                            if (cellRightBelow.Type == GridEntryType.RunningWater || cellRightBelow.Type == GridEntryType.Sand)
                            {
                                break;
                            }
                        }
                        return Tuple.Create(nextOnLeftSide, nextOnRightSide);
                    })
                    .ToArray();
            foreach (var waterBodyEdges in nextWaterBodyEdges)
            {
                switch (waterBodyEdges.Item1?.Type)
                {
                    case null:
                        //This means we have an edge here
                        break;
                    case GridEntryType.Sand:
                        MakeWaterFlow(waterBodyEdges.Item1);
                        break;
                    case GridEntryType.Clay:
                        //This case is handled by the right edge instead
                        break;
                    default:
                        //This most likely happens because we have a duplicate of the body of water, so this is just to be ignored
                        break;
                }
                switch (waterBodyEdges.Item2?.Type)
                {
                    case null:
                        //This means we have an edge here
                        break;
                    case GridEntryType.Sand:
                        MakeWaterFlow(waterBodyEdges.Item2);
                        break;
                    case GridEntryType.Clay:
                        //If both of the edges are clay, let's freeze the water.
                        if (waterBodyEdges.Item1?.Type == GridEntryType.Clay)
                        {
                            for (var x = waterBodyEdges.Item1.X + 1; x < waterBodyEdges.Item2.X; x++)
                            {
                                MakeWaterStill(Cells[waterBodyEdges.Item1.Y][x]);
                            }
                        }
                        break;
                    default:
                        //This most likely happens because we have a duplicate of the body of water, so this is just to be ignored
                        break;
                }
            }
        }

        private bool CouldWaterFlowDown(GridEntryViewModel entry)
        {
            if (entry.Y == this.YLength - 1)
            {
                //Water can flow out below the map
                return true;
            }
            var cellRightBelow = this.Cells[entry.Y + 1][entry.X];
            if (cellRightBelow.Type == GridEntryType.RunningWater)
            {
                //Water is already running down from here
                return true;
            }
            if (cellRightBelow.Type == GridEntryType.Sand)
            {
                MakeWaterFlow(cellRightBelow);
                return true;
            }
            //There is either clay or still water below us, so we can't run further down
            return false;
        }
    }
}
