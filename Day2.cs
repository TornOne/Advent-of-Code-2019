using System;
using System.IO;

class Day2 {
	public static int Solution1() => RunProgram(12, 2);

	public static int Solution2() {
		for (int noun = 0; noun < 100; noun++) {
			for (int verb = 0; verb < 100; verb++) {
				if (RunProgram(noun, verb) == 19690720) {
					return 100 * noun + verb;
				}
			}
		}

		return -1;
	}

	static int RunProgram(int noun, int verb) {
		int[] memory = Array.ConvertAll(File.ReadAllText("input2.txt").Split(','), int.Parse);
		memory[1] = noun;
		memory[2] = verb;
		for (int i = 0; memory[i] != 99; i += 4) {
			int op1 = memory[memory[i + 1]];
			int op2 = memory[memory[i + 2]];
			memory[memory[i + 3]] = memory[i] == 1 ? op1 + op2 : op1 * op2;
		}
		return memory[0];
	}
}
