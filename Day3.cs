using System;
using System.Collections.Generic;
using System.IO;

class Day3 {
	public static int Solution1() {
		string[] wires = File.ReadAllLines("input3.txt");
		HashSet<Vector2> grid = new HashSet<Vector2>();

		void LayAllWire(string wire, RefAction<Vector2, Vector2, int> LayWire) {
			Vector2 wireEnd = new Vector2();

			foreach (string direction in wire.Split(',')) {
				int distance = int.Parse(direction.Substring(1));

				switch (direction[0]) {
					case 'U':
						LayWire(ref wireEnd, new Vector2(0, 1), distance);
						break;
					case 'L':
						LayWire(ref wireEnd, new Vector2(-1, 0), distance);
						break;
					case 'D':
						LayWire(ref wireEnd, new Vector2(0, -1), distance);
						break;
					case 'R':
						LayWire(ref wireEnd, new Vector2(1, 0), distance);
						break;
				}
			}
		}

		//Wire 1
		LayAllWire(wires[0], (ref Vector2 wireEnd, Vector2 dir, int distance) => {
			for (int i = 0; i < distance; i++) {
				wireEnd += dir;
				grid.Add(wireEnd);
			}
		});

		//Wire 2
		int closestIntersection = int.MaxValue;
		LayAllWire(wires[1], (ref Vector2 wireEnd, Vector2 dir, int distance) => {
			for (int i = 0; i < distance; i++) {
				wireEnd += dir;
				if (grid.Contains(wireEnd)) {
					closestIntersection = Math.Min(closestIntersection, Math.Abs(wireEnd.x) + Math.Abs(wireEnd.y));
				}
			}
		});

		return closestIntersection;
	}

	public static int Solution2() {
		string[] wires = File.ReadAllLines("input3.txt");
		Dictionary<Vector2, int> grid = new Dictionary<Vector2, int>();
		int totalDist = 0;

		void LayAllWire(string wire, RefAction<Vector2, Vector2, int> LayWire) {
			Vector2 wireEnd = new Vector2();

			foreach (string direction in wire.Split(',')) {
				int distance = int.Parse(direction.Substring(1));

				switch (direction[0]) {
					case 'U':
						LayWire(ref wireEnd, new Vector2(0, 1), distance);
						break;
					case 'L':
						LayWire(ref wireEnd, new Vector2(-1, 0), distance);
						break;
					case 'D':
						LayWire(ref wireEnd, new Vector2(0, -1), distance);
						break;
					case 'R':
						LayWire(ref wireEnd, new Vector2(1, 0), distance);
						break;
				}
			}
		}

		//Wire 1
		LayAllWire(wires[0], (ref Vector2 wireEnd, Vector2 dir, int distance) => {
			for (int i = 0; i < distance; i++) {
				wireEnd += dir;
				totalDist++;
				if (!grid.ContainsKey(wireEnd)) {
					grid[wireEnd] = totalDist;
				}
			}
		});

		//Wire 2
		int closestIntersection = int.MaxValue;
		totalDist = 0;
		LayAllWire(wires[1], (ref Vector2 wireEnd, Vector2 dir, int distance) => {
			for (int i = 0; i < distance; i++) {
				wireEnd += dir;
				totalDist++;
				if (grid.ContainsKey(wireEnd)) {
					closestIntersection = Math.Min(closestIntersection, grid[wireEnd] + totalDist);
				}
			}
		});

		return closestIntersection;
	}

	delegate void RefAction<T1, T2, T3>(ref T1 arg1, T2 arg2, T3 arg3);
}

struct Vector2 : IEquatable<Vector2> {
	public readonly int x, y;

	public Vector2(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public static readonly Vector2 up = new Vector2(0, -1);
	public static readonly Vector2 down = new Vector2(0, 1);
	public static readonly Vector2 left = new Vector2(-1, 0);
	public static readonly Vector2 right = new Vector2(1, 0);

	public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
	public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
	public static Vector2 operator -(Vector2 a) => new Vector2(-a.x, -a.y);
	public static bool operator ==(Vector2 a, Vector2 b) => a.Equals(b);
	public static bool operator !=(Vector2 a, Vector2 b) => !a.Equals(b);

	public override bool Equals(object obj) => obj is Vector2 vector && Equals(vector);
	public bool Equals(Vector2 other) => x == other.x && y == other.y;
	public override int GetHashCode() => x << 16 ^ y;

	public override string ToString() => $"({x}, {y})";
}
