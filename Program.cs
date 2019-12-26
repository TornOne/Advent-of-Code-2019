using System;

class Program {
	static readonly Func<int>[] solutions = new Func<int>[] { Day1.Solution1, Day1.Solution2, Day2.Solution1, Day2.Solution2, Day3.Solution1, Day3.Solution2, Day4.Solution1, Day4.Solution2, Day5.Solution1, Day5.Solution2, Day6.Solution1, Day6.Solution2, Day7.Solution1, Day7.Solution2, Day8.Solution1, Day8.Solution2, Day9.Solution1, Day9.Solution2, Day10.Solution1, Day10.Solution2, Day11.Solution1, Day11.Solution2, Day12.Solution1, Day12.Solution2, Day13.Solution1, Day13.Solution2, Day14.Solution1, Day14.Solution2, Day15.Solution1, Day15.Solution2, Day16.Solution1, Day16.Solution2, Day17.Solution1, Day17.Solution2, Day18.Solution1, Day18_2.Solution2, Day19.Solution1, Day19.Solution2, Day20.Solution1, Day20.Solution2, Day21.Solution1, Day21.Solution2, Day22.Solution1, Day22.Solution2, Day23.Solution1, Day23.Solution2, Day24.Solution1, Day24.Solution2, Day25.Solution1 };

	static void Main(string[] args) {
		int i;
		if (args.Length > 0) {
			i = int.Parse(args[0]);
		} else {
			Console.WriteLine("Enter day:");
			i = int.Parse(Console.ReadLine());
		}
		i = (i - 1) * 2;

		if (i <= 0) {
			foreach (Func<int> solution in solutions) {
				Console.WriteLine(solution());
			}
			return;
		}

		Console.WriteLine(solutions[i++]());
		if (solutions.Length > i) {
			Console.WriteLine(solutions[i]());
		}
	}
}
