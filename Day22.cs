using System;
using System.IO;
using System.Numerics;

class Day22 {
	public static int Solution1() {
		//Create deck
		int[] activeDeck = new int[10007];
		for (int i = 0; i < activeDeck.Length; i++) {
			activeDeck[i] = i;
		}
		int[] otherDeck = new int[activeDeck.Length];

		//Do all the shuffling
		foreach (string line in File.ReadAllLines("input22.txt")) {
			if (line == "deal into new stack") {
				for (int i = 0; i < activeDeck.Length; i++) {
					otherDeck[otherDeck.Length - i - 1] = activeDeck[i];
				}
			} else if (line.Substring(0, 4) == "cut ") {
				int count = int.Parse(line.Substring(4));
				if (count > 0) {
					Array.Copy(activeDeck, count, otherDeck, 0, activeDeck.Length - count);
					Array.Copy(activeDeck, 0, otherDeck, activeDeck.Length - count, count);
				} else {
					Array.Copy(activeDeck, activeDeck.Length + count, otherDeck, 0, -count);
					Array.Copy(activeDeck, 0, otherDeck, -count, activeDeck.Length + count);
				}
			} else if (line.Substring(0, 20) == "deal with increment ") {
				int inc = int.Parse(line.Substring(20));
				for (int i = 0; i < activeDeck.Length; i++) {
					otherDeck[i * inc % activeDeck.Length] = activeDeck[i];
				}
			}
			Swap(ref activeDeck, ref otherDeck);
		}

		return Array.IndexOf(activeDeck, 2019);
	}

	static void Swap<T>(ref T active, ref T other) {
		T swap = active;
		active = other;
		other = swap;
	}

	public static int Solution2() {
		//Create the (abstract) deck
		const long deck = 119315717514047;
		const long repetitions = 101741582076661;
		BigInteger shift = 0;
		BigInteger increment = 1;

		BigInteger CurrentShiftFromZero() => shift * increment % deck;

		void ShiftBy(BigInteger shiftAdd) => shift = (EEA(deck, increment, 0, 1) * (CurrentShiftFromZero() + (shiftAdd < 0 ? deck + shiftAdd : shiftAdd)) % deck + deck) % deck;

		void IncrementBy(BigInteger inc) => increment = increment * inc % deck;

		//Do all the shuffling
		foreach (string line in File.ReadAllLines("input22.txt")) {
			if (line == "deal into new stack") {
				ShiftBy(-1);
				IncrementBy(deck - 1);
			} else if (line.Substring(0, 4) == "cut ") {
				ShiftBy(int.Parse(line.Substring(4)));
			} else if (line.Substring(0, 20) == "deal with increment ") {
				IncrementBy(int.Parse(line.Substring(20)));
			}
		}

		BigInteger finalIncrement = BigInteger.ModPow(increment, repetitions, deck); //Find final increment
		BigInteger finalShift = CurrentShiftFromZero() * (finalIncrement - 1) * (EEA(increment - 1, deck, 1, 0) + deck) % deck; //Convert shift to modular distance from zero and find the final shift from that using a geometric series of the increment
		increment = finalIncrement;
		shift = (EEA(deck, increment, 0, 1) * finalShift % deck + deck) % deck; //Convert shift back to the correct number

		ShiftBy(2020); //Shift by position to find the card in that position
		Console.WriteLine(shift);
		return 0;
	}

	//Extended Euclidean algorithm
	static BigInteger EEA(BigInteger r1, BigInteger r2, BigInteger b1, BigInteger b2) => r2 == 0 ? b1 : EEA(r2, r1 % r2, b2, b1 - r1 / r2 * b2);
}
