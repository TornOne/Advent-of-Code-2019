using System;
using System.Collections.Generic;
using System.IO;

class Day18 {
	static readonly Dictionary<GridState, int> states = new Dictionary<GridState, int>();

	public static int Solution1() {
		int shortestPath = int.MaxValue;
		Queue<Grid> toCheck = new Queue<Grid>();
		toCheck.Enqueue(new Grid("input18.txt"));
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
					newGrid.pos = newGrid.keyLocs[key];
					newGrid.grid[newGrid.pos.y, newGrid.pos.x] = '.';
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
		public Vector2 pos;

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
						pos = new Vector2(x, y);
					}

					grid[y, x] = c;
				}
			}
		}

		public Grid(Grid oldGrid) {
			grid = (char[,])oldGrid.grid.Clone();
			keyLocs = new Dictionary<char, Vector2>(oldGrid.keyLocs);
			doorLocs = new Dictionary<char, Vector2>(oldGrid.doorLocs);
			pos = oldGrid.pos;
		}

		//Breadth-first search
		public void PathAll() {
			path.Clear();
			HashSet<Vector2> enqueued = new HashSet<Vector2>();
			Queue<Vector2> tilesToCheck = new Queue<Vector2>();
			tilesToCheck.Enqueue(pos);
			enqueued.Add(pos);
			path[pos] = 0;

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
		readonly Vector2 pos;

		public GridState(Grid grid) {
			keys = new HashSet<char>(grid.keyLocs.Keys);
			pos = grid.pos;
		}

		public override bool Equals(object obj) => Equals(obj as GridState);
		public bool Equals(GridState other) {
			if (other == null || !pos.Equals(other.pos) || keys.Count != other.keys.Count) {
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
			int hashCode = keys.Count << 24 | pos.x << 12 | pos.y;
			foreach (char key in keys) {
				hashCode ^= 1 << (key - 97);
			}
			return hashCode;
		}
	}
}
