﻿let testData1 =
    """
-L|F7
7S-7|
L|7||
-L-J|
L|-JF
    """
        .Replace("\r", "")
        .Trim()

let testData2 =
    """
7-F7-
.FJ|7
SJLL7
|F--J
LJ.LJ
    """
        .Replace("\r", "")
        .Trim()

let testData3 =
    """
...........
.S-------7.
.|F-----7|.
.||.....||.
.||.....||.
.|L-7.F-J|.
.|..|.|..|.
.L--J.L--J.
...........
    """
        .Replace("\r", "")
        .Trim()

let testData4 =
    """
.F----7F7F7F7F-7....
.|F--7||||||||FJ....
.||.FJ||||||||L7....
FJL7L7LJLJ||LJ.L-7..
L--J.L7...LJS7F-7L7.
....F-J..F7FJ|L7L7L7
....L7.F7||L7|.L7L7|
.....|FJLJ|FJ|F7|.LJ
....FJL-7.||.||||...
....L---J.LJ.LJLJ...
    """
        .Replace("\r", "")
        .Trim()

let testData5 =
    """
FF7FSF7F7F7F7F7F---7
L|LJ||||||||||||F--J
FL-7LJLJ||||||LJL-77
F--JF--7||LJLJ7F7FJ-
L---JF-JLJ.||-FJLJJ7
|F|F-JF---7F7-L7L|7|
|FFJF7L7F-JF7|JL---7
7-L-JL7||F7|L7F-7F7|
L.L7LFJ|||||FJL7||LJ
L7JLJL-JLJLJL--JLJ.L
    """
        .Replace("\r", "")
        .Trim()

let realData =
    """
-L7F7-J77|-FF7F-.|-JF77FF-FLJF--F--77-77-F|-F77.FF|7-F|-L-LL.L.77F7F-J.F-LFJ7-F|-F7J-F|.F7.FF-7.FJ7FL7F|-F-F---F-FL-F-7J7F.F77.F77FJ.77.L.7J
J.||L-7FL|.|LF|.7JF-LJF-J7J.|7.F-7FF7F|FF-F-JJ-77J|F.|J-|.|.FL7--F77LJF|-LLL-7J||LJ7-JJ.LJL-J|F77|F-7-J..-F-JLFFFJJ||7|7-F-J7--L7-FJLJLJ-L..
|-|-JLLJ|LF|-J|FJF7.L7-JFF.-|7L7FLJ|FF-7-J||7L7F7.|.--7-LF7F|F7JF||LLF7L7.7JJ.-FFJ|7||.LJ7.LF-|.F7J.|JF|7J|77.L7JJ7|.F77-J7LJFJ-7.L7L7JJ7-LF
FF7LF7.FLFL.LL-7|-J.L---|F-.-7JFF-J7FF7JJFFLJF-7F7F7F7.FJF-77JL77.J.|||FJ-7-F7.L7-7L-J-|F-7J|FJ--7-F|F-L.--JL|L|77|JF||L-J7--77--J77F|L.|.|J
FLJ7FF-F7LLF.FJ7..-L-JJ7|J.FLL-FF7L-LJL|F-LJJJLFJLJL7F7|F|FJ7LFFJFLFFFJJJ7L-JFJJLL|7|---|-7-J7|.-|LLJ7|LF|J7F7-J.LJ7-LJFLL-7.FL7.LF7||.FJ7JF
L.L|L|7J77LLFL7L--L-7JJJLJ-7J7FJLF.|7F-F-FJJ|.-L--7FJ||F-J|JJ7.F7J|FJ|JJL77FL-.|LF|.F7-LLJ||FF--7F-J-LJ.FJ.LJJ||-.FF7-LL7FL.7JJF7.JL-J-J-L-|
|7JL7LJ-F777JF-J.FJFL77F||...-7-FJ7F|J-|FJL|7FJLF7|L7|||F7|F7-FJ|L|JF7|F-FJ|.|.7-FJ-|J-.L-LJJJJFJ|L77|-FJFFJJFF|-L7.--.L-F.L7|F7--7F||L7|LFJ
||LF|---FJ||JL7.777L777F7-FJ-|L|JF-FJJ7J|JFF7FF-JLJFJ||LJ|LJL7L7L7F-JL-77JJ|-7LF-7.LLJ.FL-|..F--JF7L77F7J-L--|---F7.|||J..|.|7LJ.||LLJF|7FFJ
-.LJJL-J.FF||L|J..-.|J7||.-7L-77FJ.-.L-.|7FF77L---7L7||JFJF--J-L7|L7F--JJ77FJ|-|-J77.|F7FLF--7-JL|.F.F-J..JJFLJ.-F7---JF7FL--F7LFFL7-|F|F-FJ
L-7.7J.L-|J77J||7.F-7F-77-L|.LJF--F|7|L7FF7||F7-F-JFJ||FJFJ-F7FFJL7|L7F7JF--77.F7-F7F7-|77L-7|7F7F-7FFJF-JJFFJF|.|JJL|J-LJ-7JJL7J7-|7F|F--J.
.FF-LF.|FJ.-J.LL-77LLFJLF77F7.F7|.FF77F-FJLJ|||FJF7L-J|L7|F-JL-JF-JL7|||.L-7L7-||FJLJL-7.FFFJL-JL7LL7|-||.LFJJ.FF7JJ.|7..LFL-FJLL7-L--J|L|LJ
L-L77LF7JL7.F7L|J.|LF|L-|L7||7|L7F--7F7-L--7|||L-JL-7FJFJ|L---7FJF7FJLJL7F7|FJFJ|L7F-7FJ7F7L---7FJF7L|-L7--JFFF-J|..7||-7-|||L7|FL7J|L7J.LL|
F||..F.JJFL--|F7F7FFFL7-L7||L7L7|L-7|||-F7FJLJL-7F--J|7L7L7F7FJ|-|||F---J|LJL7L7L7|L7LJ-FJL-77FJL-JL77|L|.L-F-|F-J..L||||F-7-.FJ-LJ.|-|.FF.F
-.7FL7F|||F..-LJ-L--|.F-7|||FJ-||F-J||L7|LJF7F--JL--7|F7L7|||L7L7|||L7F-7L--7|.L7LJFJF7JL--7|FJF----JL77FF7.F-||JJF|7.|JF7FJLF.|JLL-JFLFJL7L
L7JJ7LF.J7LF7JF7.77L|FL7|||||F7||L-7LJFJL7FJ|L7JF7F7|LJ|FJLJL7|FJ|||FJ|FJJF7||F7|F-JFJL-7F7||L7|F7F7|-FF7JFF7L||.7.F-F--||7FF7-L.7.F7L-7.L-7
FF-LF--7--.|F--|7JFFF-7||||||||||F7L-7L-7||LL7L7|LJ||F-JL-7F7LJ||||||FJL7FJLJLJLJL-7L7F-J|LJL-JLJLJL-7FJ|F7|L7||F7F-7JLFJL--J|JJL--J-.FLJJJ|
F.FF7.L|7|-7|.FL--F-JFJ|||||||LJLJL7FJF-JLJF7|FJL-7|||7F7|LJL-7L-J|||L-7|L7F----7F-JFJL-7L---7F-7F7F-JL7LJLJFJ|||||FJF7L7F---JJ|LJ-J.LF.LFF7
LF-LL7.FF77.FF-.|-L-7L-JLJLJ|L----7|L7L-7F7|LJL--7||||FJL7FF7FJF7FJ||F7||FJL7F-7LJF-JF7FJFF7FJL7LJ||F-7L--7FJFJ|||||FJ|FJL-7J|JF7LFJ|7L|7..|
LJJJ||-LJFF.L.|-L-|F|F-----7|F--7F|L7|F7|||L--7F-J||||L-7L7|LJFJLJFJLJLJ||F-J|FJF7L-7|LJF-JLJF-JF7LJL7|.F-JL7|FJ|LJ|L7||F--J-F-77JL-FJ.L|.F|
FFF---7L-|L7F77J|F7FJ|F---7||L7FJFJFJ||LJ||LF7|L7FJ||L7L|FJL-7L-77L--7F7LJL-7|L7|L7FJL-7L7F7FJF7|L-7FJL7L--7||L7L-7L7|LJL--77..L7F-7JF|.L-J|
L-|7L-|J-FFJFL-F-J|L7||F-7LJ|FJ|LL7|FJL7FJL7|||FJL7||FJFJL-77|F-JF7F7LJL---7LJFJ|FJ|F--J|LJ|L7|LJF-JL-7L7F-J||FJF7|FJ|F----J7-|L|FL---JJJ.LF
LL|7J.|-F-L-J|7L-7|.LJ||JL-7||FJF-J|L7FJL-7|||||.FJLJL7|F7FJFJL7-||||F7F---JF-JFJ|FJL-7F-7FJFJ|F-JF7F7|FJL7FJ||J|||L-JL----7J|L-L7.|7|.L7FFF
L|.|JFL7J7LL7|LJFJL--7LJF7FJLJ|.L-7|FJL7F7|||||L7L--7FJLJ|L-JF-JFJ|||||L--7FJF7|FJ|F--JL7|L7|J|L-7||||||F7|L7|L7|LJF--7F---JJF7|7LFFJ-7.J7LJ
FLLFJFL|7|7.F|JFL-7F7L7FJLJF-7L7F-JLJF-J|||||LJFJFF7||F7||F-7|F7|FJ|LJ|F7|||FJLJL7|L-7F7||7|L7|F-J|LJ|||||L7LJFJL7FJF7LJ.F---JL77F7LJJ|7|F-L
|FL.F-7.|JF7F|LFLFJ|L7LJF--J.L7|L7F-7L--J|||L-7|LFJLJLJL7LJFJLJLJL-JF7||L-JLJF7F-J|F-J||||FJFJ|L-7L-7||||L7L-7|F7|L-JL--7|F----JL-|J|F77|J7|
L|L7LLJ-F7.LL--FJL-JFJF-JF7F7F|L7LJ.L--7FJ|L7FJL7L--7F-7|F7|F-7F7F--JLJL7F-7FJLJF-J|F7||||L7|L|F-JF-J||||FJF7|LJLJF7F7F7LJL7F-77FL|F7-F7F-JJ
F|7J-FJ7LLF7J.||F|LLL-JF7|LJL7|FJF7F---JL7|FJL7FJF7FJL7LJ||LJFJ|LJF--7F7LJ7|L-7FJF-J|LJ|||7|L7||F7L-7LJ|||FJ||F---JLJLJL---J|FJ7J-FF7LLF77|7
-7JJ.J-|-J.J-L77-J7FF--J|L--7||L7||L--7F-J||F7||FJ|L7FJLFJL-7L7L7J|F-J|L7F7|F-JL7L7|L-7||L7L7LJ||L7|L7FJ||L7||L---7F-7F7F---J|F7JF7||7.L|FF7
LJ7.7.F777FJ7.-JJ.L-L--7L7|FJ||FJ||F--J|F-J|||||L7L7||F7L7F-J-L7L-J|F7|FJ|||L7F7|FJF7FJLJFJFJF-J|FJF-J|FJL7|||F---J|FJ|||F--7LJL-JLJL7JFL7.7
|--7.FFL-JLFF7|..7.|7F7L7L-JFJ|L7||L--7|L-7LJLJL7L7||LJ|FJ|F7LFJF7FJ||||FJLJFJ|LJL7||L7F7L7L7L7J|L7|F-JL-7||LJL-7F7||FJ||L-7L---7F7F-JJ7-F-|
J-LL7F|.|.-LLF7.F7-F-J|-L--7L7L7||L7F7|L-7L--7F-JL|LJF-JL7|||FJFJLJFJLJ|L-7FJ.L--7||L7LJ|FJLL7L7|FJ||LF7FJ||F--7LJLJLJFJ|F-JF--7||LJJLLF-JJ|
.|.L7FFF7F.FFJL-JL7L-7|F7F7L7|F|||FJ|||F-JF77|L-7FJF-JF-7|LJ|L7|F--JF--JF-JL-7.F7|||FJF7||LFFJFJ|L7|L7||L7LJL7FJF--7F7L-JL7L|F-J||JFL.FL7FF|
FJJ7LFJJ7.FFL----7L7.|||LJL-JL-J||L7|LJL7.|L7L7FJ|FJF7L7|L-7L7|||F--J.F7L7F--JFJ||LJL7|LJL7FJFJL|FJL7|||L|F--JL7L-7LJ|F7F7L-JL-7LJ7F--7FF|-J
||L--LL-FFF-7JF7.L7L-J|L------7FJ|FJL7F-JFJFJ.|L7|L7|L7||F7|FJ||||F7F7||FJ|F77L7|L-7FJL7F-JL7|F7||F7|||L-JL---7L--JF7LJLJL7F7F-JF7-F7J|J.|J7
L77L7|J7F7L7|F||F7L--7|F------JL-JL7.||F7L7|F7|FJ|FJL7|||||||-|||||||||||FJ||F7||F7||F7||LF7||||||||||L7F-----JF7F7|L7F7F7|||L7|||7.|F.|.|||
FLJ7FF7F7F-JL7||||F7L||L----7F7F7F7L7||||FJ|||||FJ|F-JLJ||LJL7|||||||||||L7|||||||||||||L7|||||LJLJ||L7|L--7F-7|LJLJFJ|LJ|LJL-JFJL7J7|7.F7-F
FJF|-JF|LJF-7LJLJLJL-JL-----J|||LJL7LJ||LJ7||||||FJ|F7.FJ|-F-J|||LJ|||||L7||LJLJLJ||||||FJ|LJLJJF--JL7|L7F-JL7||F---J-|F-JF-7F7L7FJ7|LL7LF.|
||-JJ|LL7FJ-L--7F7F-7F7F--7F7|LJJF7L7FJL--7||LJLJL7LJL7L7L7|F7||L-7||||L7||L--7F--J|||||L7|F-7F7|F7F7|L-J|F-7|LJL7F---JL--JFJ|L-JL7-L--7JF-J
L|L7.F|FLJLF7F-J||L7LJ||F-J|||F--JL-J|F7F-JLJF----JF-7L7|FJ||LJ|F-J||||FJ|L7F7||F-7|||||FJLJFJ|LJ||||L7F-J|FJ|F--J|F---7F--JFJF---J|.L7J7.LJ
FJ7.7-|J7LFJLJF7|L-JF7LJ|F7|LJL-----7|||L---7L-7F7FJ-L7||L7||F-JL7-LJ|||FJFJ||||L7||||||L7F7L7L7FJ|||FJL--J|J||F7|||F7-LJ|F-JFJF7F77F7FF--F7
F-LFJJLLF7L--7|||F--JL-7LJLJF7FF----JLJL-7F-JFFJ||L--7|||FJ||L7F7L7F7LJ|L7L7||||FJLJ||LJFLJ|FJFJL7LJLJFF7F7L7|LJL7|LJL7|F7|F-J7|LJL---7J|7LL
L.|LJJFFJ|F-7|||||F7F-7L--7FJ|FJF7F7F7F7-LJF-7L7|L-7FJ|||L7||FJ|L7LJL-7L7L7||LJLJJF7|L7F---SL7L7FJF----JLJL7LJF-7LJF-7L7||||F--JF-----J7-J7.
L7-|7LLL7||FJ|||||||L7L7F7|L7|L-JLJLJLJL---JFJ|||LFJL7LJ|FJ|||FJ|L7F--J-L7|||LF-7FJLJFJ|F7F7FJL|L7|F------7|F7L7|F7L7L7LJ||||F--JF--7F7J|.|.
FJF|FFFJ||||LLJLJLJL-JLLJ||FJL---7F-7F--7F-7L7FJ|FJF7L7FJL7||||F--JL----7|||L7L7LJF-7|FJ||||L-7|FJLJF--7F-JLJL-JLJL-JFJF-J|LJL---JF7LJL7FL|L
|L-|LF--JLJL7F7F7|F7F---7||L----7|L7|L7|LJJL7|L7||FJ|FJL7FJ||LJL7F7F7F--J||L7L7L-7|FJ||FJLJ|F7||L--7L-7|L----7.F--7F7L-J7FJF------JL7F7|7|LJ
F7FLJL---7F7LJLJL-J|L7F7LJL-----JL-JL-JF7LF7LJJLJ||FJL7-|L7|L7F-J||||L--7|L7L7|.FJ||FJ|L-7FJ||||F--JF-JL--7F7L-JF-J||F---JFJF7F7F--7LJLJJL7.
-7FLF-|LF|||F7F7F-7|7LJL------7F------7|L-JL----7||L-7L7|FJ|FJ|F7||||F7FJ|FJL|L7L7||L7L-7||FJ||||F--JF---7LJL7F7L7FJLJF--7L-JLJLJF7L7LFLJ.J.
FF-J|FJ.-LJ||||LJFJ|F7F------7LJF7F---J|F-7F7F-7|LJF-JFJ||-LJFJ|||||LJ||JLJ7FJFJFJ|L7L-7|||L7LJLJL---JF-7L7F7||L7LJF-7L77L7F7F7F7|L-JF-77FL7
|7J--JJFJF-J|LJF7L7|||L-----7|F7||L----JL7LJ|L7|L-7L-7L7||F7FL7|||||F-JL-7F-JFJ7L7|FJF7|LJ|FJF7F-----7L7L-J||LJ.|F7|LL7L-7LJLJ|||L--77.FJJF-
F7JL||F7|L--JF-JL-JLJ|F7|F7FJLJLJL7F-7F-7L-7L-JL--JF-JFJ|LJL7FJ||||||F7F7|L-7L--7LJL7||L-7LJ-||L----7|FJF7-|L--7|||L77L-7L---7LJ|F-7L--7JJJ.
J|FF|-7|F7LF7L-7F---7LJL-JLJF7F--7|L7|L7|F7L---7F7FJF7L7|F-7||FJ||||||||||F-JF--JF7|LJ|F-J-F7||F7F7FJLJFJL-JF7FJLJL7L7F-JF7F7L-7|L7|F7FJLLJ7
LL-|||F-JL-J|F-J|F--JF7F-7F-JLJF-JL-J|FJLJL7F--J||L7||FJ|L7LJ|L7LJLJ||LJ|||F7L7F-JL---JL-7FJLJLJLJLJF--JF---JLJFF--JFJL-7||||F-J|FJ||LJ||LFJ
..|L-F|F---7|L--JL--7|||FJ|F---JF--7JLJ|F-7|L--7|L7|||L7L7|.FJFJF---JL7.LJ||L-JL--7F-7F--JL-7F7F7F--JLF7L----7F7L---JFF7LJLJ|L7|LJFJL--77--J
7-L7JL||F7FJL----7F7||LJL7|L7F7FJF-JF7F7|FJ|F-7LJFJLJL-JJ|L7L7L7|F7F-7L-7JLJFF7F--J|FJ|F7F7FJ|||||F---JL-----J|L7F7F-7|L---7L7L--7L--7FJ|..7
L-J.7-LJ|LJF7F--7|||LJF7-LJ|LJLJFL--JLJLJ|7||7|F-JF7F---7L7L7L7|||||FJF-JF7F7|LJF7FJL7||LJ|L-JLJLJL7F7F7F7F7F7|FJ|||FJ|F---JJL7F-JF7JLJ||-7J
.||LF--7L--JLJF-J|||F7|L7F-7|F--------7F7L7|L7|L--JLJF-7|LL-JFJ|||||L7L-7||||L--J|L-7LJL-7|F7F7F7F7LJLJLJLJLJLJL7|LJL-JL7F7J.|LJF-J|-L-7JFJ7
.F7-L-7L7F7F--JF7LJ|||L7|L7L7L-------7LJL-JL-J|F7F7F7L7|L--7FL-JLJ||||F7||LJ|F7F7L--J|F--JLJLJLJLJL---7F--------J|F----7LJL7F-7J|F-J||L|LFLF
.FJ-F7L7LJ|L---JL-7||L7||7L7|F--7F---JF7F---7LLJLJ||L-JL--7L7F77|.LJFJ||||F7LJLJL7F7F7L7F7F7F7F--7F--7|L---------JL---7L---J|FJFJL7.J7---7|.
-F|J||.L-7|F7F----J||FJ||F7||L7FJL----JLJF--JF7|F7|L-----7L7LJL7J7-LL7|||LJL--7F7LJLJL7LJLJLJ||F-J|F-JL---7F-7F7F-7F-7L---7FJ|FJF7L7JFF-LL77
|J|F||F--JLJ|L-7F-7LJL7|LJLJL-JL------7F-JF7FJL-J||F7F-7FJFJF--J--77.LJ||F--7-||L7F7F7|F---7LLJL--JL-----7LJFLJ|||LJFJF---J|FJ|FJL-J7F7-7.|-
--F7||L----7L7FLJFJF-7LJF-7F-7F7F7F7F7LJF-JLJF-7FJ||LJJLJ7L-JJLL-7.-JLLLJ|F7L-J|FJ|LJ||L-7FJF7F----7F---7L----7LJF7JL-JF77FJL7|L-7F7FJL7L-F.
|FJLJ|F7F--JFJF--JFJ-L-7L7LJFLJ||LJLJL--JF7F7|||L7|L-------77L7|-F77|J|J-LJL---J|FJF7|L--JL7||L---7|L--7L-----JFFJL7F7FJL7|F7LJF7LJLJF-J-L-7
FL--7LJ|L--7|-L--7L--7F|FJF-7F7||F---7F--JLJ|L7|FJL7F7F7F--J7.F-7L|-LF7FF.LF7F--J|||LJF-7F7LJL----J|F7LL-------7L7FJ||L7FJ||L--JL-7F7L-7-JFL
|JLFJF7L77FJL-7F7|F--JFJ|JL7LJ|LJL--7|L-7F-7L7|||F7LJ|||L---7.J|FFF7LJJFL-FJLJF-7|FJF7L7|||F7F7F7F7LJL------7F7|FJL-JL-JL-JL----7-|||F-J.FL|
|L|L-JL7L-JF-7LJLJL7F7|FJF7L-7|7F-7FJL7FJ|FJJLJLJ||F7||L----J7F||F7J|7||||L--7|F||L-JL-J||||LJ||||L7F7F---7|LJLJL7F----7F-7F----J.LJLJJFF7||
F.FF---JF7FJFJF---7|||LJFJ|F-JL7L7|L-7|L-JL-----7|LJ|LJF7F7LF7-JL7JJL7FJ-JFLFJ|FJ|F-----J|LJF7LJLJFJ|||F--JF---7J||F7F7LJLLJF--7J.F7|F|F--F7
LFJL7F7FJ||FJFJF-7LJ||LFJFJL7F7L-JL--JL----7F---J|F-JF7|LJL-J|7FL77.|F77|LLJL7||FJL-7F-7FJF-JL----JFJ||L---JF--JFJLJLJL-----JF7L--JL-77J7.J.
-7.LLJLJ7LJL7|JL7L7F||FJFJLFLJL-------7F--7||F---JL7FJ||F7F7FJ777FJF|L|-JL|JFJ||||F7||FJ|JL--------JFJ|F----JFF7|F-----7F7F7FJ|F7F---J.7J.F|
L-J.FFF-----JL7.L7|FJ|L7|F7F7F7F7F----J|F-J|||F----J|FJ||LJLJJ|LJ.L--7|-L7JFL-JLJFJLJ|L7|F-----7F--7L-JL---7F7|LJL----7|||||L7LJ|L---7F|-|7F
||.--FL-7F-7F-JF-J|L7|FJLJ|||||||L-----JL-7LJ||F7F--JL-JL7F7J||7.|-JF|-J||L|.|.FFJF7FJ.LJL7F7F7LJF7L-------J|||F------JLJLJL-JF-JF7F-J7JF-JJ
F77L7F--J|FJ|.FJF7L-J|L--7LJLJLJL7F-------JF-JLJLJF-7F7F7LJ|7F|7--FJ-7..|77FF-F-JFJ||F--7L||LJL--J|F7F------JLJL-----7F-----7FJF-JLJ|LF7.|LL
7L7-FL-7FJL7|FJFJL--7|F--JF-----7|L7F------JF7F-7FJFJ|LJL-7L-7J.7J.J.L7-J-|JJ7L--JJLJ|F7L7LJF--7F7||LJF-7F-7F--------JL7F7F-JL-JF7F--7JFL-.L
|-7--F-JL7F||L7|F---JLJF-7|F--7FJL-J|F--7F--JLJJLJ-L7L---7L7FJ---FF.|7||L7J.7-|-L-F7-LJL7|F7L-7LJLJ|F7L7|L7LJF7LF---7F-J|LJFF7LFJ||F-J7|-|77
|F-7.|F7FJ-LJ.LJ|F7F7F7|FJ||F7LJF7F-J|F7LJ7F-------7L7F-7L7LJJL.L7J--JLL-J-F|F|-7FF---7FJLJL-7|F7F7||L-JL-JF7|L-JF7FJ|F7L7F7|L7|FJ|L--77-|FJ
F77F7LJ|||.L|F7.LJLJ||LJ|FJLJL7FJLJF7LJL--7L7F7F--7|FJ|7|FJJJJ.FFF7JJL7|F7LLJFF-LLL--7|L7F--7|LJLJLJL------JLJF--JLJJ||L-J|||FJ||L|F--J77LFJ
FF-||-LLJ7L.FJ|F----JL7FJ|F---J|F--JL-----JFJ||L-7||L7|FJL7JFFFF7||7.F7JJLJ|7|J-|.LF7|L-JL7FJL------------7F-7L7F7F--JL7F7|LJL-JL-J|F|JL-7L-
L|.|7FJ-|F|-L7|L------JL-JL7F--JL7F-----7F7|FJ|F7|||FLJL--JF7F7||||7JJLL77LL7|.--7FJ||F7F7|L-7F7F7F----7F7LJLL7LJLJF--7LJLJF------7L7--LJL7|
F|---F-||-7.FJL---7F-7F7F7FJ|F---J|F---7LJLJL7LJLJLJF7F--7FJ||||LJL77.||L--FL-7L.-L7|LJLJLJ-FJ|LJLJF--7LJL--7FJF7F7L-7L7F-7L-----7L-JJ7LJ.|-
LJFJ-JLFJ-7F|F---7|L7LJLJLJFJL----JL--7L7F7F7L77F7F7|||F-JL7LJ||F--J7-J7-|F-J--7F|.|L7LF7.F-JFJJF--JF7L-----J|FJLJ|F-J7|L7L7F--7FJJJFF-FJFJ|
J||J.LFJ-LF-J|F--JL-JF-7F--JF7F-----7FJ|LJLJL7L7|||||||L7F-JF-J|L---7|LF-777.LFF7F7|FJFJL7L--JF7L---JL7F----7LJF--JL--7|FJ7|L-7LJJ7F|J7..LF7
|-|J.7---JL--JL7F7F-7||LJ|F-J|L7F7F7LJF7JF7F7L7LJLJLJ||FJ|F7L7FJF--7L-7|FJJF-7F||||||FJF-JF-7FJL-----7LJ|F--JF7L---7F-JLJF7L--JF7-F7LJF77..J
F-7.-J7||-|7F-7LJLJF||F---JF7L7LJLJL--JL-JLJ|-L------J|L7LJL7||FJF7L--J||F7L7|F|LJ|||L7|F7L7LJF-7F7F7L-7FJF--JL7F-7LJF---JL-7F7|L7777-L|-|-J
L-7-|LFLJ|.FL7|F----J|L----JL7|F7F---------7L-7LF-7|F7L7L7LFJLJL-J|7F7FJLJL7|L7L-7|||FJ||L7L--JJLJLJL-7|L7L7F--JL7L7FJF-----J|||FJF77.LL7|.|
|||-J-JL-JFF-J|L-----J|F7F--7||||L-7F7JF7F7L-7L7L7L-JL7L7L-JF-----JFJLJF---J|FJF-JLJ|L7||FJ-F7F-7F7F--JL7|FJ|F-7F|FJ|FJ|F7F7FJLJL-J|-|L|J7.|
FLJ7L|L|.F-L-7|F7-F7F7FJLJF7LJLJL-7LJL-JLJ|F7L-J.|F---JFJF-7L--7F-7L--7L7F7L|L7L7F-7L-JLJ|F7||L7|||L---7|||FJ|FJFJL-JL-7|||||F--7F7L7JF--FF7
F-J7FJ-F-7-F-J||L-JLJLJF--JL---7F7L-7F--7FJ|L7F-7||F-7FJFJL|F-7LJFJJF7L7LJ|FJFJ-LJ.|F-7F7LJ|||FJ||L7F-7|LJ|L-JL-JF7F--7||LJLJL-7||L-JJ7|-F.J
|7FLL-FL7|7L7FJL-------J-F-----J|L-7||F7||.L7LJFJ|||FJL7L-7||-L7FJJFJL7|F-JL7|F7F7FJ|FLJ|F7LJ|L7|L7|L7|L--JF--7F-J|L-7|LJF--7F-JLJLJ7J|.|JF.
7F|7JF--JL--JL-7F7.F7F7F7L7F-7F7|F-JLJ||LJF7|F-JFJLJL7FJF7||L77LJF7|F7|||F7FJ||||||FJF7|LJL-7L-JL-JL-JL---7L-7|L-7|F7||F7L-7|L-7J|FL|.J7FFJ7
L-JLFL7F7F-7F-7LJL7|LJLJ|FJ||LJLJL----JL--JLJL--JF7F7LJFJLJL-JF7FJ|LJ||||||L7|||||LJFJL-7F-7L-7F7F-7F7F7F7L--JL7FJ||LJ|||F7||F7|-LF-F--J-JLJ
7J.FJ-LJ||FJ|FJF7FJL---7|L-JF7F---------7F7F-7F7FJ|||F7L--7-F7||L7L--J|||||FJLJ||L7FJF--JL7L-7LJLJ7LJLJ||L----7LJFLJF7LJLJLJLJLJL-JL|JFJ|7.J
|-FJ.|JFLJL7|L-J||.F---JL---JLJF-7F----7||||FJ|||-LJ|||F7FJFJ||L7L---7|||||L--7||FJL7|F7F7L-7|JF77F----J|F-7F7L-----JL--------7|F|F77JJLF7-|
L.L-77F-J|LLJF7FJL7L--7F7F7F-7FJ.LJF-7FJ|||||FJ|L-7JLJLJ||FJFJ|FJLF7FJLJ||L-7FJLJ|F-J|||||F-JL-JL7|F7F--JL7||L7F---7F7F7F-----JF7-||F-7.L||L
.7.7JF|7JF-7L|LJF7|.F7LJ||LJFJ|F---J7LJ7LJLJ|L7|F7L----7LJL7L-J|F7||L-7FJ|F-JL7F-JL-7LJLJ|L----7FJLJ||F---JLJJ|L-7JLJLJ|L--7F-7||FJLJFJ7.LFJ
J7F|F-.J-L7|-L-7||L-JL--J|F7|FJL---7F------7L7|LJL7F7F7L-7FL7F-J||||F-J|J||.F-JL7LF7L7F--JF-7F-JL--7LJL7F7F-7FJF7L-7F7.L--7LJFJ|||F7FJ-|-FL-
L--|-7-|..|L--7||L7F7F7F7|||||F----J|F-7F--JFLJF7-LJLJL7FJF7|L7FJLJ|L7FJFJL7L--7L7||FJ|F-7L7|L----7|F7.LJLJFJL7||F7LJL7F7FL--JF||||LJ-LJ.777
F-FJ.L.L-FJF-7LJL7||||LJLJ||LJL-----J|FJL---7F-JL-7F7F7LJFJLJFJ|F-7L7|L7|F-JF7FL7|||L7LJFJFJL--7F-JLJL----7L-7LJ|||F-7LJL-7F---JLJL7J|7F--J7
|L|F7|F-FL-JFJF7FJLJLJ-F--J|-F7F7F7F7|L-----J|F---J|||L-7L--7L7LJFJFJ|FJ|L77||F-JLJL7L7FJFJF7F7LJF-------7L-7L-7|||L7L7F--JL7F7F-7FJ-F|||JLJ
F-F-JF-F7-F-JFJ||F-----JF-7|FJLJLJLJ|L7|F--7FJ|F7JFJ|L7FJF-7|FJF7L7L-JL7|FJFJ|L----7L7||LL-JLJL--JF7F7F7JL-7L--JLJ|FJFJ|-F7L||LJL|L77LL-JL.|
L..FLL-|L7L7FJFLJL--7F--JFJLJF-7F7F7L-JFJF-JL7LJL7L7|FJL7L7|||FJ|LL--7FJ|L-JFJF7F--JFJ|L7|F7F7|F--JLJLJL7F7L---7F7LJ|L7L-JL-JL7F7L-JJF|7J.JJ
L77|F|.L7L-JL-7F--77LJF7FJF--JLLJ||L-7FJFJJF7|F--JFJ|L7FJFJLJLJFJF7F-JL7L7F7|F|||F-7L7L7L-JLJL7L--7F7F-7LJL7F7FJ|L7F-7L7F7F--7LJL7F7LF7777JJ
-.LJL|77L--7F-JL-7|F7.||L-JF7F7F-J|F-J|FJF7|||||F7L7L7||FJF-7F7|FJ|L--7L7||LJFJ|LJFJFJJL7F----JF7FLJ||FJF-7LJLJ-|FJ|FJ||||L-7L7F7LJL-JL-7|F7
|L|JJL|FJLF||J.LFJLJL7|L-7.|LJLJF-JL-7|L7||||||FJ|FJFJ||L7L7LJLJL7|F-7|FJ|L7FL7L7JL7L7-FJL-7F--JL-7FJ|L-JFJF7F7FJL-JL7FJ||F7|FJ||F--7F-7|-||
|7|JFFL7JFFLJLFFJF--7||F7L-JF-7FJFF-7||FJ|||||||FJL7L7|L7L7|F7F77||L7LJ|L|FJF7L7|F7L7L7L7F-JL---7FJL-JF--JFJLJLJF----JL7|LJLJL7||L7-|||LJ-|7
7-7-L-LL|F7-F7FJFJF-JLJ|L7F-JFLJF7L7LJ|L7||||||||F7|FJL7|FJLJ||L7|L7L-7L7|L7||FJ|||FJFJL|L-7F7F-JL7F7-L7F-JF7F--J7F7F7J|L---7-LJ|FJ-LJF|.F77
JFLJ...F||.|||L-JFJF7F7|FLJF7F7FJL-JF7L7|||||||||||||F7|||F--JL7||FJF7|FJ|FJ||L7|||L7|F7|F-J||L-7FJ||F7LJF-JLJJF7FJLJL7L----JF7FJL7LLJ77FJLJ
FJ-LL7--.FLFJL---JFJ||||F--JLJ|L--7FJ|FJ||||||||||LJ|||||||F7F7||||FJ|||FJ|FJ|FJ||L7||||||F7|L7FJL7||||F7L----7||L7F-7|F7F7F-J||F-J7.JLFF--J
LJ.7L--L7FLL--7F--JFJ|LJL7F7F7L---JL7|L7||||||||LJ7FJ|LJLJ||LJ|||||L7|||L7|L7||FJL7||||||LJ||.||F-J|LJ||L7F---J|L-JL7|LJLJLJF7|LJ7-F7-L|..LJ
FF-F-7F.L-F-J|LJF--JFJ|F7LJLJL7F7F-7||FJ|||||||L7F7|FJF77FJL7FJ|||L7|LJ|FJ|FJ||L7FJLJ|||L-7|L7||L-7L7FJ|FJL----JF7F7||F7F-7FJ|L7.J.L7-F-J7J7
F7.J7L7J.FJ-J|LFJF-7|F-JL7F7F7||||FJ|LJFJ|LJ||L7LJ|||FJL-JF7||FJ||FJ|F-J|FJ|L|L7|L7F-J|L7FJ|FJ|L7F|FJL7|L7F-7F7FJLJLJLJLJ|LJFL7|JL|7L7J|---|
.|-J7-7J7-L|JJJL-JFJ|L7F7LJLJLJ|||L7L7FJFJ|FJ|LL7FJLJL7F-7||||L7||L7||F7|L7L7|FJ|FJ|F7L7||FJ|FJFJFJL-7|L7|L7LJ|L---7F7F-----7-LJJ.|J7|-77L7|
LF7-|7..JFF7F||L|LL7|FJ|L7F-7F-J|L7|FJL7|LFJFJF-JL-7F-JL7|||||FJLJFJ|||||.L7|||7LJ7||L7|||L7|L7|FJF7FJ|FJL7L-7|F7F7LJLJF----JJ7LJ-JLL-7L7LLJ
|LL7JL--L-J-LF7.|J-LJL7|FJ|FLJF-JFJLJF-JL7L7|FJF-7FJL-7FJ|||||L-7FJFJLJLJF-J||L---7LJFJ|||.||L|||FJLJL|L7FJF-JLJLJL---7L------77F-J.L|F7LF||
7LF|7JF||.J7.LJ-L-FFF-J|L7|F--JF7L--7|F7FJ7LJ|FJFJL7.FJL7LJLJL7FJL7L--7F7L--J|F7F-JF-JFJ|L7||FJ|||F-7FJFJL7|FF-------7|F7F7F--J7-.FJ7J|7F-L7
JF-LJ-|-77.LJ||-|FF7L--JFLJL-7FJL7F-J|||L-7F-J|FJF7|FJF7L----7||LFJF--J|L7F--J|||F-JF-JL|FJLJL7||||FJL7|F-JL-JF--7F--JLJ|||L--7|LJLLL.LFJFLJ
L|LF||LJFL.FF|.FF-J|-F--7F--7||F-JL77LJL7FJ|F-JL7||||FJ|F7F--J|L7L7L-7JL7LJF7FJLJ|F7|F--JL7-F-J||LJL-7||L7F7F-JF7LJF7||LLJ|F-7|77..7|.7LLJFJ
L|-FJ7|L|.FL|-|J|F7L7L-7LJF7LJ|L-7FJF-7-||7||F--J|||LJFJ||L--7L-JFJF7|F-JF7||L-7FJ|LJL7F-7|FJF-J|F7F-JLJFJ||L--JL7FJL-77LL|L7|L77-F|J7JJ|LLJ
FJ-|FLLJ.F||LLL-LJL7|F-JF-JL-7L7FJL7L7|FJL7||L7F7||L77L-JL-7FJF7|L7||||F7|||L7FJL-JF7F|L7||L7L-7|||L---7L-JL7F7F7LJF-7L7LF|FJ|FJJ.77FL-.|-F-
-L.--J7.FF-7.L|JF--JLJF7|F7F-JFJ|F-JFJLJF7||L7LJLJL-JF-----JL-J|F7LJLJ|||||L7|||F7.|L-JFJ||||F-J|||F7F-JF---J|LJ|F-J.|FJF-JL7LJ-F-|JFLJ7JF|.
||F|-F7.FL-777|JL--7F-J||||L-7L7|L-7L-7FJ||L7L---7F--JF-7F7F--7LJL7F7|LJ|||FJ|L-JL7L7F7|FJ|FJL7.||LJ|L-7L-7F7|F7|L--7||LL-7FJ7L-7||.7.LL7-L7
|JFJ7L-L|7-|.J7FJ..||F7|||L--JFJ|F7L7FJ|F||J|F7F-JL7F7|FJ||L-7L7F7LJL-7FJ|||FJF-7FJFJ||||FJL7FJFJL7|L-7|F-J||LJ|L---JLJ-JL|L-7FLF7J.|F.FJ7L7
L7J.FF7F777.FJLJ7.FJ||LJ|L---7L7LJ|FJL7L7||FJ|||F--J|LJL7||F7L7|||F7F7|L-J||L7|FJL7|FJ|||L7JLJFJF7L--7LJL-7||F7L--7F7JFL77L--JJ-FJJ--F7FFFJ.
FJF7J|F-JLJ--J.F|FL7|L7FJF---JFJF-J|F-JFJ|||FJ|||F-7|F-7LJ||L7|LJ||LJ|L-7FJL7||L7FJLJL|||FJF--JFJ|F-7L-7|FJ|||L7F7LJL--7LF||||L-|.|LLL-|-77-
--|7F77.F|.--F7---7||FJ|FJF7F7L7|F7|L7FJJ|||L7LJ|L7LJL7|-FJL7LJF-JL7JL--JL-7|||FJL---7|||L7|F-7|FJ|FJF7L7L-J||FJ||F7F--J7-F7.FJF|-F..|-|.|J|
||JL7J|7FJ.|7LL.|L-LJL-JL-JLJ|FJLJ|L7||F-J||FJ7FJFJF7FJL-JF-JF7L-7FJF7F7F-7LJ|||F7F7FJ|||FJLJFJ||FJL7|L7L-7FJ|L7||||L7|JF-7LFJFJL-L77FF.FL-J
FL7|J-7-F7.77|.FF-LL7LF-----7||-F7L-J||L-7|||-FJFJFJLJF7F7|F7|L--JL7|LJLJFJF7LJ||LJLJ|||LJ.LLL7|||F7||-L7FJL7|FJ||||FJF77.L77LJ77.L--|.J-LFL
|L-|-F|-|-|L7F|-|.LF7.|F-7F7LJL-JL--7LJFFJ|||FJFJ7L-7FJ|||LJ|L7F7F7LJF7F-JF||F-JL----7LJ|L|7||||LJ|LJL-7|L-7||L7||||L-J|F77F77L7--F7-J.|||||
-J|L7-L.-F.LLLF-.77LJ-LJFJ|L-7F--7F7|LJ-L-J|||FJF--7||FJ||F7L7LJ||L7FJ|L---J||F--7F-7|.FJLLF--JL7.|F-7FJL7FJ||FJ||||F-7LJL-JL-7J7F|L..F7--7|
|-|-|7JF||--JJ|.-FJ-|F|LL-JF-JL-7LJLJFF|-|L||LJ-L-7LJ|L7||||FJF-J|FJ|FJF--7FJ||F-JL7|L7JJ7LL-7F7|FJ|F|L-7|L7LJL7||||L7L7F7F7F-J-F-7-JJ-77FLJ
|.|LL7.-JL.|.FL7.|.77J7FJ|JL-7F7L-7-L7JLFF-LJ7.F--JF7|F|||||L7L7FJL7|L7L-7|L7|||F7FJL-JJ7-FFLLJ||L7L7L7FJL-J-|-LJ|||FJLLJLJ||JFF-7F-7J.L-J-7
|7.F-FJJF|-FJ7-F7.F.--J|.7JF-J||F7L7-L|F7L|.FF.L--7|||FJ|LJ|FJF|L-7||FJF7|L-J||LJ|L-77|||J|7J.LLJ7L7|.|L---7-L-JL||||L7||.LLJ|-|J-..7-.L|JJ.
F..|J||-77.JJ||FJ.J7-7.FJJFJF-J||L-J7|LLF.--J-FLLL||LJL7L-7||F-JF7|||L-J|L--7||J|L7FJ7-FJ7L--FJ7LF-JL7|F7F7|-L7.FLJLJL|F|7.FLLJJ|.L.L7J-|LF7
F-|.||.FJ|-7F7-7-7L7|JFLJLL7|JJ|L-7J-LJLLJ.F|.7||FJ|JFLL7FJ||L7FJ||LJ7F-JF7FJLJJL7LJJF-7F|-LFJ77FL-7FJ||||LJ7--7FJL-J-|-7--JJJ.FF7L-7JJLJ-|7
|.LF7|7|FL7FLL.J7F.L-77|.LLLJ7FJF-J.|JFFL7F|-7JL-L-J||FFJ|FJ|FJ|FJ|F--JF7|LJ.|J7.J|LF|JLF7-FL.J--JFLJFJ|LJJ-L7L||F|-LJ|LLL|-|7LJ.-7J|F7.L7|L
LF--JJLF7.FL7FF--.J7|LLL7FF7.FJFJ.L-J.|JFJ.JFL-JL7LFFJLL-JL-J|FJL-J|F7FJ|L7.FL--J7|LL.7.||F|J-7||-JJFJFJ.|JJ.L||F|..LFL..FJJLJJ7|L7-L-JF7LL7
L|7F|7..J7L|-|L7L|F7-JJLLJJF-L7L-7.|.F|L77F7-|7J.F777J|.LLJ|L||J.F-J|LJFJFJ7.L-J.JJ.|-||--L-..F-L7F-L7|-F||FF.FJ-L777LLL|J7.|J.F|.|.F7FJJ.||
-|-L|-FL7|FJ-J7|LLFL7-77|..JLLL--J-F-|L7JF|JLLL7L--.||FF.-L7L||J7L7FJJFL7L7.77|.F|-L7.L|LL7|.F--|LLJLLJ.-J-F|7JLJF7JJ|LF|LFLL.|JJ-LJLLJ|7FL.
FL7-|F77F|7-FFL-.|F7J7JFJ-7FFJJFLL.7-F-.|F77-.LJJJL|JL-L|77FFLJF7FLJL77FJFJ..F|-LJ|LLFLL7L77FFL-L7JF7F|7LJ|L||F-LLJ|.J.L7||FLFJL7.|..|FJF7J|
|LJ.--F7LJ|F7.|.-FLF7-.LF|7LL7FLF-L--7J.LFL-.|.F||..-JLLL7-JLJ.LL-7JJ|FL-J-F7.LF|F7.L|JJ.-JJ7L.LL|J.-FL7JL-JLFF-FL---J.|J-F7||LLJ.LL-FJ77|-J
|-LF7|.|..JLJ.FJ.||.--J.J.JLFLL7F77.|.|7LJ|FF.F7FLL-FF7LJ||-J.7..|L.LLLJLJ7JFJ.L|F--7|F7F||7-|FFFL---F-JFJ-LF7.FJ|7|7FL-J.FJ7L-7LJ7J.|L-J|||
|7-L|--J-L.|JLJLJ-JJ-7-F|-L-|JLJL---LJLJ-LJJ--L-F.J.F|.--LJJ..7--|L7-LLFJ|J-|JL-7J.7J-F----JLL-F7L7J-L|LJ-.L-JF|-L--J-JJ-J7JLJ--FL--L7JL-F|J
    """
        .Replace("\r", "")
        .Trim()

type Direction =
    | Up
    | Down
    | Left
    | Right

let getNextPosition (map: char array array) (fromX: int, fromY: int) (currentX: int, currentY: int) =
    let options =
        match map.[currentY].[currentX] with
        | '|' ->
            [ ((currentX, currentY - 1), [ Right ])
              ((currentX, currentY + 1), [ Left ]) ]
        | '-' ->
            [ ((currentX - 1, currentY), [ Up ])
              ((currentX + 1, currentY), [ Down ]) ]
        | 'L' ->
            [ ((currentX, currentY - 1), [])
              ((currentX + 1, currentY), [ Left; Down ]) ]
        | 'J' ->
            [ ((currentX, currentY - 1), [ Right; Down ])
              ((currentX - 1, currentY), []) ]
        | '7' ->
            [ ((currentX, currentY + 1), [])
              ((currentX - 1, currentY), [ Right; Up ]) ]
        | 'F' ->
            [ ((currentX, currentY + 1), [ Left; Up ])
              ((currentX + 1, currentY), []) ]
        | '.' -> raise (System.Exception "Why are you on the ground?")
        | 'S' -> raise (System.Exception "We should never walk from the start again")
        | _ -> raise (System.Exception "This is not a valid piece of data")

    match options with
    | [ (a, _); b ] when a = (fromX, fromY) -> b
    | [ a; _ ] -> a
    | _ -> raise (System.Exception "Invalid number of entries in the options list")

let runPart1 (startPosition: int * int) (path1: int * int) (path2: int * int) (input: string) =
    let map =
        input.Split("\n")
        |> Array.map (fun line -> line.ToCharArray())

    let rec findDistance (distanceMap: Map<int * int, int64>) (steps: int64) (currentA, fromA) (currentB, fromB) =
        if currentA = currentB then
            steps
        else if distanceMap |> Map.containsKey currentA then
            printfn "%i, %A" steps currentA
            min distanceMap.[currentA] steps
        else if distanceMap |> Map.containsKey currentB then
            min distanceMap.[currentB] steps
        else
            let (nextA, _) = getNextPosition map fromA currentA

            let (nextB, _) = getNextPosition map fromB currentB

            let newMap =
                distanceMap
                |> Map.add currentA steps
                |> Map.add currentB steps

            findDistance newMap (steps + 1L) (nextA, currentA) (nextB, currentB)

    findDistance Map.empty 1L (path1, startPosition) (path2, startPosition)

let runPart2 (startPosition: int * int) startDirection (clockWiseNextStep: int * int) (input: string) =
    let map =
        input.Split("\n")
        |> Array.map (fun line -> line.ToCharArray())

    let rec walk position previousPosition walkedPath =
        let (x, y) = position

        if map.[y].[x] = 'S' then
            walkedPath
        else
            let ((nextX, nextY), direction) =
                getNextPosition map previousPosition position

            walk
                (nextX, nextY)
                position
                ([ (position, direction) ]
                 |> List.append walkedPath)

    let walkedPath =
        walk clockWiseNextStep startPosition [ (startPosition, startDirection) ]

    let rec walkAgainAndMarkInnerEntries path marked =
        match path with
        | [] -> marked
        | ((x, y), directions) :: restOfPath ->
            let rec markAllAdjacent (x, y) marked =
                if walkedPath
                   |> List.exists (fun (p, _) -> p = (x, y))
                   || marked |> Set.contains (x, y) then
                    marked
                else
                    [ (x + 1, y)
                      (x - 1, y)
                      (x, y - 1)
                      (x, y + 1) ]
                    |> List.fold (fun a c -> a |> markAllAdjacent c) (marked |> Set.add (x, y))

            let newMarked =
                directions
                |> List.fold
                    (fun a c ->
                        match c with
                        | Up -> a |> markAllAdjacent (x, y - 1)
                        | Down -> a |> markAllAdjacent (x, y + 1)
                        | Left -> a |> markAllAdjacent (x - 1, y)
                        | Right -> a |> markAllAdjacent (x + 1, y))
                    marked

            walkAgainAndMarkInnerEntries restOfPath newMarked

    let innerEntries =
        walkAgainAndMarkInnerEntries walkedPath Set.empty

    innerEntries |> Set.count

testData1
|> runPart1 (1, 1) (1, 2) (2, 1)
|> printfn "Test data 1, part 1: %i"

testData2
|> runPart1 (0, 2) (1, 2) (0, 3)
|> printfn "Test data 2, part 1: %i"

realData
|> runPart1 (91, 41) (90, 41) (91, 40)
|> printfn "Real data, part 1: %i"

testData3
|> runPart2 (1, 1) [] (2, 1)
|> printfn "Test data 3, part 2: %i"

testData4
|> runPart2 (12, 4) [] (12, 5)
|> printfn "Test data 4, part 2: %i"

testData5
|> runPart2 (4, 0) [] (4, 1)
|> printfn "Test data 5, part 2: %i"

realData
|> runPart2 (91, 41) [] (90, 41)
|> printfn "Test data 5, part 2: %i"
