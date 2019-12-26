using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

//Buggy mess of stuff not behaving as the AoC creator expected when asynchronous.
class Day23 {
	public static int Solution1() {
		Intcode referenceComputer = new Intcode("input23.txt", false);
		Router router = new Router() {
			isSolution1 = true
		};

		for (int address = 0; address < 50; address++) {
			Intcode computer = referenceComputer.Duplicate();
			computer.SetInput(new Queue<BigInteger>());
			computer.InQueue.Enqueue(address);
			computer.SetOutput(new Queue<BigInteger>());
			router.inputs.Add(computer.OutQueue);
			router.outputs[address] = computer.InQueue;
			_ = computer.RunAsync();
		}

		return router.Start().Result;
	}

	public static int Solution2() {
		Intcode referenceComputer = new Intcode("input23.txt", false);
		Router router = new Router() {
			isSolution1 = false
		};

		for (int address = 0; address < 50; address++) {
			Intcode computer = referenceComputer.Duplicate();
			computer.SetInput(new Queue<BigInteger>());
			computer.InQueue.Enqueue(address);
			computer.SetOutput(new Queue<BigInteger>());
			router.inputs.Add(computer.OutQueue);
			router.outputs[address] = computer.InQueue;
			_ = computer.RunAsync();
		}

		return router.Start().Result;
	}

	class Router {
		public readonly List<Queue<BigInteger>> inputs = new List<Queue<BigInteger>>();
		public readonly Dictionary<int, Queue<BigInteger>> outputs = new Dictionary<int, Queue<BigInteger>>();
		public bool isSolution1;
		BigInteger x, y;

		public async Task<int> Start() {
			BigInteger lasty = int.MinValue;
			int idleCount = 0;

			while (true) {
				bool inputEmpty = true;
				foreach (Queue<BigInteger> input in inputs) {
					while (input.Count >= 3) { //3 instructions necessary to send a packet
						inputEmpty = false;

						int destination;
						BigInteger x, y;
						lock (input) {
							destination = (int)input.Dequeue();
							x = input.Dequeue();
							y = input.Dequeue();
						}

						if (destination == 255) {
							if (isSolution1) {
								return (int)y;
							}

							this.x = x;
							this.y = y;
						} else {
							Queue<BigInteger> output = outputs[destination];
							lock (output) {
								output.Enqueue(x);
								output.Enqueue(y);
							}
						}
					}
				}

				//Check for system idle
				bool outputEmpty = true;
				foreach (Queue<BigInteger> output in outputs.Values) {
					if (output.Count > 0) {
						outputEmpty = false;
						break;
					}
				}

				if (inputEmpty) {
					if (outputEmpty) {
						idleCount++;

						if (idleCount > 100) {
							if (y == lasty) {
								return (int)y;
							}
							lasty = y;

							Queue<BigInteger> output = outputs[0];
							lock (output) {
								output.Enqueue(x);
								output.Enqueue(y);
							}
						}
					} else {
						idleCount = 0;
					}

					await Task.Delay(1);
				} else {
					idleCount = 0;
				}
			}
		}
	}
}
