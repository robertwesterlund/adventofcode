defmodule App do
    def parseExpenceReport input do
        split = String.split(input, "\r\n", trim: true)
        Enum.map(split, fn s -> {v, _} = Integer.parse(s); v end)
    end

    def findEntryWhichSumsToTargetValue(currentValue, targetValue, data) do
        case data do
            [] -> nil
            _ -> 
                [h|t] = data
                case h + currentValue do
                    ^targetValue -> h
                    _ -> findEntryWhichSumsToTargetValue(currentValue, targetValue, t)
                end
        end
    end

    def findPairWhichSumsTo(targetValue, data) do
        case data do
            [] -> nil
            _ -> 
                [h|t] = data
                case findEntryWhichSumsToTargetValue(h, targetValue, data) do
                    nil -> findPairWhichSumsTo(targetValue, t)
                    val -> { h, val }
                end
        end
    end

    def findTripleWhichSumsTo2020 data do
        [h|t] = data
        targetForOtherTwoValues = 2020 - h
        case findPairWhichSumsTo(targetForOtherTwoValues, t) do
            nil -> findTripleWhichSumsTo2020 t
            match -> {h, elem(match, 0), elem(match, 1)}            
        end
    end

    def run input do
        data = parseExpenceReport input
        matches = findPairWhichSumsTo(2020, data)

        IO.puts "Finding pairs"
        IO.puts "Found the following entries: #{elem(matches,0)}, #{elem(matches,1)}"
        IO.puts "Product of the two becomes: #{elem(matches,0) * elem(matches,1)}"

        tripleMatches = findTripleWhichSumsTo2020 data

        IO.puts "Finding triple"
        IO.puts "Found the following entries: #{elem(tripleMatches,0)}, #{elem(tripleMatches,1)}, #{elem(tripleMatches,2)}"
        IO.puts "Product of the three becomes: #{elem(tripleMatches,0) * elem(tripleMatches,1) * elem(tripleMatches,2)}"
    end
end

sampleText = File.read!("sample.txt")
IO.puts "Using sample data"
IO.puts "-----------------"
App.run sampleText
IO.puts ""

inputText = File.read!("input.txt")
IO.puts "Using input data"
IO.puts "----------------"
App.run  inputText