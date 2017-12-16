using System;
using System.Linq;

namespace _4
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintHowManyPassphrasesAreValid2();
        }

        private static void PrintTestResults()
        {
            Console.WriteLine(new Passphrase("aa bb cc dd ee") + " - (should be valid)");
            Console.WriteLine(new Passphrase("aa bb cc dd aa") + " - (should NOT be valid)");
            Console.WriteLine(new Passphrase("aa bb cc dd aaa") + " - (should be valid)");
        }

        private static void PrintTestResultsWithIsValid2(){
            Console.WriteLine(new Passphrase("abcde fghij").ToString2() + " - (should be valid)");
            System.Console.WriteLine(new Passphrase("abcde xyz ecdab").ToString2() + " - (should NOT be valid)");
            System.Console.WriteLine(new Passphrase("a ab abc abd abf abj").ToString2()  + " - (should be valid)");
            System.Console.WriteLine(new Passphrase("iiii oiii ooii oooi oooo").ToString2() + " - (should be valid)");
            System.Console.WriteLine(new Passphrase("oiii ioii iioi iiio").ToString2()  + " - (should NOT be valid)");
        }
        private static void PrintHowManyPassphrasesAreValid2()
        {
            var count = System.IO.File.ReadAllLines("passphrases.txt")
                .Select(line => new Passphrase(line))
                .Where(phrase => phrase.IsValid2())
                .Count();

            System.Console.WriteLine($"There are {count} valid passphrases according to the new rules");
        }

        private static void PrintHowManyPassphrasesAreValid()
        {
            var count = System.IO.File.ReadAllLines("passphrases.txt")
                .Select(line => new Passphrase(line))
                .Where(phrase => phrase.IsValid())
                .Count();

            System.Console.WriteLine($"There are {count} valid passphrases");
        }
    }

    public class Passphrase
    {
        public Passphrase(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }

        private string[] SplitIntoWords()
        {
            return Value.Split(" ");
        }

        public bool IsValid()
        {
            var split = SplitIntoWords();
            return split.Length == split.Distinct().Count();
        }

        public bool IsValid2()
        {
            var split = SplitIntoWords()
                .Select(w => { var arr = w.ToCharArray(); Array.Sort(arr); return arr; })
                .Select(arr => new string(arr))
                .ToArray();
            
            return split.Length == split.Distinct().Count();
        }

        public override string ToString()
        {
            var negationOrNot = IsValid() ? "" : "NOT ";
            return $"'{Value}' is {negationOrNot}a valid passphrase";
        }

        public string ToString2()
        {
            var negationOrNot = IsValid2() ? "" : "NOT ";
            return $"'{Value}' is {negationOrNot}a valid passphrase";
        }
    }
}
