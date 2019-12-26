using System;

class Day4 {
	const int lowerBound = 254032;
	const int higherBound = 789860;

	public static int Solution1() => CheckAll((a, b, c, d, e, f) => {
		int n = ((((a * 10 + b) * 10 + c) * 10 + d) * 10 + e) * 10 + f;
		return (a == b || b == c || c == d || d == e || e == f) && n > lowerBound && n < higherBound;
	});

	public static int Solution2() => CheckAll((a, b, c, d, e, f) => {
		int n = ((((a * 10 + b) * 10 + c) * 10 + d) * 10 + e) * 10 + f;
		return (a == b && b != c || b == c && a != b && c != d || c == d && b != c && d != e || d == e && c != d && e != f || e == f && d != e) && n > lowerBound && n < higherBound;
	});

	static int CheckAll(Func<int, int, int, int, int, int, bool> test) {
		int solutions = 0;
		for (int a = 2; a <= 7; a++) {
			for (int b = a; b <= 9; b++) {
				for (int c = b; c <= 9; c++) {
					for (int d = c; d <= 9; d++) {
						for (int e = d; e <= 9; e++) {
							for (int f = e; f <= 9; f++) {
								if (test(a, b, c, d, e, f)) {
									solutions++;
								}
							}
						}
					}
				}
			}
		}

		return solutions;
	}
}
