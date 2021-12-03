OccuranceCount[] CountOccurances(string[] data)
{
    int length = data[0].Length;
    var result = data.Aggregate(
        Enumerable.Range(0, length).Select(index => new OccuranceCount(BitPosition: length - 1 - index, Index: index, Zeros: 0L, Ones: 0L)),
        (state, line) => state.Select(i => line[i.Index] switch
        {
            '1' => i with { Ones = i.Ones + 1L },
            '0' => i with { Zeros = i.Zeros + 1L },
            _ => throw new Exception($"Unknown value {line[i.Index]} at index {i.Index} in line {line}")
        })
    ).ToArray();
    return result;
}

long GetPart2Result(string[] data)
{
    string[] findMatch(string[] innerData, int index, Func<OccuranceCount, char> filterFunc)
    {
        var counts = CountOccurances(innerData);
        var countAtIndex = counts.SingleOrDefault(c => c.Index == index);
        var lookForCharacter = countAtIndex == null ? 'X' : filterFunc(countAtIndex);
        return innerData.Length switch
        {
            1 => innerData,
            _ => findMatch(
                innerData.Where(l => l[index] == lookForCharacter).ToArray(),
                index + 1,
                filterFunc
            )
        };
    }
    var length = data[0].Length;
    var oxygenRating = findMatch(data, 0, c => c.Ones >= c.Zeros ? '1' : '0')[0].Select((b, i) => b == '1' ? 1L << (length - 1 - i) : 0).Sum();
    var co2ScrubberRating = findMatch(data, 0, c => c.Ones >= c.Zeros ? '0' : '1')[0].Select((b, i) => b == '1' ? 1L << (length - 1 - i) : 0).Sum();
    return oxygenRating * co2ScrubberRating;
}

long GetPart1Result(string[] data)
{
    var result = CountOccurances(data);
    var gammaRate = result.Where(b => b.Ones > b.Zeros).Aggregate(0L, (acc, val) => acc + (1L << val.BitPosition));
    var epsilonRate = result.Where(b => b.Ones < b.Zeros).Aggregate(0L, (acc, val) => acc + (1L << val.BitPosition));
    return gammaRate * epsilonRate;
}
System.Console.WriteLine("Part 1 - Test: " + GetPart1Result(Data.TestData));
System.Console.WriteLine("Part 1 - Real: " + GetPart1Result(Data.Input));
System.Console.WriteLine("Part 2 - Test: " + GetPart2Result(Data.TestData));
System.Console.WriteLine("Part 2 - Real: " + GetPart2Result(Data.Input));

record OccuranceCount(int BitPosition, int Index, long Zeros, long Ones);