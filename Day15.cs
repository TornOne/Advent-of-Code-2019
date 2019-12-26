using System;
using System.Collections.Generic;
using System.Numerics;

class Day15 {
	public static int Solution1() {
		RobotController controller = new RobotController {
			InQueue = new Queue<BigInteger>(),
			OutQueue = new Queue<BigInteger>()
		};

		Intcode robot = new Intcode("input15.txt");
		robot.SetInput(controller);
		robot.SetOutput(controller);

		//Console.WindowWidth = 42;
		//Console.WindowHeight = 43;
		//Console.BufferWidth = Console.WindowWidth;
		//Console.BufferHeight = Console.WindowHeight;

		try {
			robot.Run();
		} catch {
			//Console.ReadLine();
		}
		return 0;
	}

	//Lazily just repeat Solution1, because that has both answers
	public static int Solution2() => Solution1();

	class Grid {
		public readonly Dictionary<Vector2, bool> grid = new Dictionary<Vector2, bool>();

		//Breadth-first search
		public IEnumerable<Vector2> Path(Vector2 source, Vector2 destination) {
			Dictionary<Vector2, Vector2> enqueued = new Dictionary<Vector2, Vector2>();
			Queue<Vector2> tilesToCheck = new Queue<Vector2>();
			//Path backwards so it'd be easier to return the path
			tilesToCheck.Enqueue(destination);
			enqueued[destination] = destination; //Self parent indicates root node

			while (tilesToCheck.Count > 0) {
				Vector2 tile = tilesToCheck.Dequeue();
				foreach (Vector2 neighbourTile in new Vector2[] { tile + Vector2.up, tile + Vector2.down, tile + Vector2.left, tile + Vector2.right }) {
					//If you reach the source, traverse the path back
					if (neighbourTile.Equals(source)) {
						Vector2 parent = tile;
						while (!parent.Equals(source)) {
							yield return parent - source;
							source = parent;
							parent = enqueued[source];
						}
						yield break;
					}

					if (!enqueued.ContainsKey(neighbourTile) && grid.TryGetValue(neighbourTile, out bool isWall) && !isWall) {
						tilesToCheck.Enqueue(neighbourTile);
						enqueued[neighbourTile] = tile;
					}
				}
			}
		}

		//Breadth-first search that doesn't have a target
		public int LongestPath(Vector2 source) {
			Dictionary<Vector2, int> enqueued = new Dictionary<Vector2, int>();
			Queue<Vector2> tilesToCheck = new Queue<Vector2>();
			tilesToCheck.Enqueue(source);
			enqueued[source] = 0; //Source tile distance is 0
			int longestPath = 0;

			while (tilesToCheck.Count > 0) {
				Vector2 tile = tilesToCheck.Dequeue();
				foreach (Vector2 neighbourTile in new Vector2[] { tile + Vector2.up, tile + Vector2.down, tile + Vector2.left, tile + Vector2.right }) {
					if (!enqueued.ContainsKey(neighbourTile) && grid.TryGetValue(neighbourTile, out bool isWall) && !isWall) {
						tilesToCheck.Enqueue(neighbourTile);
						enqueued[neighbourTile] = enqueued[tile] + 1;
					}
				}
				longestPath = Math.Max(longestPath, enqueued[tile]);
			}

			return longestPath;
		}

		public void Render(Vector2 robotPos) {
			int minX = 0, maxX = 0, minY = 0, maxY = 0;
			foreach (Vector2 coords in grid.Keys) {
				minX = Math.Min(minX, coords.x);
				maxX = Math.Max(maxX, coords.x);
				minY = Math.Min(minY, coords.y);
				maxY = Math.Max(maxY, coords.y);
			}

			string screen = "";
			for (int y = minY; y <= maxY; y++) {
				for (int x = minX; x <= maxX; x++) {
					if (y == robotPos.y && x == robotPos.x) {
						screen += 'D';
						continue;
					}

					screen += grid.TryGetValue(new Vector2(x, y), out bool isWall) ? (isWall ? '█' : '.') : ' ';
				}
				screen += "\n";
			}
			Console.Clear();
			Console.Write(screen);
		}
	}

	class RobotController : IIntcode {
		Vector2 robotPos = new Vector2();
		Vector2 systemPos = new Vector2();
		Vector2 lastCommand = new Vector2();
		public Grid grid = new Grid();
		readonly HashSet<Vector2> explorableTiles = new HashSet<Vector2>();

		public IIntcode Input { get; set; }
		public IIntcode Output { get; set; }
		public Queue<BigInteger> InQueue { get; set; }
		public Queue<BigInteger> OutQueue { get; set; }

		public void RequestInput() {
			ReadQueueToGrid();

			//grid.Render(robotPos);
			//System.Threading.Thread.Sleep(50);

			if (explorableTiles.Count > 0) {
				Vector2 tile;
				using (var enumerator = explorableTiles.GetEnumerator()) {
					enumerator.MoveNext();
					tile = enumerator.Current;
				}
				explorableTiles.Remove(tile);
				foreach (Vector2 moveInstruction in grid.Path(robotPos, tile)) {
					OutQueue.Enqueue(Array.FindIndex(new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right }, direction => direction.Equals(moveInstruction)) + 1);
					lastCommand = moveInstruction;
				}
				robotPos = tile;
				return;
			}

			//Got the answer, print it, crash the robot to get back from it
			Console.WriteLine(new List<Vector2>(grid.Path(new Vector2(), systemPos)).Count);
			Console.WriteLine(grid.LongestPath(systemPos));
			Input.RequestInput(); //Bad thing to do
		}

		public void Continue() {
			return;
		}

		void ReadQueueToGrid() {
			//We know the map up to the last tile we went to, so we only care about the last command and the last reply
			int lastReply = 1;
			while (InQueue.Count > 0) {
				lastReply = (int)InQueue.Dequeue();
			}

			switch (lastReply) {
				case 0:
					grid.grid[robotPos] = true;
					robotPos -= lastCommand;
					break;
				case 1:
				case 2:
					grid.grid[robotPos] = false;
					//Add any new tiles we can now try to explore
					foreach (Vector2 newPos in new Vector2[] { robotPos + Vector2.up, robotPos + Vector2.down, robotPos + Vector2.left, robotPos + Vector2.right }) {
						if (!grid.grid.ContainsKey(newPos)) {
							explorableTiles.Add(newPos);
						}
					}
					if (lastReply == 2) {
						systemPos = robotPos;
					}
					break;
			}
		}
	}
}
