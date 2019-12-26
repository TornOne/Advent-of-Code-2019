using System;
using System.Collections.Generic;
using System.IO;

class Day10 {
	public static int Solution1() => Solution0(out _, out _, out _);

	public static int Solution2() {
		Solution0(out int stationY, out int stationX, out bool[,] map);
		int height = map.GetLength(0);
		int width = map.GetLength(1);

		LinkedList<Position> asteroids = new LinkedList<Position>();
		void AddAsteroid(Position asteroid) {
			LinkedListNode<Position> currentNode = asteroids.First;
			if (currentNode is null) {
				asteroids.AddFirst(asteroid);
				return;
			}

			while (asteroid > currentNode.Value) {
				currentNode = currentNode.Next;
				if (currentNode is null) {
					asteroids.AddLast(asteroid);
					return;
				}
			}

			if (currentNode.Value > asteroid) {
				asteroids.AddBefore(currentNode, asteroid);
			} else {
				if (asteroid.distance < currentNode.Value.distance) {
					Position temp = currentNode.Value;
					currentNode.Value = asteroid;
					asteroid = temp;
				}
				asteroid.layer++;
				AddAsteroid(asteroid);
			}
		}

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if (!map[y, x] || y == stationY && x == stationX) {
					continue;
				}

				AddAsteroid(new Position(y - stationY, x - stationX));
			}
		}

		LinkedListNode<Position> node = asteroids.First;
		for (int i = 1; i < 200; i++) {
			node = node.Next;
		}
		int realY = node.Value.offsetY * node.Value.distance + stationY;
		int realX = node.Value.offsetX * node.Value.distance + stationX;

		return realX * 100 + realY;
	}

	static int Solution0(out int stationY, out int stationX, out bool[,] map) {
		string[] allLines = File.ReadAllLines("input10.txt");
		int height = allLines.Length;
		int width = allLines[0].Length;
		map = new bool[height, width];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				map[y, x] = allLines[y][x] == '#';
			}
		}

		int mostAsteroids = 0;
		stationY = -1;
		stationX = -1;
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if (!map[y, x]) {
					continue;
				}

				bool[,] newMap = (bool[,])map.Clone();
				for (int y2 = 0; y2 < height; y2++) {
					for (int x2 = 0; x2 < width; x2++) {
						if (!newMap[y2, x2] || y2 == y && x2 == x) {
							continue;
						}

						int offsetY = y2 - y;
						int offsetX = x2 - x;
						if (offsetY == 0) {
							offsetX = Math.Sign(offsetX);
						} else if (offsetX == 0) {
							offsetY = Math.Sign(offsetY);
						} else {
							int GCFYX = GCF(Math.Abs(offsetY), Math.Abs(offsetX));
							offsetY /= GCFYX;
							offsetX /= GCFYX;
						}
						for (int newY = y2 + offsetY, newX = x2 + offsetX; newY >= 0 && newY < height && newX >= 0 && newX < width; newY += offsetY, newX += offsetX) {
							newMap[newY, newX] = false;
						}
					}
				}

				int asteroids = 0;
				foreach (bool isAsteroid in newMap) {
					if (isAsteroid) {
						asteroids++;
					}
				}

				if (asteroids > mostAsteroids) {
					stationY = y;
					stationX = x;
					mostAsteroids = asteroids;
				}
			}
		}

		return mostAsteroids - 1;
	}

	static int GCF(int a, int b) {
		while (a != b) {
			if (a > b) {
				a -= b;
			} else {
				b -= a;
			}
		}
		return a;
	}

	class Position {
		public readonly int offsetY, offsetX, distance;
		public int layer = 0;

		public Position(int offsetY, int offsetX) {
			if (offsetY == 0) {
				this.offsetY = 0;
				this.offsetX = Math.Sign(offsetX);
				distance = offsetX;
			} else if (offsetX == 0) {
				this.offsetX = 0;
				this.offsetY = Math.Sign(offsetY);
				distance = offsetY;
			} else {
				int GCFYX = GCF(Math.Abs(offsetY), Math.Abs(offsetX));
				this.offsetY = offsetY / GCFYX;
				this.offsetX = offsetX / GCFYX;
				distance = GCFYX;
			}
		}

		double Angle => (Math.Atan2(offsetY, offsetX) + 2.5 * Math.PI) % (2 * Math.PI);

		public static bool operator ==(Position a, Position b) => a.offsetY == b.offsetY && a.offsetX == b.offsetX && a.layer == b.layer;
		public static bool operator !=(Position a, Position b) => !(a == b);

		public static bool operator >(Position a, Position b) => a.layer > b.layer || a != b && a.Angle > b.Angle;
		public static bool operator <(Position a, Position b) => a.layer < b.layer || a != b && a.Angle < b.Angle;

		public static bool operator >=(Position a, Position b) => !(a < b);
		public static bool operator <=(Position a, Position b) => !(a > b);

		public override bool Equals(object obj) => obj is Position && this == (Position)obj;
		public override int GetHashCode() => offsetY * offsetX * layer;
	}
}
