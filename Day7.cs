using System;
using System.Collections.Generic;
using System.Numerics;

class Day7 {
	public static int Solution1() {
		Queue<BigInteger> outputSignals = new Queue<BigInteger>();
		return Solution(new HashSet<int>() { 0, 1, 2, 3, 4 }, outputSignals, (_, amplifierE) => {
			amplifierE.SetOutput(outputSignals);
			amplifierE.Run();
		});
	}

	public static int Solution2() {
		List<BigInteger> outputSignals = new List<BigInteger>();
		return Solution(new HashSet<int>() { 5, 6, 7, 8, 9 }, outputSignals, (amplifierA, amplifierE) => {
			amplifierE.SetOutput(amplifierA);
			amplifierE.Run();
			outputSignals.Add(amplifierE.OutQueue.Peek());
		});
	}

	static int Solution(HashSet<int> setA, IEnumerable<BigInteger> outputSignals, Action<Intcode, Intcode> action) {
		Intcode amplifier = new Intcode("input7.txt");

		foreach (int a in setA) {
			HashSet<int> setB = Except(setA, a);
			foreach (int b in setB) {
				HashSet<int> setC = Except(setB, b);
				foreach (int c in setC) {
					HashSet<int> setD = Except(setC, c);
					foreach (int d in setD) {
						HashSet<int> setE = Except(setD, d);
						foreach (int e in setE) {
							Intcode amplifierA = amplifier.Duplicate();
							amplifierA.SetInput(new Queue<BigInteger>(new BigInteger[] { a, 0 }));
							amplifierA.SetOutput(new Queue<BigInteger>(new BigInteger[] { b }));
							Intcode amplifierB = amplifier.Duplicate();
							amplifierB.SetInput(amplifierA);
							amplifierB.SetOutput(new Queue<BigInteger>(new BigInteger[] { c }));
							Intcode amplifierC = amplifier.Duplicate();
							amplifierC.SetInput(amplifierB);
							amplifierC.SetOutput(new Queue<BigInteger>(new BigInteger[] { d }));
							Intcode amplifierD = amplifier.Duplicate();
							amplifierD.SetInput(amplifierC);
							amplifierD.SetOutput(new Queue<BigInteger>(new BigInteger[] { e }));
							Intcode amplifierE = amplifier.Duplicate();
							amplifierE.SetInput(amplifierD);
							action(amplifierA, amplifierE);
						}
					}
				}
			}
		}

		int highestSignal = int.MinValue;
		foreach (int signal in outputSignals) {
			highestSignal = Math.Max(highestSignal, signal);
		}
		return highestSignal;
	}

	static HashSet<int> Except(HashSet<int> a, int b) {
		HashSet<int> c = new HashSet<int>(a);
		c.Remove(b);
		return c;
	}
}
