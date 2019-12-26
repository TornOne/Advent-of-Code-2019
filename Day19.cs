using System;
using System.Collections.Generic;
using System.Numerics;

class Day19 {
	public static int Solution1() {
		Queue<BigInteger> tractorQueue = new Queue<BigInteger>(2500);
		Intcode originalDrone = new Intcode("input19.txt");

		for (int y = 0; y < 50; y++) {
			for (int x = 0; x < 50; x++) {
				DroneController controller = new DroneController(x, y) {
					OutQueue = new Queue<BigInteger>()
				};

				Intcode drone = originalDrone.Duplicate();
				drone.SetInput(controller);
				drone.SetOutput(tractorQueue);
				drone.Run();
			}
		}

		int sum = 0;
		foreach (BigInteger result in tractorQueue) {
			sum += (int)result;
		}
		return sum;
	}

	public static int Solution2() {
		Intcode drone = new Intcode("input19.txt");
		Vector2 topLeft = new Vector2();
		bool topRightAligned = TestCoord(GetTopRight(topLeft), drone);
		bool botLeftAligned = TestCoord(GetBotLeft(topLeft), drone);

		while (!(topRightAligned && botLeftAligned)) {
			while (!topRightAligned) {
				topLeft += Vector2.down;
				topRightAligned = TestCoord(GetTopRight(topLeft), drone);
			}
			botLeftAligned = TestCoord(GetBotLeft(topLeft), drone);
			while (!botLeftAligned) {
				topLeft += Vector2.right;
				botLeftAligned = TestCoord(GetBotLeft(topLeft), drone);
			}
			topRightAligned = TestCoord(GetTopRight(topLeft), drone);
		}

		return topLeft.x * 10000 + topLeft.y;
	}

	static Vector2 GetTopRight(Vector2 topLeft) => topLeft + new Vector2(99, 0);
	static Vector2 GetBotLeft(Vector2 topLeft) => topLeft + new Vector2(0, 99);
	static bool TestCoord(Vector2 coord, Intcode original) {
		DroneController controller = new DroneController(coord.x, coord.y) {
			OutQueue = new Queue<BigInteger>()
		};

		Intcode drone = original.Duplicate();
		drone.SetInput(controller);
		drone.SetOutput(new Queue<BigInteger>());
		drone.Run();

		return drone.OutQueue.Dequeue() == 1;
	}

	class DroneController : IIntcode {
		readonly int x, y;

		public DroneController(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public IIntcode Input { get; set; }
		public IIntcode Output { get; set; }
		public Queue<BigInteger> InQueue { get; set; }
		public Queue<BigInteger> OutQueue { get; set; }

		public void RequestInput() {
			OutQueue.Enqueue(x);
			OutQueue.Enqueue(y);
		}

		public void Continue() {
			return;
		}
	}
}
