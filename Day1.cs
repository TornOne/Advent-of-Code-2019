using System;
using System.IO;

class Day1 {
	public static int Solution1() => Sum(Array.ConvertAll(File.ReadAllLines("input1.txt"), x => int.Parse(x) / 3 - 2));

	public static int Solution2() => Sum(Array.ConvertAll(File.ReadAllLines("input1.txt"), x => FuelCost(int.Parse(x) / 3 - 2)));

	static int FuelCost(int mass) => mass <= 0 ? 0 : mass + FuelCost(mass / 3 - 2);

	static int Sum(int[] ints) {
		int sum = 0;
		foreach (int i in ints) {
			sum += i;
		}

		return sum;
	}
}
