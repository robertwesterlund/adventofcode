namespace _17
{
    public class DesignTimeGridDataViewModel : GridDataViewModel
    {
        public DesignTimeGridDataViewModel()
            : base(1, new GridEntryViewModel[][]
            {
                new GridEntryViewModel[]
                {
                    new GridEntryViewModel()
                    {
                        Y = 0, X = 0, Type = GridEntryType.Clay
                    },
                    new GridEntryViewModel()
                    {
                        Y = 0, X = 1, Type = GridEntryType.Sand
                    },
                    new GridEntryViewModel()
                    {
                        Y = 0, X = 2, Type = GridEntryType.Sand
                    }
                },
                new GridEntryViewModel[]
                {
                    new GridEntryViewModel()
                    {
                        Y = 1, X = 0, Type = GridEntryType.Sand
                    },
                    new GridEntryViewModel()
                    {
                        Y = 1, X = 1, Type = GridEntryType.StillWater
                    },
                    new GridEntryViewModel()
                    {
                        Y = 1, X = 2, Type = GridEntryType.Sand
                    }
                }
            })
        {
            LetWaterTakeOneStep();
        }
    }
}
