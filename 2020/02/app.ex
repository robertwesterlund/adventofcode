defmodule App do
    def parse(input) do
        Enum.map(String.split(input, "\r\n"), fn line -> 
            [policy, password] = String.split(line, ": ")
            [firstString, secondString, character] = String.split(policy, ~r/-| /)
            {first, _} = Integer.parse(firstString)
            {second, _} = Integer.parse(secondString)
            {:ok, part1Regex} = Regex.compile("^(?:[^#{character}]*#{character}){#{first},#{second}}[^#{character}]*$")
            part2Alt1 = ("" |> String.pad_trailing(first - 1, ".")) <> character <> ("" |> String.pad_trailing(second - first - 1, ".")) <> "[^#{character}].*"
            part2Alt2 = ("" |> String.pad_trailing(first - 1, ".")) <> "[^#{character}]" <> ("" |> String.pad_trailing(second - first - 1, ".")) <> "#{character}.*"
            {:ok, part2Regex} = Regex.compile("^#{part2Alt1}$|^#{part2Alt2}$")
            {
                {first, second, character, policy},
                password,
                {
                    Regex.match?(part1Regex, password),
                    Regex.match?(part2Regex, password)
                }
            }
        end)
    end
    def run(filename) do
        IO.puts "File: #{filename}"
        results = File.read!(filename) |> App.parse() |> Enum.map(fn {_policy, _password, r} -> r end)
        IO.puts "Part 1: #{results |> Enum.filter(fn {isPart1Valid, _} -> isPart1Valid end) |> Enum.count()}"
        IO.puts "Part 2: #{results |> Enum.filter(fn {_, isPart2Valid} -> isPart2Valid end) |> Enum.count()}"
    end
end

App.run "sample.txt"
App.run "input.txt"