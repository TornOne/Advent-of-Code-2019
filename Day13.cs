using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

class Day13 {
	public static int Solution1() {
		Intcode game = new Intcode("input13.txt");
		game.SetOutput(new Queue<BigInteger>());
		game.Run();

		Dictionary<Vector2, int> screen = new Dictionary<Vector2, int>();

		while (game.OutQueue.Count > 0) {
			screen[new Vector2((int)game.OutQueue.Dequeue(), (int)game.OutQueue.Dequeue())] = (int)game.OutQueue.Dequeue();
		}

		int count = 0;
		foreach (int tile in screen.Values) {
			if (tile == 2) {
				count++;
			}
		}

		return count;
	}

	public static int Solution2() {
		PaddleController controller = new PaddleController {
			InQueue = new Queue<BigInteger>(),
			OutQueue = new Queue<BigInteger>()
		};

		BigInteger[] memory = Array.ConvertAll(File.ReadAllText("input13.txt").Split(','), BigInteger.Parse);
		memory[0] = 2; //Play4Free hax
		Intcode game = new Intcode(memory);
		game.SetInput(controller);
		game.SetOutput(controller);

		//Console.WindowWidth = 41;
		//Console.WindowHeight = 28;
		//Console.BufferWidth = Console.WindowWidth;
		//Console.BufferHeight = Console.WindowHeight;
		//Console.ReadLine();
		game.Run();

		return controller.grid[new Vector2(-1, 0)];
	}

	class GameGrid {
		readonly Dictionary<Vector2, int> grid = new Dictionary<Vector2, int>();
		int width, height;

		public int this[Vector2 coords] {
			get => grid.TryGetValue(coords, out int value) ? value : 0;
			set => grid[coords] = value;
		}

		public void Render() {
			//Calculate the grid size if it has not been received before
			if (width == 0) {
				foreach (Vector2 coords in grid.Keys) {
					width = Math.Max(width, coords.x);
					height = Math.Max(height, coords.y);
				}
			}

			string screen = "";
			screen += $"Score: {this[new Vector2(-1, 0)]}\n";
			for (int y = 0; y <= height; y++) {
				for (int x = 0; x <= width; x++) {
					char tile;
					switch (this[new Vector2(x, y)]) {
						case 0: //Nothing
							tile = ' ';
							break;
						case 1: //Wall
							tile = '█';
							break;
						case 2: //Block
							tile = '▓';
							break;
						case 3: //Paddle
							tile = '═';
							break;
						default: //4 - Ball
							tile = 'Θ';
							break;
					}
					screen += tile;
				}
				screen += "\n";
			}
			Console.Clear();
			Console.Write(screen);
		}
	}

	class PaddleController : IIntcode {
		Vector2 paddlePos = new Vector2();
		Vector2 ballPos = new Vector2();
		public GameGrid grid = new GameGrid();

		public IIntcode Input { get; set; }
		public IIntcode Output { get; set; }
		public Queue<BigInteger> InQueue { get; set; }
		public Queue<BigInteger> OutQueue { get; set; }

		public void RequestInput() {
			//Check if there's output data to put on the grid
			ReadQueueToGrid();

			//grid.Render();
			//System.Threading.Thread.Sleep(50);

			//Move paddle to meet ball
			OutQueue.Enqueue(Math.Sign(ballPos.x - paddlePos.x));
		}

		public void Continue() => ReadQueueToGrid();

		void ReadQueueToGrid() {
			while (InQueue.Count > 0) {
				Vector2 pos = new Vector2((int)InQueue.Dequeue(), (int)InQueue.Dequeue());
				int tile = (int)InQueue.Dequeue();
				grid[pos] = tile;

				if (tile == 3) {
					paddlePos = pos;
				} else if (tile == 4) {
					ballPos = pos;
				}
			}
		}
	}
}
