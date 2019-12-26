using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

class Day17 {
	public static int Solution1() {
		RobotController controller = new RobotController {
			InQueue = new Queue<BigInteger>(),
			OutQueue = new Queue<BigInteger>()
		};

		Intcode robot = new Intcode("input17.txt");
		robot.SetInput(controller);
		robot.SetOutput(controller);

		robot.Run();

		int sum = 0;
		for (int y = 0; y < controller.grid.width; y++) {
			for (int x = 0; x < controller.grid.height; x++) {
				if (controller.grid[new Vector2(x, y)] && controller.grid[new Vector2(x - 1, y)] && controller.grid[new Vector2(x + 1, y)] && controller.grid[new Vector2(x, y - 1)] && controller.grid[new Vector2(x, y + 1)]) {
					sum += x * y;
				}
			}
		}
		return sum;
	}

	public static int Solution2() {
		RobotController controller = new RobotController {
			InQueue = new Queue<BigInteger>(),
			OutQueue = new Queue<BigInteger>()
		};

		BigInteger[] memory = Array.ConvertAll(File.ReadAllText("input17.txt").Split(','), BigInteger.Parse);
		memory[0] = 2; //Scrub4Free hax
		Intcode robot = new Intcode(memory);
		robot.SetInput(controller);
		robot.SetOutput(controller);

		robot.Run();

		/* Manual solving help
		Vector2 currentDirection = Vector2.up;
		Vector2 GetFreeDirection() {
			foreach (Vector2 dir in new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right }) {
				if (!dir.Equals(-currentDirection) && controller.grid[controller.robotPos + dir]) {
					if (currentDirection.x == 0) {
						Console.Write(currentDirection.y == dir.x ? 'L' : 'R');
					} else {
						Console.Write(currentDirection.x == dir.y ? 'R' : 'L');
					}
					return dir;
				}
			}
			return new Vector2();
		}

		while (!(currentDirection = GetFreeDirection()).Equals(new Vector2())) {
			int segmentLength = 0;
			while (controller.grid[controller.robotPos + currentDirection]) {
				controller.robotPos += currentDirection;
				segmentLength++;
			}
			Console.Write(segmentLength + ", ");
		}
		Console.WriteLine();
		*/

		return 0;
	}

	class Grid {
		readonly Dictionary<Vector2, bool> grid = new Dictionary<Vector2, bool>();
		public int height = -1;
		public int width = -1;

		public bool this[Vector2 coords] {
			get => grid.TryGetValue(coords, out bool isFloor) && isFloor;
			set => grid[coords] = value;
		}

		public void Render(Vector2 robotPos, Facing facing) {
			//Initialize width and height
			if (height == -1) {
				foreach (Vector2 coords in grid.Keys) {
					width = Math.Max(width, coords.x + 1);
					height = Math.Max(height, coords.y + 1);
				}
			}

			string screen = "";
			for (int y = 0; y < width; y++) {
				for (int x = 0; x < height; x++) {
					if (y == robotPos.y && x == robotPos.x) {
						screen += facing == Facing.North ? '^' : (facing == Facing.South ? 'v' : (facing == Facing.West ? '<' : '>'));
						continue;
					}

					screen += this[new Vector2(x, y)] ? '#' : '.';
				}
				screen += "\n";
			}
			Console.Write(screen);
		}
	}

	class RobotController : IIntcode {
		public Vector2 robotPos = new Vector2();
		Facing facing = Facing.North;
		public Grid grid = new Grid();

		public IIntcode Input { get; set; }
		public IIntcode Output { get; set; }
		public Queue<BigInteger> InQueue { get; set; }
		public Queue<BigInteger> OutQueue { get; set; }

		public void RequestInput() {
			ReadQueueToGrid();
			EnqueueString("A,B,B,C,B,C,B,C,A,A\nL,6,R,8,L,4,R,8,L,12\nL,12,R,10,L,4\nL,12,L,6,L,4,L,4\nn\n"); //Manual solution
		}

		public void Continue() {
			ReadQueueToGrid();
			grid.Render(robotPos, facing);
			if (InQueue.Count > 0) {
				Console.WriteLine(InQueue.Dequeue());
			}
		}

		void EnqueueString(string toQueue) {
			foreach (char c in toQueue) {
				OutQueue.Enqueue(c);
			}
		}

		void ReadQueueToGrid() {
			int y = 0;
			int x = 0;
			while (InQueue.Count > 0) {
				if (InQueue.Peek().CompareTo(char.MaxValue) > 0) {
					return;
				}
				char tile = (char)InQueue.Dequeue();
				if (tile == '\n') {
					x = 0;
					y++;
					continue;
				}
				if (tile == '.' || tile == 'X') {
					grid[new Vector2(x, y)] = false;
					if (tile == 'X') {
						robotPos = new Vector2(x, y);
					}
				} else if (tile == '#' || tile == '^' || tile == 'v' || tile == '<' || tile == '>') {
					grid[new Vector2(x, y)] = true;
					if (tile != '#') {
						robotPos = new Vector2(x, y);
						if (tile == '^') {
							facing = Facing.North;
						} else if (tile == 'v') {
							facing = Facing.South;
						} else if (tile == '<') {
							facing = Facing.West;
						} else {
							facing = Facing.East;
						}
					}
				}
				x++;
			}
		}
	}

	enum Facing {
		North,
		South,
		West,
		East
	}
}
