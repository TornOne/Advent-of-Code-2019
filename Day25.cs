using System;
using System.Collections.Generic;
using System.Numerics;

class Day25 {
	public static int Solution1() {
		DroidController controller = new DroidController() {
			InQueue = new Queue<BigInteger>(),
			OutQueue = new Queue<BigInteger>()
		};

		Intcode droid = new Intcode("input25.txt");
		droid.SetInput(controller);
		droid.SetOutput(controller);

		droid.Run();
		return 0;
	}

	class DroidController : IIntcode {
		public IIntcode Input { get; set; }
		public IIntcode Output { get; set; }
		public Queue<BigInteger> InQueue { get; set; }
		public Queue<BigInteger> OutQueue { get; set; }

		public void RequestInput() {
			PrintQueue();
			EnqueueString(Console.ReadLine());
			OutQueue.Enqueue('\n');
		}

		void EnqueueString(string toQueue) {
			foreach (char c in toQueue) {
				OutQueue.Enqueue(c);
			}
		}

		public void Continue() => PrintQueue();

		void PrintQueue() {
			while (InQueue.Count > 0) {
				Console.Write((char)InQueue.Dequeue());
			}
		}
	}
}
