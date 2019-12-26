using System;
using System.Collections.Generic;
using System.IO;

class Day18_2 {
	static readonly Dictionary<GridState, int> states = new Dictionary<GridState, int>();

	public static int Solution2() {
		int shortestPath = int.MaxValue;
		Queue<Grid> toCheck = new Queue<Grid>();
		toCheck.Enqueue(new Grid("input18-2.txt"));
		states[new GridState(toCheck.Peek())] = 0;

		//Breadth-first search
		while (toCheck.Count > 0) {
			Grid grid = toCheck.Dequeue();
			int oldDistance = states[new GridState(grid)];

			if (grid.keyLocs.Count == 0) {
				shortestPath = Math.Min(shortestPath, oldDistance);
				continue;
			}

			grid.PathAll();
			foreach (char key in grid.keyLocs.Keys) {
				if (grid.path.TryGetValue(grid.keyLocs[key], out int distance)) {
					//Duplicate grid, change position, remove key and door from dictionaries and map
					Grid newGrid = new Grid(grid);
					Vector2 newPos = newGrid.keyLocs[key];
					if (newPos.y < 40) {
						if (newPos.x < 40) {
							newGrid.pos1 = newPos;
						} else {
							newGrid.pos2 = newPos;
						}
					} else {
						if (newPos.x < 40) {
							newGrid.pos3 = newPos;
						} else {
							newGrid.pos4 = newPos;
						}
					}
					newGrid.grid[newPos.y, newPos.x] = '.';
					newGrid.keyLocs.Remove(key);
					Vector2 doorPos = newGrid.doorLocs[key];
					newGrid.grid[doorPos.y, doorPos.x] = '.';
					newGrid.doorLocs.Remove(key);

					GridState state = new GridState(newGrid);
					int newDistance = oldDistance + distance;
					if (!states.TryGetValue(state, out int totalDist) || totalDist > newDistance) {
						states[state] = newDistance;
						toCheck.Enqueue(newGrid);
					}
				}
			}
		}

		return shortestPath;
	}

	class Grid {
		public readonly char[,] grid;
		public readonly Dictionary<Vector2, int> path = new Dictionary<Vector2, int>();
		public readonly Dictionary<char, Vector2> keyLocs = new Dictionary<char, Vector2>();
		public readonly Dictionary<char, Vector2> doorLocs = new Dictionary<char, Vector2>();
		public Vector2 pos1, pos2, pos3, pos4;

		public Grid(string fileName) {
			string[] lines = File.ReadAllLines(fileName);
			grid = new char[lines.Length, lines[0].Length];

			for (int y = 0; y < grid.GetLength(0); y++) {
				char[] line = lines[y].ToCharArray();

				for (int x = 0; x < grid.GetLength(1); x++) {
					char c = line[x];

					if (c >= 'A' && c <= 'Z') {
						doorLocs[(char)(c + 32)] = new Vector2(x, y);
					} else if (c >= 'a' && c <= 'z') {
						keyLocs[c] = new Vector2(x, y);
					} else if (c == '@') {
						c = '.';
						if (y < 40) {
							if (x < 40) {
								pos1 = new Vector2(x, y);
							} else {
								pos2 = new Vector2(x, y);
							}
						} else {
							if (x < 40) {
								pos3 = new Vector2(x, y);
							} else {
								pos4 = new Vector2(x, y);
							}
						}
					}

					grid[y, x] = c;
				}
			}
		}

		public Grid(Grid oldGrid) {
			grid = (char[,])oldGrid.grid.Clone();
			keyLocs = new Dictionary<char, Vector2>(oldGrid.keyLocs);
			doorLocs = new Dictionary<char, Vector2>(oldGrid.doorLocs);
			pos1 = oldGrid.pos1;
			pos2 = oldGrid.pos2;
			pos3 = oldGrid.pos3;
			pos4 = oldGrid.pos4;
		}

		//Breadth-first search
		public void PathAll() {
			path.Clear();
			HashSet<Vector2> enqueued = new HashSet<Vector2>();
			Queue<Vector2> tilesToCheck = new Queue<Vector2>();
			foreach (Vector2 pos in new Vector2[] { pos1, pos2, pos3, pos4 }) {
				tilesToCheck.Enqueue(pos);
				enqueued.Add(pos);
				path[pos] = 0;
			}

			while (tilesToCheck.Count > 0) {
				Vector2 tile = tilesToCheck.Dequeue();
				foreach (Vector2 neighbourTile in new Vector2[] { tile + Vector2.up, tile + Vector2.down, tile + Vector2.left, tile + Vector2.right }) {
					char tileType = grid[neighbourTile.y, neighbourTile.x];
					if (!enqueued.Contains(neighbourTile) && (tileType == '.' || tileType >= 'a' && tileType <= 'z')) {
						if (tileType == '.') { //Don't path through keys, but do consider them as potential path points
							tilesToCheck.Enqueue(neighbourTile);
						}
						enqueued.Add(neighbourTile);
						path[neighbourTile] = path[tile] + 1;
					}
				}
			}
		}
	}

	class GridState : IEquatable<GridState> {
		readonly HashSet<char> keys;
		readonly Vector2 pos1, pos2, pos3, pos4;

		public GridState(Grid grid) {
			keys = new HashSet<char>(grid.keyLocs.Keys);
			pos1 = grid.pos1;
			pos2 = grid.pos2;
			pos3 = grid.pos3;
			pos4 = grid.pos4;
		}

		public override bool Equals(object obj) => Equals(obj as GridState);
		public bool Equals(GridState other) {
			if (other == null || keys.Count != other.keys.Count || !pos1.Equals(other.pos1) || !pos2.Equals(other.pos2) || !pos3.Equals(other.pos3) || !pos4.Equals(other.pos4)) {
				return false;
			}

			foreach (char key in keys) {
				if (!other.keys.Contains(key)) {
					return false;
				}
			}

			return true;
		}

		public override int GetHashCode() {
			int hashCode = keys.Count << 28 ^ pos1.x << 24 ^ pos1.y << 21 ^ pos2.x << 17 ^ pos2.y << 14 ^ pos3.x << 10 ^ pos3.y << 7 ^ pos4.x << 3 ^ pos4.y;
			foreach (char key in keys) {
				hashCode ^= 1 << (key - 97);
			}
			return hashCode;
		}
	}
}
