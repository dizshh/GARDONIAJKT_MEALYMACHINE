using System;

namespace MealySimulator
{
    internal static class MachineTests
    {
        public static int Run()
        {
            var cases = new (string Input, string Outputs, bool Accepted)[]
            {
                ("", "", false),
                ("/", "0", false),
                ("/*", "00", false),
                ("/*/", "000", false),
                ("/**", "000", false),
                ("/**/", "0001", true),
                ("/***/", "00001", true),
                ("/*dish*/", "00000001", true),
                ("/*dis*h*/", "000000001", true),
                ("/*dish/dish*/", "0000000000001", true),
                ("/*dish", "000000", false),
                ("dish*/", "000000", false),
                ("/*dish*/h", "000000010", false),
                ("/**//**/", "00010000", false),
                ("//dish*/", "00000000", false),
                ("/*a*/", "00000", false),
                ("/*DISH*/", "00000000", false),
                ("/*d ish*/", "000000000", false),
                ("/*\n*/", "00000", false)
            };

            foreach (var test in cases)
            {
                var result = CommentMachine.Evaluate(test.Input);
                Check(result.Accepted == test.Accepted && result.Outputs == test.Outputs,
                    $"Unexpected result for {test.Input}: {result.Outputs}, {result.Accepted}");
            }

            // Exhaust all strings of length 0..6 over DISH, *, / plus invalid x.
            // The independent rule requires only allowed symbols, opening /*,
            // and the FIRST closing */ at the very end.
            int checkedCount = 0;
            CheckAllStrings("", 6, ref checkedCount);
            Console.WriteLine($"PASS: {cases.Length} sample cases and {checkedCount} exhaustive cases.");
            return 0;
        }

        private static void CheckAllStrings(string input, int remaining, ref int count)
        {
            var result = CommentMachine.Evaluate(input);
            int firstClose = input.IndexOf("*/", Math.Min(2, input.Length), StringComparison.Ordinal);
            bool allowed = true;
            foreach (char symbol in input)
                if ("dish*/".IndexOf(symbol) < 0) allowed = false;
            bool expected = allowed && input.StartsWith("/*", StringComparison.Ordinal)
                && firstClose >= 2 && firstClose == input.Length - 2;
            Check(result.Accepted == expected, $"Acceptance mismatch: {input}");
            Check(result.Outputs.Length == input.Length, $"Output length mismatch: {input}");
            Check(result.Trace.Split(" -> ", StringSplitOptions.None).Length == input.Length + 1,
                $"Trace length mismatch: {input}");
            count++;

            if (remaining == 0) return;
            foreach (char symbol in "dish*/x")
                CheckAllStrings(input + symbol, remaining - 1, ref count);
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
