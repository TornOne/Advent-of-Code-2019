using System;
using System.Collections.Generic;
using System.IO;

class Day6 {
	public static int Solution1() {
		HashSet<Planet> planets = new HashSet<Planet>(new PlanetComparer());
		Planet GetPlanet(string name) {
			Planet referencePlanet = new Planet(name);
			if (planets.TryGetValue(referencePlanet, out Planet realPlanet)) {
				return realPlanet;
			} else {
				planets.Add(referencePlanet);
				return referencePlanet;
			}
		}

		string[] allOrbits = File.ReadAllLines("input6.txt");
		foreach (string orbit in allOrbits) {
			string[] planetPair = orbit.Split(')');
			GetPlanet(planetPair[0]).AddChild(GetPlanet(planetPair[1]));
		}

		return GetPlanet("COM").GetChildOrbitCount();
	}

	public static int Solution2() {
		#region Duplicate planet input code
		HashSet<Planet> planets = new HashSet<Planet>(new PlanetComparer());
		Planet GetPlanet(string name) {
			Planet referencePlanet = new Planet(name);
			if (planets.TryGetValue(referencePlanet, out Planet realPlanet)) {
				return realPlanet;
			} else {
				planets.Add(referencePlanet);
				return referencePlanet;
			}
		}

		string[] allOrbits = File.ReadAllLines("input6.txt");
		foreach (string orbit in allOrbits) {
			string[] planetPair = orbit.Split(')');
			GetPlanet(planetPair[0]).AddChild(GetPlanet(planetPair[1]));
		}
		#endregion

		List <Planet> orbitHierarchy1 = new List<Planet>();
		GetPlanet("YOU").GetOrbitHierarchy(orbitHierarchy1);
		List<Planet> orbitHierarchy2 = new List<Planet>();
		GetPlanet("SAN").GetOrbitHierarchy(orbitHierarchy2);

		for (int i = 0; ; i++) {
			if (orbitHierarchy1[i].name != orbitHierarchy2[i].name) {
				return orbitHierarchy1.Count + orbitHierarchy2.Count - i * 2;
			}
		}
	}

	class Planet {
		public string name;
		readonly List<Planet> children = new List<Planet>();
		public Planet parent;

		public Planet(string name) {
			this.name = name;
		}

		public void AddChild(Planet planet) {
			children.Add(planet);
			planet.parent = this;
		}

		public int GetChildOrbitCount(int depth = 0) {
			int sum = depth;
			foreach (Planet child in children) {
				sum += child.GetChildOrbitCount(depth + 1);
			}
			return sum;
		}

		public void GetOrbitHierarchy(List<Planet> orbitHierarchy) {
			if (parent != null) {
				parent.GetOrbitHierarchy(orbitHierarchy);
				orbitHierarchy.Add(parent);
			}
		}
	}

	class PlanetComparer : EqualityComparer<Planet> {
		public override bool Equals(Planet x, Planet y) => x.name == y.name;
		public override int GetHashCode(Planet obj) => obj.name.GetHashCode();
	}
}
