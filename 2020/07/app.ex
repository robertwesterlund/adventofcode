defmodule App do
    def parse input do
        input |>
            String.split("\r\n") |>
            Enum.map(fn line -> 
                [bagString, contentLine] = line |> String.split("contain")
                content =
                    case Regex.match?(~r/no other bags/, contentLine) do
                        true -> []
                        false -> 
                            contentLine |> 
                                String.split(",") |>
                                Enum.map(fn contentBag -> 
                                    c = Regex.named_captures(~r/^(?<count>\d+) (?<bag>[a-z]+ [a-z]+) bag/, contentBag |> String.trim())
                                    {count, _} = Integer.parse(c["count"])
                                    %{
                                        :color => c["bag"],
                                        :count => count
                                    }
                                end)
                    end
                c = Regex.named_captures(~r/^(?<bag>[a-z]+ [a-z]+) bag/, bagString)
                %{
                    :color => c["bag"],
                    :content => content
                }
            end)
    end

    def createTree(currentBag, data) do
        %{
            :bag => currentBag,
            :content => 
                currentBag.content |> 
                    Enum.map(fn c -> 
                        tree = data |> Enum.find(fn b -> b.color == c.color end) |> createTree(data)
                        %{
                            :tree => tree,
                            :count => c.count
                        }
                    end)
        }
    end

    def createTrees data do
        data |> Enum.map(fn b -> createTree(b, data) end)
    end

    def doesTreeContain(tree, colorToLookFor) do
        case tree.bag.color == colorToLookFor do
            true -> true
            false -> tree.content |> Enum.any?(fn c -> c.tree |> doesTreeContain(colorToLookFor) end)
        end
    end

    def countBags(node, multiplier) do
        multiplier + (node.content |> Enum.reduce(0, fn val, acc -> acc + (val.tree |> countBags(multiplier * val.count)) end))
    end

    def run filename do
        IO.puts "File: #{filename}"
        trees = File.read!(filename) |> parse() |> createTrees()
        shinyGoldCount = trees |> Enum.filter(fn t -> t |> doesTreeContain("shiny gold") end) |> Enum.count() 
        # -1 since the gold bag itself is one of them and shouldn't be counted
        IO.puts "Number of bag colors which can contain a shiny gold bag: #{shinyGoldCount - 1}"
        shinyGoldBag = trees |> Enum.find(fn b -> b.bag.color == "shiny gold" end)
        # -1 since the gold bag itself will be counted
        IO.puts "Number of bags inside a shiny gold bag: #{countBags(shinyGoldBag, 1) - 1}"
    end
end

App.run "sample.txt"
App.run "input.txt"