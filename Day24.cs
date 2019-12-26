using System;
using System.Collections.Generic;
using System.IO;

class Day24 {
	public static int Solution1() {
		HashSet<Grid> states = new HashSet<Grid>();
		Grid eris = new Grid("input24.txt");

		while (states.Add(eris)) {
			eris.Iterate();
		}

		return eris.biodiversity;
	}

	public static int Solution2() {
		RecursiveGrid eris = new RecursiveGrid("input24.txt");
		for (int i = 0; i < 200; i++) {
			eris.Iterate();
		}
		return eris.bugCount;
	}

	class Grid : IEquatable<Grid> {
		bool[,] bugs;
		public int biodiversity {
			get {
				int sum = 0;
				for (int y = 0; y < bugs.GetLength(0); y++) {
					for (int x = 0; x < bugs.GetLength(1); x++) {
						if (bugs[y, x]) {
							sum |= 1 << (y * 5 + x); //Hardcoded assumption that we have a 5x5 grid
						}
					}
				}
				return sum;
			}
		}

		public Grid(string fileName) {
			string[] lines = File.ReadAllLines(fileName);
			bugs = new bool[lines.Length, lines[0].Length];

			for (int y = 0; y < lines.Length; y++) {
				for (int x = 0; x < lines[y].Length; x++) {
					bugs[y, x] = lines[y][x] == '#';
				}
			}
		}

		public Grid(bool[,] bugs) {
			this.bugs = bugs;
		}

		public void Iterate() {
			bool[,] newBugs = new bool[bugs.GetLength(0), bugs.GetLength(1)];
			for (int y = 0; y < bugs.GetLength(0); y++) {
				for (int x = 0; x < bugs.GetLength(1); x++) {
					int count = (y > 0 && bugs[y - 1, x] ? 1 : 0) + (x > 0 && bugs[y, x - 1] ? 1 : 0) + (y + 1 < bugs.GetLength(0) && bugs[y + 1, x] ? 1 : 0) + (x + 1 < bugs.GetLength(1) && bugs[y, x + 1] ? 1 : 0);
					newBugs[y, x] = count == 1 || !bugs[y, x] && count == 2;
				}
			}
			bugs = newBugs;
		}

		public override bool Equals(object obj) => Equals(obj as Grid);
		public bool Equals(Grid other) => !(other is null) && biodiversity == other.biodiversity;
		public override int GetHashCode() => biodiversity;

		public static bool operator ==(Grid left, Grid right) => left.Equals(right);
		public static bool operator !=(Grid left, Grid right) => !left.Equals(right);
	}

	class RecursiveGrid {
		struct Position : IEquatable<Position> {
			public readonly int x, y, layer;

			public Position(int x, int y, int layer = 0) {
				this.x = x;
				this.y = y;
				this.layer = layer;
			}

			//Unused
			public bool IsAdjacent(Position other) => layer == other.layer && Math.Abs(x - other.x) + Math.Abs(y - other.y) == 1 || layer == other.layer - 1 && (other.y == 2 && (x == 4 && other.x == 3 || x == 0 && other.x == 1) || other.x == 2 && (y == 4 && other.y == 3 || y == 0 && other.y == 1)) || layer == other.layer + 1 && (y == 2 && (other.x == 4 && x == 3 || other.x == 0 && x == 1) || x == 2 && (other.y == 4 && y == 3 || other.y == 0 && y == 1));

			public IEnumerable<Position> GetAdjacent() {
				//left-right
				if (x == 0) { //left edge
					yield return new Position(1, 2, layer + 1); //left
					yield return new Position(1, y, layer); //right
				} else if (x == 4) { //right edge
					yield return new Position(3, 2, layer + 1); //right
					yield return new Position(3, y, layer); //left
				} else if (y != 2) { //most other tiles
					yield return new Position(x - 1, y, layer); //left
					yield return new Position(x + 1, y, layer); //right
				} else if (x == 1) { //middle row left
					yield return new Position(0, 2, layer); //left
					for (int y = 0; y < 5; y++) { //all the right
						yield return new Position(0, y, layer - 1);
					}
				} else { //middle row right
					yield return new Position(4, 2, layer); //right
					for (int y = 0; y < 5; y++) { //all the left
						yield return new Position(4, y, layer - 1);
					}
				}

				//top-bottom
				if (y == 0) { //top edge
					yield return new Position(2, 1, layer + 1); //top
					yield return new Position(x, 1, layer); //bottom
				} else if (y == 4) { //bottom edge
					yield return new Position(2, 3, layer + 1); //bottom
					yield return new Position(x, 3, layer); //top
				} else if (x != 2) { //most other tiles
					yield return new Position(x, y - 1, layer); //top
					yield return new Position(x, y + 1, layer); //bottom
				} else if (y == 1) { //middle column top
					yield return new Position(2, 0, layer); //top
					for (int x = 0; x < 5; x++) { //all the bottom
						yield return new Position(x, 0, layer - 1);
					}
				} else { //middle column bottom
					yield return new Position(2, 4, layer); //bottom
					for (int x = 0; x < 5; x++) { //all the top
						yield return new Position(x, 4, layer - 1);
					}
				}
			}

			public override bool Equals(object obj) => obj is Position position && Equals(position);
			public bool Equals(Position other) => x == other.x && y == other.y && layer == other.layer;
			public override int GetHashCode() => x << 28 | y << 24 | layer;

			public override string ToString() => $"({x}, {y}, {layer})";
		}

		HashSet<Position> bugs = new HashSet<Position>();
		public int bugCount => bugs.Count;

		public RecursiveGrid(string fileName) {
			string[] lines = File.ReadAllLines(fileName);

			for (int y = 0; y < lines.Length; y++) {
				for (int x = 0; x < lines[y].Length; x++) {
					if (lines[y][x] == '#') {
						bugs.Add(new Position(x, y));
					}
				}
			}
		}

		public void Iterate() {
			//Add all tiles with bugs and their surroundings into a new set to check
			HashSet<Position> toCheck = new HashSet<Position>(bugs.Count * 4);
			foreach (Position tile in bugs) {
				toCheck.Add(tile);
				foreach (Position adjacent in tile.GetAdjacent()) {
					toCheck.Add(adjacent);
				}
			}

			//Populate the newBugs and replace the old list
			HashSet<Position> newBugs = new HashSet<Position>(bugs.Count * 2);
			foreach (Position tile in toCheck) {
				int count = 0;
				foreach (Position adjacent in tile.GetAdjacent()) {
					if (bugs.Contains(adjacent)) {
						count++;
					}
				}

				if (count == 1 || !bugs.Contains(tile) && count == 2) {
					newBugs.Add(tile);
				}
			}
			bugs = newBugs;
		}
	}
}
