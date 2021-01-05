defmodule App do
    def parse inputLine do
        inputLine |> String.to_charlist() |> Enum.map(fn char ->
            case char do
                ?F -> :lower
                ?B -> :upper
                ?L -> :lower
                ?R -> :upper
            end
        end)
    end

    def findSeat(spec, lower, upper) do
        case spec do
            [] -> lower
            [h|t] -> 
                mid = ((upper - lower + 1) / 2) + lower
                case h do
                    :upper -> t |> findSeat(mid, upper)
                    :lower -> t |> findSeat(lower, (mid - 1))
                end
        end
    end

    def getSeatData spec do
        row = spec |> Enum.slice(0, 7) |> findSeat(0, 127)
        column = spec |> Enum.slice(7, 3) |> findSeat(0, 7)
        %{
            :row => row,
            :column => column,
            :seatId => row * 8 + column
        }
    end

    def getSeatsFromFile filename do 
        File.read!(filename) |> 
            String.split("\r\n") |> 
            Enum.map(fn s -> s |> App.parse() |> App.getSeatData() end)
    end

    def findSeat seats do
        seatIds = seats |> Enum.map(fn s -> s.seatId end) |> Enum.sort()
        [_|seatIdsExceptFirst] = seatIds
        {seatBefore, seatAfter} = Enum.zip(seatIds, seatIdsExceptFirst) |>
            Enum.find(fn {firstSeat, secondSeat} -> secondSeat == firstSeat + 2 end)
        seatBefore + 1
    end
end

#418 is too low

"sample.txt" |> App.getSeatsFromFile |> IO.inspect
seatData = "input.txt" |> App.getSeatsFromFile 
topSeat = seatData |> Enum.max_by(fn s -> s.seatId end)
IO.puts "Top seat id: #{topSeat.seatId}"
IO.puts "Found empty seat at: #{seatData |> App.findSeat}"