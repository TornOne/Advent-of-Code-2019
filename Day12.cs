using System;

class Day12 {
	static readonly Vector3[] startPositions = new Vector3[] { new Vector3(-16, 15, -9), new Vector3(-14, 5, 4), new Vector3(2, 0, 6), new Vector3(-3, 18, 9) };

	public static int Solution1() {
		Vector3[] positions = (Vector3[])startPositions.Clone();
		Vector3[] velocities = new Vector3[positions.Length];

		for (int i = 0; i < 1000; i++) {
			//Apply acceleration
			for (int a = 0; a < positions.Length; a++) {
				for (int b = a + 1; b < positions.Length; b++) {
					Vector3 delta = (positions[a] - positions[b]).Sign;
					velocities[a] -= delta;
					velocities[b] += delta;
				}
			}

			//Apply velocity
			for (int a = 0; a < positions.Length; a++) {
				positions[a] += velocities[a];
			}
		}

		int energy = 0;
		for (int i = 0; i < positions.Length; i++) {
			energy += positions[i].ManhattanDistance * velocities[i].ManhattanDistance;
		}

		return energy;
	}

	public static int Solution2() {
		Vector3[] positions = (Vector3[])startPositions.Clone();
		Vector3[] velocities = new Vector3[positions.Length];
		/* You can actually assume the simulation reaches its initial state, not some intermediate state it's been in
		HashSet<State1D> xStates = new HashSet<State1D>();
		HashSet<State1D> yStates = new HashSet<State1D>();
		HashSet<State1D> zStates = new HashSet<State1D>();
		*/
		State1D xState = new State1D(positions[0].x, positions[1].x, positions[2].x, positions[3].x, velocities[0].x, velocities[1].x, velocities[2].x, velocities[3].x);
		State1D yState = new State1D(positions[0].y, positions[1].y, positions[2].y, positions[3].y, velocities[0].y, velocities[1].y, velocities[2].y, velocities[3].y);
		State1D zState = new State1D(positions[0].z, positions[1].z, positions[2].z, positions[3].z, velocities[0].z, velocities[1].z, velocities[2].z, velocities[3].z);

		int i = 0;
		bool xNotFound = true, yNotFound = true, zNotFound = true;
		int xCycles = 0, yCycles = 0, zCycles = 0;
		while (xNotFound || yNotFound || zNotFound) {
			//Apply acceleration
			for (int a = 0; a < positions.Length; a++) {
				for (int b = a + 1; b < positions.Length; b++) {
					Vector3 delta = (positions[a] - positions[b]).Sign;
					velocities[a] -= delta;
					velocities[b] += delta;
				}
			}

			//Apply velocity
			for (int a = 0; a < positions.Length; a++) {
				positions[a] += velocities[a];
			}

			i++;
			if (xNotFound && xState == new State1D(positions[0].x, positions[1].x, positions[2].x, positions[3].x, velocities[0].x, velocities[1].x, velocities[2].x, velocities[3].x)) {
				xNotFound = false;
				xCycles = i;
			}
			if (yNotFound && yState == new State1D(positions[0].y, positions[1].y, positions[2].y, positions[3].y, velocities[0].y, velocities[1].y, velocities[2].y, velocities[3].y)) {
				yNotFound = false;
				yCycles = i;
			}
			if (zNotFound && zState == new State1D(positions[0].z, positions[1].z, positions[2].z, positions[3].z, velocities[0].z, velocities[1].z, velocities[2].z, velocities[3].z)) {
				zNotFound = false;
				zCycles = i;
			}
		}

		Console.WriteLine(LCM(LCM(xCycles, yCycles), zCycles));
		return 0;
	}

	static long GCF(long a, long b) {
		while (a != b) {
			if (a > b) {
				a -= b;
			} else {
				b -= a;
			}
		}
		return a;
	}

	static long LCM(long a, long b) => a / GCF(a, b) * b;

	/*
	class State1DComparer : EqualityComparer<State1D> {
		public override bool Equals(State1D x, State1D y) => x == y;

		public override int GetHashCode(State1D obj) => obj.pos1 ^ (obj.pos2 << 4) ^ (obj.pos3 << 8) ^ (obj.pos4 << 12) ^ (obj.vel1 << 16) ^ (obj.vel2 << 20) ^ (obj.vel3 << 24) ^ (obj.vel4 << 28);
	}
	*/

	class State1D : IEquatable<State1D> {
		public readonly int pos1, pos2, pos3, pos4, vel1, vel2, vel3, vel4;

		public State1D (int pos1, int pos2, int pos3, int pos4, int vel1, int vel2, int vel3, int vel4) {
			this.pos1 = pos1;
			this.pos2 = pos2;
			this.pos3 = pos3;
			this.pos4 = pos4;
			this.vel1 = vel1;
			this.vel2 = vel2;
			this.vel3 = vel3;
			this.vel4 = vel4;
		}

		public override bool Equals(object obj) => obj is State1D d && Equals(d);
		public bool Equals(State1D other) => this == other;
		public override int GetHashCode() => pos1 ^ (pos2 << 4) ^ (pos3 << 8) ^ (pos4 << 12) ^ (vel1 << 16) ^ (vel2 << 20) ^ (vel3 << 24) ^ (vel4 << 28);

		public static bool operator ==(State1D a, State1D b) => a.pos1 == b.pos1 && a.pos2 == b.pos2 && a.pos3 == b.pos3 && a.pos4 == b.pos4 && a.vel1 == b.vel1 && a.vel2 == b.vel2 && a.vel3 == b.vel3 && a.vel4 == b.vel4;
		public static bool operator !=(State1D a, State1D b) => !(a == b);
	}
}

struct Vector3 {
	public readonly int x, y, z;

	public Vector3(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3 Sign => new Vector3(Math.Sign(x), Math.Sign(y), Math.Sign(z));

	public int ManhattanDistance => Math.Abs(x) + Math.Abs(y) + Math.Abs(z);

	public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
	public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
	public static Vector3 operator -(Vector3 a) => new Vector3(-a.x, -a.y, -a.z);
}