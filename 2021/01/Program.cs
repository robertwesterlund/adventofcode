async Task DoTask1(string filename)
{
    var data = (await File.ReadAllLinesAsync(filename)).Select(p => long.Parse(p)).ToArray();
    var result = data.Aggregate<long, (long prev, long numberOfIncreases)>(
        (prev: data[0], numberOfIncreases: 0),
        (acc, cur) => (prev: cur, numberOfIncreases: cur > acc.prev ? acc.numberOfIncreases + 1 : acc.numberOfIncreases));
    Console.WriteLine(result.numberOfIncreases);
}

async Task DoTask2(string filename)
{
    var data = (await File.ReadAllLinesAsync(filename)).Select(p => long.Parse(p)).ToArray();
    var windows = Enumerable.Range(2, data.Length - 2).Select(i => data[i - 2] + data[i - 1] + data[i]).ToArray();
    var result = windows.Aggregate<long, (long prev, long numberOfIncreases)>(
        (prev: windows[0], numberOfIncreases: 0),
        (acc, cur) => (prev: cur, numberOfIncreases: cur > acc.prev ? acc.numberOfIncreases + 1 : acc.numberOfIncreases));
    Console.WriteLine(result.numberOfIncreases);
}

await DoTask1("testdata.txt");
await DoTask1("input.txt");
await DoTask2("testdata.txt");
await DoTask2("input.txt");