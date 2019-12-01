param(
	[Parameter(ValueFromPipeline = $true)]
	[string]$ElfCode
)
BEGIN
{
	$LineNumber = 0
}
PROCESS
{
	if ($ElfCode.StartsWith('#ip'))
	{
		return $ElfCode
	}
	$split = $ElfCode.Split(' ')
	function GetOpText
	{
		$firstParam = $split[1]
		$secondParam = $split[2]
		switch ($split[0])
		{
			"seti" { $firstParam }
			"setr" { "r[$firstParam]" }
			"bani" { "r[$firstParam] & $secondParam" }
			"eqri" { "r[$firstParam] == $secondParam ? 1 : 0" }
			"addr" { "r[$firstParam] + r[$secondParam]" }
			"addi" { "r[$firstParam] + $secondParam" }
			"bori" { "r[$firstParam] | $secondParam" }
			"muli" { "r[$firstParam] * $secondParam" }
			"gtir" { "$firstParam > r[$secondParam] ? 1 : 0" }
			"gtrr" { "r[$firstParam] > r[$secondParam] ? 1 : 0" }
			"eqrr" { "r[$firstParam] == r[$secondParam] ? 1 : 0" }
			default { throw "Can't handle '$($split[0])'" }
		}
	}
	
	"/*$($LineNumber.ToString("00"))*/ r[$($split[3])] = $(GetOpText);"
	$LineNumber++ | Out-Null
}