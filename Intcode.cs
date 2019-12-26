using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

class Intcode : IIntcode {
	enum State {
		Ready, //Program is created, has never been run
		Paused, //Program has been run, but has been paused and is ready to run some more
		Waiting, //Program is waiting for additional input
		Finished //Program has finished execution
	}

	enum ParameterMode {
		Position,
		Immediate,
		Relative
	}

	readonly Memory memory;
	State state = State.Ready;
	bool isWaitedOn = false;
	int i = 0; //Memory pointer
	int relativeBase = 0;
	public IIntcode Input { get; set; }
	public IIntcode Output { get; set; }
	public Queue<BigInteger> InQueue { get; set; }
	public Queue<BigInteger> OutQueue { get; set; }
	readonly bool blocking;

	public void SetInput(IIntcode input) {
		Input = input;
		input.Output = this;
		InQueue = input.OutQueue;
	}

	public void SetInput(Queue<BigInteger> inQueue) {
		if (!(Input is null)) {
			Input.Output = null;
			Input = null;
		}
		InQueue = inQueue;
	}

	public void SetOutput(IIntcode output) {
		Output = output;
		output.Input = this;
		OutQueue = output.InQueue;
	}

	public void SetOutput(Queue<BigInteger> outQueue) {
		if (!(Output is null)) {
			Output.Input = null;
			Output = null;
		}
		OutQueue = outQueue;
	}

	public Intcode(string fileName, bool blocking = true) : this(Array.ConvertAll(File.ReadAllText(fileName).Split(','), BigInteger.Parse), blocking) { }

	public Intcode(BigInteger[] memory, bool blocking = true) : this(new Memory(memory), blocking) { }

	public Intcode(Memory memory, bool blocking = true) {
		this.memory = memory;
		this.blocking = blocking;
	}

	//Duplicate doesn't keep IO references, it only copies the queues, if present
	public Intcode Duplicate() {
		if (state != State.Ready) {
			throw new InvalidOperationException("Can't duplicate an already started program.");
		}

		Intcode duplicate = new Intcode(memory.Clone(), blocking);
		if (!(InQueue is null)) {
			duplicate.InQueue = new Queue<BigInteger>(InQueue);
		}
		if (!(OutQueue is null)) {
			duplicate.OutQueue = new Queue<BigInteger>(OutQueue);
		}

		return duplicate;
	}

	public void Continue() {
		if (state == State.Ready || state == State.Paused) {
			Run();
		}
	}

	public int Run() {
		//Check state before running
		switch (state) {
			case State.Finished:
				throw new InvalidOperationException("Attempted to Run a Finished program.");
			case State.Waiting:
				throw new InvalidOperationException("Attempted to Run a program Waiting for input. This is invalid and might cause infinite recursion.");
		}

		while (true) {
			int op = (int)(memory[i] % 100);
			ParameterMode mode1 = (ParameterMode)(int)(memory[i] % 1000 / 100);
			ParameterMode mode2 = (ParameterMode)(int)(memory[i] % 10000 / 1000);
			ParameterMode mode3 = (ParameterMode)(int)(memory[i++] % 100000 / 10000); //Increment on last one

			switch (op) {
				//Addition
				case 1:
					BigInteger sum = NextParameter(mode1) + NextParameter(mode2);
					memory[NextPosition(mode3)] = sum;
					break;
				//Multiplication
				case 2:
					BigInteger product = NextParameter(mode1) * NextParameter(mode2);
					memory[NextPosition(mode3)] = product;
					break;
				//Input
				case 3:
					if (!blocking) {
						if (InQueue != null && InQueue.Count > 0) {
							memory[NextPosition(mode1)] = InQueue.Dequeue();
						} else {
							memory[NextPosition(mode1)] = -1;
						}
						break;
					}

					if (InQueue is null) { //No queue, got to ask from user
						Console.WriteLine("Input integer:");
						memory[NextPosition(mode1)] = int.Parse(Console.ReadLine());
					} else { //Have queue, try to take from that
						if (InQueue.Count == 0) { //Queue is empty
							if (Input is null) { //And nothing to give me more input - must crash
								throw new IOException("Input queue empty - stuck waiting for input.");
							} else { //Have backing input program, ask from that
								state = State.Waiting;
								Input.RequestInput();
								memory[NextPosition(mode1)] = InQueue.Dequeue();
							}
						} else { //Queue isn't empty, take from it and move on
							memory[NextPosition(mode1)] = InQueue.Dequeue();
						}
					}
					break;
				//Output
				case 4:
					if (OutQueue is null) { //No queue, printing to console
						Console.WriteLine(NextParameter(mode1));
					} else { //Have queue, add to that
						OutQueue.Enqueue(NextParameter(mode1));
						if (isWaitedOn) { //Return control to calling method if waited on
							isWaitedOn = false;
							state = State.Paused;
							return 1;
						}
					}
					break;
				//Jump if non-zero
				case 5:
					if (NextParameter(mode1) != 0) {
						i = (int)NextParameter(mode2);
					} else {
						i++;
					}
					break;
				//Jump if zero
				case 6:
					if (NextParameter(mode1) == 0) {
						i = (int)NextParameter(mode2);
					} else {
						i++;
					}
					break;
				//Less than
				case 7:
					BigInteger b = NextParameter(mode1) < NextParameter(mode2) ? 1 : 0;
					memory[NextPosition(mode3)] = b;
					break;
				//Equality
				case 8:
					b = NextParameter(mode1) == NextParameter(mode2) ? 1 : 0;
					memory[NextPosition(mode3)] = b;
					break;
				case 9:
					relativeBase += (int)NextParameter(mode1);
					break;
				//Halt
				case 99:
					state = State.Finished;
					Output?.Continue();
					return 0;
			}
		}
	}

	public async Task<int> RunAsync() {
		//Check state before running
		switch (state) {
			case State.Finished:
				throw new InvalidOperationException("Attempted to Run a Finished program.");
			case State.Waiting:
				throw new InvalidOperationException("Attempted to Run a program Waiting for input. This is invalid and might cause infinite recursion.");
		}

		while (true) {
			int op = (int)(memory[i] % 100);
			ParameterMode mode1 = (ParameterMode)(int)(memory[i] % 1000 / 100);
			ParameterMode mode2 = (ParameterMode)(int)(memory[i] % 10000 / 1000);
			ParameterMode mode3 = (ParameterMode)(int)(memory[i++] % 100000 / 10000); //Increment on last one

			switch (op) {
				//Addition
				case 1:
					BigInteger sum = NextParameter(mode1) + NextParameter(mode2);
					memory[NextPosition(mode3)] = sum;
					break;
				//Multiplication
				case 2:
					BigInteger product = NextParameter(mode1) * NextParameter(mode2);
					memory[NextPosition(mode3)] = product;
					break;
				//Input
				case 3:
					if (!blocking) {
						await Task.Delay(1);
						if (InQueue != null && InQueue.Count > 0) {
							lock (InQueue) {
								memory[NextPosition(mode1)] = InQueue.Dequeue();
							}
						} else {
							memory[NextPosition(mode1)] = -1;
						}
						break;
					}

					if (InQueue is null) { //No queue, got to ask from user
						Console.WriteLine("Input integer:");
						memory[NextPosition(mode1)] = int.Parse(Console.ReadLine());
					} else { //Have queue, try to take from that
						if (InQueue.Count == 0) { //Queue is empty
							if (Input is null) { //And nothing to give me more input - must crash
								throw new IOException("Input queue empty - stuck waiting for input.");
							} else { //Have backing input program, ask from that
								for (int delay = 1; InQueue.Count == 0; delay++) {
									await Task.Delay(delay); //increasing delay, waiting for input
								}
								lock (InQueue) {
									memory[NextPosition(mode1)] = InQueue.Dequeue();
								}
							}
						} else { //Queue isn't empty, take from it and move on
							lock (InQueue) {
								memory[NextPosition(mode1)] = InQueue.Dequeue();
							}
						}
					}
					break;
				//Output
				case 4:
					if (OutQueue is null) { //No queue, printing to console
						Console.WriteLine(NextParameter(mode1));
					} else { //Have queue, add to that
						lock (OutQueue) {
							OutQueue.Enqueue(NextParameter(mode1));
						}
					}
					break;
				//Jump if non-zero
				case 5:
					if (NextParameter(mode1) != 0) {
						i = (int)NextParameter(mode2);
					} else {
						i++;
					}
					break;
				//Jump if zero
				case 6:
					if (NextParameter(mode1) == 0) {
						i = (int)NextParameter(mode2);
					} else {
						i++;
					}
					break;
				//Less than
				case 7:
					BigInteger b = NextParameter(mode1) < NextParameter(mode2) ? 1 : 0;
					memory[NextPosition(mode3)] = b;
					break;
				//Equality
				case 8:
					b = NextParameter(mode1) == NextParameter(mode2) ? 1 : 0;
					memory[NextPosition(mode3)] = b;
					break;
				case 9:
					relativeBase += (int)NextParameter(mode1);
					break;
				//Halt
				case 99:
					state = State.Finished;
					Output?.Continue();
					return 0;
			}
		}
	}

	public void RequestInput() {
		switch (state) {
			case State.Waiting:
				throw new IOException("Circular input waiting loop encountered - deadlocked.");
			case State.Finished:
				throw new IOException("Input suppling program has finished execution - stuck waiting for input.");
			default:
				isWaitedOn = true;
				if (Run() == 0) { //Input finished execution without returning input
					throw new IOException("Input suppling program has finished execution - stuck waiting for input.");
				}
				return; //Successfully got input
		}
	}

	public int RunDuplicate() => Duplicate().Run();

	BigInteger NextParameter(ParameterMode mode) {
		switch (mode) {
			case ParameterMode.Position:
				return memory[(int)memory[i++]];
			case ParameterMode.Immediate:
				return memory[i++];
			case ParameterMode.Relative:
				return memory[(int)memory[i++] + relativeBase];
			default:
				throw new ArgumentException($"Unknown parameter mode {mode} near memory address {i}");
		}
	}

	int NextPosition(ParameterMode mode) {
		switch (mode) {
			case ParameterMode.Position:
				return (int)memory[i++];
			case ParameterMode.Relative:
				return (int)memory[i++] + relativeBase;
			default:
				throw new ArgumentException($"Unknown or invalid position mode {mode} near memory address {i}");
		}
	}

	public static int RunFromFile(string fileName) => new Intcode(fileName).Run();
}

class Memory {
	BigInteger[] memory;

	public BigInteger this[int i] {
		get => i >= memory.Length ? 0 : memory[i];
		set {
			if (i >= memory.Length) {
				Array.Resize(ref memory, i + 1);
			}
			memory[i] = value;
		}
	}

	public Memory(BigInteger[] memory) {
		this.memory = memory;
	}

	public Memory Clone() => new Memory((BigInteger[])memory.Clone());
}

interface IIntcode {
	IIntcode Input { get; set; }
	IIntcode Output { get; set; }
	Queue<BigInteger> InQueue { get; set; }
	Queue<BigInteger> OutQueue { get; set; }

	/// <summary>
	/// Input MUST populate the shared OutQueue before returning.
	/// </summary>
	void RequestInput();

	/// <summary>
	/// Output MAY continue execution as if started normally.
	/// </summary>
	void Continue();
}
