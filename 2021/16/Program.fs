type Packet =
    | Literal of version: int64 * value: int64
    | Operator of version: int64 * typeId: int64 * packets: Packet list

type ParseContext =
    { Data: System.Collections.BitArray
      PacketStartIndex: int32 }

let getBitsFromHex hexString =
    System.Convert.FromHexString((string) hexString)
    |> Seq.map (fun byte ->
        seq {
            for bit in System.Collections.BitArray([| byte |]) do
                yield bit
        }
        |> Seq.rev)
    |> Seq.concat
    |> Seq.toArray
    |> System.Collections.BitArray

let convertToInt bits =
    bits
    |> Seq.rev
    |> Seq.indexed
    |> Seq.filter (fun (_, v) -> v)
    |> Seq.fold (fun acc (index, _) -> acc + (1L <<< index)) 0L

let getBits count from (array: System.Collections.BitArray) =
    seq { from .. (from + count - 1) }
    |> Seq.map (fun i -> array.[i])

let getThreeBits = getBits 3
let getFourBits = getBits 4

let getVersion context =
    context.Data
    |> getThreeBits context.PacketStartIndex
    |> convertToInt

let getTypeId context =
    context.Data
    |> getThreeBits (context.PacketStartIndex + 3)
    |> convertToInt

let getLiteralValueAndLength context =
    let rec getLiteralBits arr index soFar =
        let bits = arr |> getFourBits (index + 1)

        match arr.[index] with
        | false -> bits |> Seq.append soFar
        | true ->
            bits
            |> Seq.append soFar
            |> getLiteralBits arr (index + 5)

    let literalBits =
        getLiteralBits context.Data (context.PacketStartIndex + 6) Seq.empty

    let literalValueLength =
        match literalBits |> Seq.length with
        | x -> x + x / 4 // The + x/4 part is because of the extra bit for stating whether it is the last one or not

    (literalBits |> convertToInt, literalValueLength)


let rec parse context =
    //printfn "Parsing from index %i" context.PacketStartIndex

    let GetOperatorPackets opContext =
        let lengthTypeId =
            opContext.Data.[opContext.PacketStartIndex + 6]

        match lengthTypeId with
        | true ->
            let numberOfSubPackets =
                opContext.Data
                |> getBits 11 (opContext.PacketStartIndex + 7)
                |> convertToInt

            let mutable subpacketOffset = 6 + 1 + 11

            let packets =
                [ 1 .. (int32) numberOfSubPackets ]
                |> List.map (fun _ ->
                    let (sub, subSize) =
                        { opContext with PacketStartIndex = opContext.PacketStartIndex + subpacketOffset }
                        |> parse

                    subpacketOffset <- subpacketOffset + subSize
                    sub)

            (packets, subpacketOffset)
        | false ->
            let subpacketBits =
                opContext.Data
                |> getBits 15 (opContext.PacketStartIndex + 7)
                |> convertToInt

            let initialSubpacketOffset = 6 + 1 + 15
            let mutable additionalSubpacketOffset = 0

            let packets =
                [ while additionalSubpacketOffset < (int32) subpacketBits do
                      let (sub, subSize) =
                          { opContext with
                              PacketStartIndex =
                                  opContext.PacketStartIndex
                                  + initialSubpacketOffset
                                  + additionalSubpacketOffset }
                          |> parse

                      additionalSubpacketOffset <- additionalSubpacketOffset + subSize
                      yield sub ]

            (packets, initialSubpacketOffset + additionalSubpacketOffset)

    let typeId = context |> getTypeId

    match typeId with
    | 4L ->
        let (literalValue, valueLength) = context |> getLiteralValueAndLength
        let packetLength = 3 + 3 + valueLength

        (Literal(version = (context |> getVersion), value = literalValue), packetLength)
    | version ->
        let (subpackets, opSize) = context |> GetOperatorPackets
        (Operator(version = (context |> getVersion), typeId = typeId, packets = subpackets), opSize)

let rec sumVersions packet =
    match packet with
    | Literal (v, _) -> v
    | Operator (v, _, p) -> v + (p |> List.sumBy sumVersions)

let rec sum packet =
    let intmax = System.Int64.MaxValue
    let intmin = System.Int64.MinValue

    match packet with
    | Literal (_, value) -> value
    | Operator (_, opCode, p) ->
        match opCode with
        | 0L -> p |> List.sumBy sum
        | 1L -> p |> List.fold (fun s p2 -> s * (sum p2)) 1L
        | 2L -> p |> List.fold (fun s p2 -> min s (sum p2)) intmax
        | 3L -> p |> List.fold (fun s p2 -> max s (sum p2)) intmin
        | 5L
        | 6L
        | 7L ->
            let [ p1; p2 ] = p

            match opCode with
            | 5L -> if sum p1 > sum p2 then 1 else 0
            | 6L -> if sum p1 < sum p2 then 1 else 0
            | 7L -> if sum p1 = sum p2 then 1 else 0

// let printValue value =
//     printf "%s: " value

//     seq {
//         for bit in (getBitsFromHex value) do
//             match bit with
//             | true -> 1
//             | false -> 0
//     }
//     |> Seq.iter (printf "%i")

//     printfn ""

// [ "0"
//   "1"
//   "2"
//   "3"
//   "4"
//   "5"
//   "6"
//   "7"
//   "8"
//   "9"
//   "A"
//   "B"
//   "C"
//   "D"
//   "E"
//   "F" ]
// |> List.iter (fun v -> printValue ("0" + v))

// "D2FE28" |> printValue

{ Data = (getBitsFromHex "D2FE28")
  PacketStartIndex = 0 }
|> parse
|> printfn "%O"


{ Data = (getBitsFromHex "EE00D40C823060")
  PacketStartIndex = 0 }
|> parse
|> printfn "%O"

{ Data = (getBitsFromHex "38006F45291200")
  PacketStartIndex = 0 }
|> parse
|> printfn "%O"

{ Data = (getBitsFromHex "A0016C880162017C3686B18A3D4780")
  PacketStartIndex = 0 }
|> parse
|> printfn "%O"

{ Data = (getBitsFromHex "A0016C880162017C3686B18A3D4780")
  PacketStartIndex = 0 }
|> parse
|> fst
|> sumVersions
|> printfn "%O"


{ Data =
    (getBitsFromHex
        "A20D5080210CE4BB9BAFB001BD14A4574C014C004AE46A9B2E27297EECF0C013F00564776D7E3A825CAB8CD47B6C537DB99CD746674C1000D29BBC5AC80442966FB004C401F8771B61D8803D0B22E4682010EE7E59ACE5BC086003E3270AE4024E15C8010073B2FAD98E004333F9957BCB602E7024C01197AD452C01295CE2DC9934928B005DD258A6637F534CB3D89A944230043801A596B234B7E58509E88798029600BCF5B3BA114F5B3BA10C9E77BAF20FA4016FCDD13340118B929DD4FD54E60327C00BEB7002080AA850031400D002369400B10034400F30021400F20157D804AD400FE00034E000A6D001EB2004E5C00B9AE3AC3C300470029091ACADBFA048D656DFD126792187008635CD736B3231A51BA5EBDF42D4D299804F26B33C872E213C840022EC9C21FFB34EDE7C559C8964B43F8AD77570200FC66697AFEB6C757AC0179AB641E6AD9022006065CEA714A4D24C0179F8E795D3078026200FC118EB1B40010A8D11EA27100990200C45A83F12C401A8611D60A0803B1723542889537EFB24D6E0844004248B1980292D608D00423F49F9908049798B4452C0131006230C14868200FC668B50650043196A7F95569CF6B663341535DCFE919C464400A96DCE1C6B96D5EEFE60096006A400087C1E8610A4401887D1863AC99F9802DC00D34B5BCD72D6F36CB6E7D95EBC600013A88010A8271B6281803B12E124633006A2AC3A8AC600BCD07C9851008712DEAE83A802929DC51EE5EF5AE61BCD0648028596129C3B98129E5A9A329ADD62CCE0164DDF2F9343135CCE2137094A620E53FACF37299F0007392A0B2A7F0BA5F61B3349F3DFAEDE8C01797BD3F8BC48740140004322246A8A2200CC678651AA46F09AEB80191940029A9A9546E79764F7C9D608EA0174B63F815922999A84CE7F95C954D7FD9E0890047D2DC13B0042488259F4C0159922B0046565833828A00ACCD63D189D4983E800AFC955F211C700")
  PacketStartIndex = 0 }
|> parse
|> fst
|> sumVersions
|> printfn "Version number: %O"

{ Data =
    (getBitsFromHex
        "A20D5080210CE4BB9BAFB001BD14A4574C014C004AE46A9B2E27297EECF0C013F00564776D7E3A825CAB8CD47B6C537DB99CD746674C1000D29BBC5AC80442966FB004C401F8771B61D8803D0B22E4682010EE7E59ACE5BC086003E3270AE4024E15C8010073B2FAD98E004333F9957BCB602E7024C01197AD452C01295CE2DC9934928B005DD258A6637F534CB3D89A944230043801A596B234B7E58509E88798029600BCF5B3BA114F5B3BA10C9E77BAF20FA4016FCDD13340118B929DD4FD54E60327C00BEB7002080AA850031400D002369400B10034400F30021400F20157D804AD400FE00034E000A6D001EB2004E5C00B9AE3AC3C300470029091ACADBFA048D656DFD126792187008635CD736B3231A51BA5EBDF42D4D299804F26B33C872E213C840022EC9C21FFB34EDE7C559C8964B43F8AD77570200FC66697AFEB6C757AC0179AB641E6AD9022006065CEA714A4D24C0179F8E795D3078026200FC118EB1B40010A8D11EA27100990200C45A83F12C401A8611D60A0803B1723542889537EFB24D6E0844004248B1980292D608D00423F49F9908049798B4452C0131006230C14868200FC668B50650043196A7F95569CF6B663341535DCFE919C464400A96DCE1C6B96D5EEFE60096006A400087C1E8610A4401887D1863AC99F9802DC00D34B5BCD72D6F36CB6E7D95EBC600013A88010A8271B6281803B12E124633006A2AC3A8AC600BCD07C9851008712DEAE83A802929DC51EE5EF5AE61BCD0648028596129C3B98129E5A9A329ADD62CCE0164DDF2F9343135CCE2137094A620E53FACF37299F0007392A0B2A7F0BA5F61B3349F3DFAEDE8C01797BD3F8BC48740140004322246A8A2200CC678651AA46F09AEB80191940029A9A9546E79764F7C9D608EA0174B63F815922999A84CE7F95C954D7FD9E0890047D2DC13B0042488259F4C0159922B0046565833828A00ACCD63D189D4983E800AFC955F211C700")
  PacketStartIndex = 0 }
|> parse
|> fst
|> sum
|> printfn "Sum is: %O"
