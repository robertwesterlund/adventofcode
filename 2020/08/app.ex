defmodule App do
    def parse input do
        input |> 
            String.split("\r\n") |> 
            Enum.map(fn line ->
                matches = Regex.named_captures(~r/^(?<op>[a-z]+) \+?(?<val>-?\d+)$/, line)
                {val, _} = Integer.parse(matches["val"])
                %{
                    :value => val,
                    :operation => case matches["op"] do
                        "nop" -> :nop
                        "acc" -> :acc
                        "jmp" -> :jmp
                    end
                }
            end) |>
            Enum.with_index() |>
            Enum.reduce(%{}, fn {action, index}, map -> map |> Map.put(index, action) end)
    end

    def execute(context, action) do
        case action.operation do
            :nop -> %{context | :pointer => context.pointer + 1}
            :acc -> %{context | :accumulator => context.accumulator + action.value, :pointer => context.pointer + 1}
            :jmp -> %{context | :pointer => context.pointer + action.value}
        end
    end

    def run_until_loop_detected(context, previousPointerPositions \\ []) do
        case {previousPointerPositions |> Enum.member?(context.pointer), context.instructions[context.pointer]} do
            {true, _} -> context
            {false, nil} -> context
            {false, action} -> 
                context |> execute(action) |> run_until_loop_detected([context.pointer | previousPointerPositions])
        end
    end

    def mutate_and_run(originalContext, currentInstructionToMutate \\ 0) do
        actionToMutate = originalContext.instructions[currentInstructionToMutate]
        targetPointerNumber = originalContext.instructions |> map_size()
        case actionToMutate.operation do
            :acc -> originalContext |> mutate_and_run(currentInstructionToMutate + 1)
            _ -> 
                newAction = case actionToMutate.operation do
                    :nop -> %{ actionToMutate | :operation => :jmp }
                    :jmp -> %{ actionToMutate | :operation => :nop }
                end
                mutatedContext = %{
                    originalContext | 
                    :instructions => %{originalContext.instructions | currentInstructionToMutate => newAction}
                }
                result = mutatedContext |> run_until_loop_detected()
                case result.pointer do
                    ^targetPointerNumber -> result
                    _ -> originalContext |> mutate_and_run(currentInstructionToMutate + 1)
                end
        end
    end

    def run filename do
        IO.puts "File: #{filename}"
        instructions = File.read!(filename) |> parse()
        context = %{
            :instructions => instructions,
            :pointer => 0,
            :accumulator => 0
        } 
        p1_result = context |> run_until_loop_detected()
        IO.puts "Part 1 - Accumulator value when loop was going to occur: #{p1_result.accumulator}"
        p2_result = context |> mutate_and_run()
        IO.puts "Part 2 - Accumulator value when program existed correctly: #{p2_result.accumulator}"
    end
end

App.run "sample.txt"
App.run "input.txt"