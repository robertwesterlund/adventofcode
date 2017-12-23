using System;

namespace C__23_part2
{
    class Program
    {
        static void Main(string[] args)
        {
            var startNumber = 99;
            startNumber *= 100;
            startNumber += 100000;
            var highestNumberToCheck = startNumber + 17000;
            int amountOfDivisibleNumbers = 0;

            Console.WriteLine($"Checking every 17th number between {startNumber} and {highestNumberToCheck}, a total of {(int)(highestNumberToCheck - startNumber) / 17} numbers");
            var currentNumberToCheck = startNumber;
            while(true)
            {
                bool isNotDivisible = true;
                //Check if any two numbers lower than b, 
                //multiplied by each other
                //equal b
                for (var divisor = 2; divisor < currentNumberToCheck; divisor++)
                {
                    if (currentNumberToCheck % divisor == 0)
                    {
                        isNotDivisible = false;
                        break;
                    }
                }
                if (!isNotDivisible)
                {
                    amountOfDivisibleNumbers++;
                    Console.WriteLine(currentNumberToCheck + ": Have now found " + amountOfDivisibleNumbers + " numbers");
                }
                if (currentNumberToCheck == highestNumberToCheck)
                {
                    break;
                }
                currentNumberToCheck += 17;
            }
            Console.WriteLine("In the end there were " + amountOfDivisibleNumbers + " numbers found");
        }
    }
}
