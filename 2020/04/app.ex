defmodule App do
    def parse(input) do
        input |> 
            String.split("\r\n\r\n") |> 
            Enum.map(fn allppdata -> 
                pp = allppdata |> 
                    String.split(~r/\n| /, trim: true) |>
                        Enum.reduce(%{}, fn (val, acc) -> 
                            [key, value] = String.split(val, ":", trim: true)
                            acc |> Map.put(String.trim(key), String.trim(value))
                        end)
                { pp, %{ :isValidPart1 => isValidPassportPart1(pp), :isValidPart2 => isValidPassportPart2(pp)} }
            end)
    end

    def validateInteger(value, min, max) do
        {v, rest} = Integer.parse(value)
        case String.length(rest) do
            0 -> min <= v and v <= max
            _ -> false
        end
    end

    def isValidPassportPart1(pp) do
        mandatoryFields = ["byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"]
        mandatoryFields |> Enum.all?(fn key -> pp |> Map.has_key?(key) end)
    end

    def isValidPassportPart2(pp) do
        mandatoryFields = [
            {"byr", fn v -> validateInteger(v, 1920, 2002) end}, 
            {"iyr", fn v -> validateInteger(v, 2010, 2020) end}, 
            {"eyr", fn v -> validateInteger(v, 2020, 2030) end}, 
            {"hgt", fn v -> 
                strlen = String.length(v)
                strlen > 2 && case String.slice(v, strlen - 2, 2) do
                    "cm" -> validateInteger(String.slice(v, 0, strlen - 2), 150, 193)
                    "in" -> validateInteger(String.slice(v, 0, strlen - 2), 59, 76)
                    _ -> false
                end 
            end},
            {"hcl", fn v -> Regex.match?(~r/^#[a-f0-9]{6}$/, v) end},
            {"ecl", fn v -> Regex.match?(~r/^(?:amb|blu|brn|gry|grn|hzl|oth)$/, v) end},
            {"pid", fn v -> Regex.match?(~r/^[0-9]{9}$/, v) end }
        ]
        mandatoryFields |> Enum.all?(fn {key, validator} -> 
            Map.has_key?(pp, key) && validator.(Map.get(pp, key))
        end)
    end

    def run(filename) do
        IO.puts "File: #{filename}"
        IO.puts "Valid passports, Part1: #{File.read!(filename) |> App.parse() |> Enum.filter(fn {_, d} -> d.isValidPart1 end) |> Enum.count}"
        IO.puts "Valid passports, Part2: #{File.read!(filename) |> App.parse() |> Enum.filter(fn {_, d} -> d.isValidPart2 end) |> Enum.count}"
    end
end

App.run "sample.txt"
App.run "0-valid.txt"
App.run "4-valid.txt"
App.run "input.txt"