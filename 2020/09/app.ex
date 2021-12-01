defmodule App do
    def parse input do
        input |> String.split("\r\n") |> Enum.map(fn s -> {v, _} = Integer.parse(s); v end)
    end

    def is_sum_of_two_preambles?(value, preamble) do
        with_index = preamble |> Enum.with_index
        with_index |> Enum.any?(fn {p1, i1} -> 
            target = value - p1
            with_index |> Enum.any?(fn {p2, i2} -> p2 == target and i1 != i2 end)
        end)
    end

    def find_first_xmas_outlier(data, preamble_length) do
        preamble = data |> Enum.take(preamble_length)
        value = data |> Enum.at(preamble_length)
        case value |> is_sum_of_two_preambles?(preamble) do
            true -> 
                [_|tail] = data
                find_first_xmas_outlier(tail, preamble_length)
            false -> value
        end
    end

    def find_weak_range(data, xmas_outlier) do
        [_|t] = data
        case find_weak_range(data, xmas_outlier, []) do
            nil -> find_weak_range(t, xmas_outlier)
            val -> val
        end
    end

    def find_weak_range(data, xmas_outlier_remainder, current_weak_range) do
        [h|t] = data
        remainder = xmas_outlier_remainder - h
        case remainder do
            x when x < 0 -> nil
            x when x == 0 -> [h | current_weak_range]
            x when x > 0 -> find_weak_range(t, x, [h | current_weak_range])
        end
    end

    def run(filename, preamble_length) do
        IO.puts "File: #{filename}"
        data = File.read!(filename) |> parse()
        xmas_outlier = data |> find_first_xmas_outlier(preamble_length)
        IO.puts "XMAS outlier: #{xmas_outlier}"
        weak_range = data |> find_weak_range(xmas_outlier)
        weak_range_min = weak_range |> Enum.min()
        weak_range_max = weak_range |> Enum.max()
        IO.puts "XMAS weak range found. Min: #{weak_range_min}, max: #{weak_range_max}, summed: #{weak_range_min + weak_range_max}"
    end
end

App.run("sample.txt", 5)
App.run("input.txt", 25)