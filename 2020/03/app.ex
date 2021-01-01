defmodule App do
    def parse(input) do
        String.split(input, "\r\n") |> Enum.map(fn line -> 
            line |> String.to_charlist() |> Enum.map(fn char -> 
                case char do
                    ?. -> :empty
                    ?# -> :tree
                end
            end)
        end)
    end

    def slide(map, slope) do 
        slide({0,0}, slope, 0, map)
    end

    def slide({x, y} = _position, {slopeX, slopeY} = slope, treesHitSoFar, map) do
        height = map |> Enum.count
        case y >= height do
            true -> treesHitSoFar
            false -> 
                treesHit = case Enum.fetch!(Enum.fetch!(map, y), x) do
                    :empty -> treesHitSoFar
                    :tree -> treesHitSoFar + 1
                end
                width = map |> Enum.fetch!(1) |> Enum.count
                {rem(x + slopeX, width), y + slopeY} |> slide(slope, treesHit, map)
        end
    end

    def run(filename) do
        map = File.read!(filename) |> parse()
        IO.puts "File: #{filename}"
        IO.puts "Trees hit with ordinary slide: #{map |> slide({3,1})}"
        result = [{1,1}, {3,1}, {5,1}, {7,1}, {1,2}] |> Enum.map(fn slope ->
            slide(map, slope)
        end)
        product = result |> Enum.reduce(1, fn (val, acc) -> acc * val end)
        IO.puts "Product of all slopes: #{product}"
    end
end

App.run "sample.txt"
App.run "input.txt"