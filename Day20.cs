using System;
using System.Collections.Generic;
using System.IO;

class Day20 {
	public static int Solution1() => new Grid("input20.txt").Path();

	public static int Solution2() => new RecursiveGrid("input20.txt").Path();

	class Grid {
		readonly bool[,] grid;
		readonly Dictionary<Vector2, Vector2> portals;
		readonly Vector2 start, end;

		public Grid(string fileName) {
			portals = new Dictionary<Vector2, Vector2>();
			Dictionary<string, Vector2> portalEnds = new Dictionary<string, Vector2>();

			string[] lines = File.ReadAllLines(fileName);
			grid = new bool[lines.Length - 4, lines[0].Length - 4];

			for (int y = 0; y < lines.Length - 1; y++) {
				for (int x = 0; x < lines[y].Length - 1; x++) {
					char c = lines[y][x];

					if (c == '.') {
						grid[y - 2, x - 2] = true;
					} else if (c >= 'A' && c <= 'Z') {
						char d = lines[y + 1][x];
						Vector2 thisEnd = new Vector2(x - 2, y == 0 || lines[y - 1][x] != '.' ? y : y - 3);
						if (!(d >= 'A' && d <= 'Z')) {
							d = lines[y][x + 1];
							thisEnd = new Vector2(x == 0 || lines[y][x - 1] != '.' ? x : x - 3, y - 2);
							if (!(d >= 'A' && d <= 'Z')) {
								continue;
							}
						}

						string name = new string(new char[] { c, d });
						if (name == "AA") {
							start = thisEnd;
							continue;
						} else if (name == "ZZ") {
							end = thisEnd;
							continue;
						} else if (portalEnds.TryGetValue(name, out Vector2 otherEnd)) {
							portalEnds.Remove(name);
							portals[thisEnd] = otherEnd;
							portals[otherEnd] = thisEnd;
						} else {
							portalEnds[name] = thisEnd;
						}
					}
				}
			}
		}

		//Breadth-first search
		public int Path() {
			Dictionary<Vector2, int> enqueued = new Dictionary<Vector2, int>();
			Queue<Vector2> tilesToCheck = new Queue<Vector2>();
			tilesToCheck.Enqueue(start);
			enqueued[start] = 0;

			while (tilesToCheck.Count > 0) {
				Vector2 tile = tilesToCheck.Dequeue();
				
				if (portals.TryGetValue(tile, out Vector2 otherEnd) && !enqueued.ContainsKey(otherEnd)) {
					tilesToCheck.Enqueue(otherEnd);
					enqueued[otherEnd] = enqueued[tile] + 1;
				}

				foreach (Vector2 neighbourTile in new Vector2[] { tile + Vector2.up, tile + Vector2.down, tile + Vector2.left, tile + Vector2.right }) {
					if (neighbourTile.Equals(end)) {
						return enqueued[tile] + 1;
					}
					if (neighbourTile.x >= 0 && neighbourTile.y >= 0 && neighbourTile.x < grid.GetLength(1) && neighbourTile.y < grid.GetLength(0) && grid[neighbourTile.y, neighbourTile.x] && !enqueued.ContainsKey(neighbourTile)) {
						tilesToCheck.Enqueue(neighbourTile);
						enqueued[neighbourTile] = enqueued[tile] + 1;
					}
				}
			}

			return -1;
		}
	}

	class RecursiveGrid {
		struct Position : IEquatable<Position> {
			public Vector2 coords;
			public int layer;

			public Position(int x, int y, int layer = 0) : this(new Vector2(x, y), layer) { }

			public Position(Vector2 coords, int layer = 0) {
				this.coords = coords;
				this.layer = layer;
			}

			public static Position operator +(Position a, Vector2 b) => new Position(a.coords + b, a.layer);
			public static Position operator -(Position a, Vector2 b) => new Position(a.coords - b, a.layer);
			public static Position operator +(Position a, int b) => new Position(a.coords, a.layer + b);
			public static Position operator -(Position a, int b) => new Position(a.coords, a.layer - b);
			public static bool operator ==(Position a, Position b) => a.Equals(b);
			public static bool operator !=(Position a, Position b) => !a.Equals(b);

			public override bool Equals(object obj) => obj is Position position && Equals(position);
			public bool Equals(Position other) => coords.x == other.coords.x && coords.y == other.coords.y && layer == other.layer;
			public override int GetHashCode() => layer << 16 | coords.x << 8 | coords.y;
		}

		readonly bool[,] grid;
		readonly Dictionary<Vector2, Vector2> outerPortals;
		readonly Dictionary<Vector2, Vector2> innerPortals;
		readonly Position start, end;

		Position? GetOtherPortalPosition(Position position) {
			if (position.layer > 0 && outerPortals.TryGetValue(position.coords, out Vector2 otherEnd)) {
				return new Position(otherEnd, position.layer - 1);
			}
			if (innerPortals.TryGetValue(position.coords, out otherEnd)) {
				return new Position(otherEnd, position.layer + 1);
			}
			return null; //Layer 0 outer portal encountered
		}

		public RecursiveGrid(string fileName) {
			outerPortals = new Dictionary<Vector2, Vector2>();
			innerPortals = new Dictionary<Vector2, Vector2>();
			Dictionary<string, Vector2> portalEnds = new Dictionary<string, Vector2>();

			string[] lines = File.ReadAllLines(fileName);
			grid = new bool[lines.Length - 4, lines[0].Length - 4];

			for (int y = 0; y < lines.Length - 1; y++) {
				for (int x = 0; x < lines[y].Length - 1; x++) {
					char c = lines[y][x];

					if (c == '.') {
						grid[y - 2, x - 2] = true;
					} else if (c >= 'A' && c <= 'Z') {
						char d = lines[y + 1][x];
						Vector2 thisEnd = new Vector2(x - 2, y == 0 || lines[y - 1][x] != '.' ? y : y - 3);
						if (!(d >= 'A' && d <= 'Z')) {
							d = lines[y][x + 1];
							thisEnd = new Vector2(x == 0 || lines[y][x - 1] != '.' ? x : x - 3, y - 2);
							if (!(d >= 'A' && d <= 'Z')) {
								continue;
							}
						}

						string name = new string(new char[] { c, d });
						if (name == "AA") {
							start = new Position(thisEnd);
							continue;
						} else if (name == "ZZ") {
							end = new Position(thisEnd);
							continue;
						} else if (portalEnds.TryGetValue(name, out Vector2 otherEnd)) {
							portalEnds.Remove(name);
							if (thisEnd.x == 0 || thisEnd.y == 0 || thisEnd.x == grid.GetLength(1) - 1 || thisEnd.y == grid.GetLength(0) - 1) {
								outerPortals[thisEnd] = otherEnd;
								innerPortals[otherEnd] = thisEnd;
							} else {
								innerPortals[thisEnd] = otherEnd;
								outerPortals[otherEnd] = thisEnd;
							}
						} else {
							portalEnds[name] = thisEnd;
						}
					}
				}
			}
		}

		//Breadth-first search
		public int Path() {
			Dictionary<Position, int> enqueued = new Dictionary<Position, int>();
			Queue<Position> tilesToCheck = new Queue<Position>();
			tilesToCheck.Enqueue(start);
			enqueued[start] = 0;

			while (tilesToCheck.Count > 0) {
				Position tile = tilesToCheck.Dequeue();

				Position? otherEnd = GetOtherPortalPosition(tile);
				if (otherEnd.HasValue && !enqueued.ContainsKey(otherEnd.Value)) {
					tilesToCheck.Enqueue(otherEnd.Value);
					enqueued[otherEnd.Value] = enqueued[tile] + 1;
				}

				foreach (Position neighbourTile in new Position[] { tile + Vector2.up, tile + Vector2.down, tile + Vector2.left, tile + Vector2.right }) {
					if (neighbourTile.Equals(end)) {
						return enqueued[tile] + 1;
					}
					if (neighbourTile.coords.x >= 0 && neighbourTile.coords.y >= 0 && neighbourTile.coords.x < grid.GetLength(1) && neighbourTile.coords.y < grid.GetLength(0) && grid[neighbourTile.coords.y, neighbourTile.coords.x] && !enqueued.ContainsKey(neighbourTile)) {
						tilesToCheck.Enqueue(neighbourTile);
						enqueued[neighbourTile] = enqueued[tile] + 1;
					}
				}
			}

			return -1;
		}
	}
}
