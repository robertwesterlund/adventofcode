defmodule App do
    def parse input do
        input |> String.split("\r\n\r\n") |> Enum.map(fn groupData ->
            groupData |> String.split("\r\n")
        end)
    end

    def getAnswersForGroup_part1 groupData do
        groupData |> Enum.flat_map(fn s -> String.to_charlist(s) end) |> Enum.uniq
    end

    def getAnswersForGroup_part2 groupData do
        groupSize = groupData |> Enum.count()
        groupData |> 
            Enum.flat_map(fn s -> String.to_charlist(s) end) |> 
            Enum.frequencies() |>
            Map.to_list() |>
            Enum.filter(fn {char, count} -> count == groupSize end) |>
            Enum.map(fn {char, count} -> char end)
    end

    def run filename do
        File.read!(filename) |> parse |> Enum.reduce({0,0}, fn val, {acc_part1, acc_part2} -> 
            {
                acc_part1 + (val |> getAnswersForGroup_part1() |> Enum.count()),
                acc_part2 + (val |> getAnswersForGroup_part2() |> Enum.count())
            }
        end) |> IO.inspect
    end
end

App.run "sample.txt"
App.run "input.txt"