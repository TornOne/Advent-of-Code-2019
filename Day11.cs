using System;
using System.Collections.Generic;
using System.Numerics;

class Day11 {
	public static int Solution1() {
		RobotController robotController = new RobotController {
			InQueue = new Queue<BigInteger>(),
			OutQueue = new Queue<BigInteger>()
		};
		
		Intcode robot = new Intcode("input11.txt");
		robot.SetInput(robotController);
		robot.SetOutput(robotController);

		robot.Run();
		return robotController.grid.paintedTiles;
	}

	public static int Solution2() {
		RobotController robotController = new RobotController {
			InQueue = new Queue<BigInteger>(),
			OutQueue = new Queue<BigInteger>()
		};
		robotController.grid[new Vector2()] = true;

		Intcode robot = new Intcode("input11.txt");
		robot.SetInput(robotController);
		robot.SetOutput(robotController);

		robot.Run();
		robotController.grid.Render();
		return 0;
	}

	class ShipGrid {
		readonly Dictionary<Vector2, bool> grid = new Dictionary<Vector2, bool>();
		public int paintedTiles = 0;

		public bool this[Vector2 coords] {
			get => grid.TryGetValue(coords, out bool value) && value;
			set {
				if (!grid.ContainsKey(coords)) {
					paintedTiles++;
				}
				grid[coords] = value;
			}
		}

		public void Render() {
			int minX = 0, maxX = 0, minY = 0, maxY = 0;
			foreach (Vector2 coords in grid.Keys) {
				minX = Math.Min(minX, coords.x);
				maxX = Math.Max(maxX, coords.x);
				minY = Math.Min(minY, coords.y);
				maxY = Math.Max(maxY, coords.y);
			}

			for (int y = minY; y <= maxY; y++) {
				for (int x = minX; x <= maxX; x++) {
					Console.Write(this[new Vector2(x, y)] ? ' ' : '█');
				}
				Console.WriteLine();
			}
		}
	}

	class RobotController : IIntcode {
		enum Facing {
			North,
			East,
			South,
			West
		}

		Vector2 position = new Vector2();
		Facing facing = Facing.North;
		public ShipGrid grid = new ShipGrid();

		public IIntcode Input { get; set; }
		public IIntcode Output { get; set; }
		public Queue<BigInteger> InQueue { get; set; }
		public Queue<BigInteger> OutQueue { get; set; }

		public void RequestInput() {
			//Check if there's movement data to process in the InQueue
			if (InQueue.Count > 0) {
				grid[position] = InQueue.Dequeue() == 1;
				facing = (Facing)(((int)facing + (InQueue.Dequeue() == 1 ? 1 : 3)) % 4);
				switch (facing) {
					case Facing.North:
						position += new Vector2(0, -1);
						break;
					case Facing.East:
						position += new Vector2(1, 0);
						break;
					case Facing.South:
						position += new Vector2(0, 1);
						break;
					case Facing.West:
						position += new Vector2(-1, 0);
						break;
				}
			}

			//Add current tile to OutQueue
			OutQueue.Enqueue(grid[position] ? 1 : 0);
		}

		public void Continue() {
			return;
		}
	}
}
