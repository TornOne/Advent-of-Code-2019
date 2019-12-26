using System;
using System.Collections.Generic;
using System.Numerics;

class Day21 {
	public static int Solution1() {
		SpringController controller = new SpringController("NOT C J\nNOT B T\nOR T J\nNOT A T\nOR T J\nAND D J\nWALK\n") {
			InQueue = new Queue<BigInteger>(),
			OutQueue = new Queue<BigInteger>()
		};

		Intcode spring = new Intcode("input21.txt");
		spring.SetInput(controller);
		spring.SetOutput(controller);

		return spring.Run();
	}

	public static int Solution2() {
		SpringController controller = new SpringController("NOT H J\nOR G J\nOR E J\nOR F T\nAND E T\nNOT T T\nAND T J\nAND F J\nOR C J\nAND B J\nNOT J J\nAND D J\nNOT A T\nOR T J\nRUN\n") {
			InQueue = new Queue<BigInteger>(),
			OutQueue = new Queue<BigInteger>()
		};

		Intcode spring = new Intcode("input21.txt");
		spring.SetInput(controller);
		spring.SetOutput(controller);

		return spring.Run();
	}

	class SpringController : IIntcode {
		readonly string program;

		public SpringController(string program) {
			this.program = program;
		}

		public IIntcode Input { get; set; }
		public IIntcode Output { get; set; }
		public Queue<BigInteger> InQueue { get; set; }
		public Queue<BigInteger> OutQueue { get; set; }

		public void RequestInput() {
			PrintQueue();
			EnqueueString(program);
			////Manual solution
			//if (!fast) {
			//	EnqueueString("NOT C J\nNOT B T\nOR T J\nNOT A T\nOR T J\nAND D J\nWALK\n");
			//} else {
			//	EnqueueString("NOT C J\nNOT B T\nOR T J\nNOT A T\nOR T J\nAND D J\nRUN\n");
			//}
		}

		public void Continue() => PrintQueue();

		void EnqueueString(string toQueue) {
			foreach (char c in toQueue) {
				OutQueue.Enqueue(c);
			}
		}

		void PrintQueue() {
			while (InQueue.Count > 0) {
				BigInteger bint = InQueue.Dequeue();
				if (bint > char.MaxValue) {
					Console.WriteLine(bint);
					return;
				}
				Console.Write((char)bint);
			}
		}
	}
}
