using System;
using System.IO;

class Day16 {
	public static int Solution1() {
		int[] oldSignal = Array.ConvertAll(File.ReadAllText("input16.txt").ToCharArray(), c => c - 48);
		int[] newSignal = new int[oldSignal.Length];
		int[] basePattern = new int[] { 0, 1, 0, -1 };

		for (int a = 0; a < 100; a++) {
			for (int i = 0; i < oldSignal.Length; i++) {
				int lastDigit = 0;
				for (int j = 0; j < oldSignal.Length; j++) {
					lastDigit += oldSignal[j] * basePattern[(j + 1) / (i + 1) % 4];
				}
				newSignal[i] = Math.Abs(lastDigit % 10);
			}
			Array.Copy(newSignal, oldSignal, oldSignal.Length);
		}

		return newSignal[0] * 10000000 + newSignal[1] * 1000000 + newSignal[2] * 100000 + newSignal[3] * 10000 + newSignal[4] * 1000 + newSignal[5] * 100 + newSignal[6] * 10 + newSignal[7];
	}

	public static int Solution2() {
		int[] original = Array.ConvertAll(File.ReadAllText("input16.txt").ToCharArray(), c => c - 48);
		int pos = original[0] * 1000000 + original[1] * 100000 + original[2] * 10000 + original[3] * 1000 + original[4] * 100 + original[5] * 10 + original[6];

		//Compose the part of the signal we actually need
		int[] signal = new int[original.Length * 10000 - pos];
		for (int i = 1; i <= signal.Length; i++) {
			signal[signal.Length - i] = original[(original.Length * 10000 - i) % original.Length];
		}

		for (int i = 0; i < 100; i++) {
			for (int j = signal.Length - 2; j >= 0; j--) {
				signal[j] = (signal[j + 1] + signal[j]) % 10;
			}
		}

		return signal[0] * 10000000 + signal[1] * 1000000 + signal[2] * 100000 + signal[3] * 10000 + signal[4] * 1000 + signal[5] * 100 + signal[6] * 10 + signal[7];
	}
}
