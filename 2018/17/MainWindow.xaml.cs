using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace _17
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.DataContext = new FakeGridDataViewModel();
        }

        private void ApplyData(ScanResult[] scans)
        {
            var minYValue = scans.Min(s => s.Y.Min());
            var maxYValue = scans.Max(s => s.Y.Max());
            var minXValue = scans.Min(s => s.X.Min());
            var maxXValue = scans.Max(s => s.X.Max());
            var yLength = maxYValue - minYValue + 1;
            var xLength = maxXValue - minXValue + 1;
            var grid =
                Enumerable
                    .Range(0, yLength)
                    .Select(y =>
                    {
                        var offsettedY = y + minYValue;
                        return
                            new[] { new GridEntryViewModel() { Type = GridEntryType.Sand, X = 0, Y = y } } //Allow overflow on left side
                            .Concat(
                                Enumerable.Range(0, xLength)
                                .Select(x =>
                                {
                                    var offsettedX = x + minXValue;
                                    var isClay = scans.Any(s => s.X.Contains(offsettedX) && s.Y.Contains(offsettedY));
                                    return new GridEntryViewModel()
                                    {
                                        Type = isClay ? GridEntryType.Clay : GridEntryType.Sand,
                                        X = x + 1, // +1 to compensate for allowing overflowing on left side
                                        Y = y
                                    };
                                })
                            )
                            .Concat(
                                new[] { new GridEntryViewModel() { Type = GridEntryType.Sand, X = xLength + 1, Y = y } } //Allow overflow on right side
                            )
                            .ToArray();
                    })
                    .ToArray();

            var wellLocation = 500 - minXValue + 1; // +1 to compensate for allowing overflowing on left side
            var vm = new GridDataViewModel(wellLocation, grid);

            SetDataContext(vm);
        }

        private void SetDataContext(GridDataViewModel vm)
        {
            SetDataLoadButtonsEnabledTo(false);
            SetFlowButtonsEnabledTo(true);

            ItemsPanel.RowDefinitions.Clear();
            for (var y = 0; y < vm.ViewBoxYSize; y++)
            {
                ItemsPanel.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
            ItemsPanel.ColumnDefinitions.Clear();
            for (var x = 0; x < vm.ViewBoxXSize; x++)
            {
                ItemsPanel.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            this.DataContext = vm;
        }

        private System.Windows.Controls.Grid ItemsPanel;

        private void LoadData(string path)
        {
            SetDataLoadButtonsEnabledTo(false);
            SetFlowButtonsEnabledTo(true);

            var fileContent = File.ReadAllLines(path);
            int[] parseValueToArray(string value)
            {
                if (value.Contains("."))
                {
                    var split = value.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    var minValue = int.Parse(split[0]);
                    var maxValue = int.Parse(split[1]);
                    return Enumerable.Range(minValue, maxValue - minValue + 1).ToArray();
                }
                else
                {
                    return new[] { int.Parse(value) };
                }
            }
            var scans = fileContent.Select(line =>
            {
                var match = Regex.Match(line, @"^(?:x=(?<x>[\d\.]+)(?:, )?|y=(?<y>[\d\.]+)(?:, )?){2}$");
                return new ScanResult()
                {
                    X = parseValueToArray(match.Groups["x"].Value),
                    Y = parseValueToArray(match.Groups["y"].Value)
                };
            }).ToArray();
            ApplyData(scans);
        }

        private void SetFlowButtonsEnabledTo(bool val)
        {
            flowButton.IsEnabled = val;
            flowUntilDoneButton.IsEnabled = val;
            saveStateToFile.IsEnabled = val;
            flowPlayButton.IsEnabled = val;
        }

        private void SetDataLoadButtonsEnabledTo(bool val)
        {
            loadSampleDataButton.IsEnabled = val;
            loadBigTestDataButton.IsEnabled = val;
            loadRealDataButton.IsEnabled = val;
            LoadSandboxDataButton.IsEnabled = val;
        }

        private void FlowUntilDone_Click(object sender, RoutedEventArgs e)
        {
            var grid = (GridDataViewModel)DataContext;
            var previousValue = grid.WaterCellCount;
            var tryNumberOfTimes = 10;
            while (true)
            {
                grid.LetWaterTakeOneStep();
                var valueAfter = grid.WaterCellCount;
                if (previousValue == valueAfter)
                {
                    if (tryNumberOfTimes-- <= 0)
                    {
                        break;
                    }
                }
                else
                {
                    previousValue = valueAfter;
                    tryNumberOfTimes = 10;
                }
            }
        }

        private void Flow_Click(object sender, RoutedEventArgs e)
        {
            ((GridDataViewModel)DataContext).LetWaterTakeOneStep();
        }

        private void LoadRealData_Click(object sender, RoutedEventArgs e)
        {
            LoadData("input.data");
        }

        private void LoadSampleData_Click(object sender, RoutedEventArgs e)
        {
            LoadData("sample.data");
        }

        private void LoadBigTestData_Click(object sender, RoutedEventArgs e)
        {
            LoadData("BigTest.data");
        }

        private void LoadSandbox_Click(object sender, RoutedEventArgs e)
        {
            var grid = new GridDataViewModel(50,
                Enumerable.Range(0, 100).Select(y =>
                    Enumerable.Range(0, 100).Select(x =>
                        new GridEntryViewModel()
                        {
                            Type = GridEntryType.Sand,
                            X = x,
                            Y = y
                        }
                    ).ToArray()
                ).ToArray()
            );
            SetDataContext(grid);
        }

        private async void FlowPlay_Click(object sender, RoutedEventArgs e)
        {
            SetFlowButtonsEnabledTo(false);

            var grid = (GridDataViewModel)DataContext;
            var previousValue = grid.WaterCellCount;
            var tryNumberOfTimes = 10;
            while (true)
            {
                await Task.Delay(50);
                grid.LetWaterTakeOneStep();
                var valueAfter = grid.WaterCellCount;
                if (previousValue == valueAfter)
                {
                    if (tryNumberOfTimes-- <= 0)
                    {
                        break;
                    }
                }
                else
                {
                    previousValue = valueAfter;
                    tryNumberOfTimes = 10;
                }
            }

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ItemsPanel = (System.Windows.Controls.Grid)sender;
        }

        private void SaveState_Click(object sender, RoutedEventArgs e)
        {
            var grid = (GridDataViewModel)DataContext;
            using (var stream = System.IO.File.OpenWrite("output.txt"))
            {
                using (var streamWriter = new System.IO.StreamWriter(stream))
                {
                    for (var y = 0; y < grid.YLength; y++)
                    {
                        for (var x = 0; x < grid.XLength; x++)
                        {
                            var character = '+';
                            switch (grid.Cells[y][x].Type)
                            {
                                case GridEntryType.Clay:
                                    character = '#';
                                    break;
                                case GridEntryType.Sand:
                                    character = '.';
                                    break;
                                case GridEntryType.RunningWater:
                                    character = '|';
                                    break;
                                case GridEntryType.StillWater:
                                    character = '~';
                                    break;
                                default:
                                    throw new Exception("Don't have any idea how to handle this one");
                            }
                            streamWriter.Write(character);
                        }
                        streamWriter.WriteLine();
                    }
                }
            }
        }

        private void HandleMouseOnRectangle(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                var rect = (System.Windows.Shapes.Rectangle)sender;
                var context = rect.DataContext as GridEntryViewModel;
                context.Type = GridEntryType.Clay;
            }
        }
    }
}
