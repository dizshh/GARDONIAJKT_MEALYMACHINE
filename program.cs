using System;
using System.Text;

namespace MealySimulator
{
    public enum State { q0, q1, q2, q3, q4, qD }

    public static class CommentMachine
    {
        public const string Alphabet = "dish*/";

        // Columns are d, i, s, h, *, /. Each DISH letter uses the source NFA's a moves.
        // Empty-set moves become qD; completing */ produces output 1.
        private static readonly (State Next, char Output)[,] Transitions =
        {
            { (State.qD, '0'), (State.qD, '0'), (State.qD, '0'), (State.qD, '0'), (State.qD, '0'), (State.q1, '0') },
            { (State.qD, '0'), (State.qD, '0'), (State.qD, '0'), (State.qD, '0'), (State.q2, '0'), (State.qD, '0') },
            { (State.q2, '0'), (State.q2, '0'), (State.q2, '0'), (State.q2, '0'), (State.q3, '0'), (State.q2, '0') },
            { (State.q2, '0'), (State.q2, '0'), (State.q2, '0'), (State.q2, '0'), (State.q3, '0'), (State.q4, '1') },
            { (State.qD, '0'), (State.qD, '0'), (State.qD, '0'), (State.qD, '0'), (State.qD, '0'), (State.qD, '0') },
            { (State.qD, '0'), (State.qD, '0'), (State.qD, '0'), (State.qD, '0'), (State.qD, '0'), (State.qD, '0') }
        };

        public static (State Next, char Output) Transition(State state, char symbol)
        {
            if (!Enum.IsDefined(typeof(State), state))
                throw new ArgumentOutOfRangeException(nameof(state));

            // Unsupported characters permanently invalidate this input.
            int column = Alphabet.IndexOf(symbol);
            if (column < 0) return (State.qD, '0');
            return Transitions[(int)state, column];
        }

        public static (bool Accepted, string Outputs, string Trace) Evaluate(string input)
        {
            ArgumentNullException.ThrowIfNull(input);
            State state = State.q0;
            var outputs = new StringBuilder();
            var trace = new StringBuilder(state.ToString());
            char lastOutput = '0';

            foreach (char symbol in input)
            {
                var move = Transition(state, symbol);
                state = move.Next;
                lastOutput = move.Output;
                outputs.Append(lastOutput);
                trace.Append(" -> ").Append(state);
            }

            // A previous 1 is insufficient: trailing characters invalidate a comment.
            return (lastOutput == '1', outputs.ToString(), trace.ToString());
        }
    }

    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--test")
                return MachineTests.Run();

            if (args.Length != 0)
            {
                Console.Error.WriteLine("Usage: dotnet run [-- --test]");
                return 1;
            }

            Console.WriteLine("--- DISH C-Style Comment Mealy Machine ---");
            Console.WriteLine("Inputs: d, i, s, h, *, / (lowercase only; no spaces)");
            Console.WriteLine("Output: 1 only on the / that closes a valid comment; 0 otherwise.");
            Console.WriteLine("The final output must be 1 for the entire string to be accepted.");
            Console.WriteLine("Type exit to quit. Empty input is tested and rejected.");

            while (true)
            {
                Console.Write("\nEnter a string to test: ");
                string? input = Console.ReadLine();
                if (input == null || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                var result = CommentMachine.Evaluate(input);
                Console.WriteLine($"States : {result.Trace}");
                Console.WriteLine($"Outputs: {(result.Outputs.Length == 0 ? "(empty)" : result.Outputs)}");
                Console.WriteLine($"Result : {(result.Accepted ? "ACCEPTED" : "REJECTED")}");
            }

            return 0;
        }
    }
}
