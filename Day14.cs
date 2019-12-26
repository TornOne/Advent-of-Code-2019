using System;
using System.Collections.Generic;
using System.IO;

class Day14 {
	public static int Solution1() {
		//Read in all the recipes
		string[] recipeStrings = File.ReadAllLines("input14.txt");
		Dictionary<string, (string, int)[]> recipes = new Dictionary<string, (string, int)[]>();
		Dictionary<string, int> recipeGains = new Dictionary<string, int>();
		foreach (string recipe in recipeStrings) {
			string[] recipeParts = recipe.Split(new string[] { " => " }, StringSplitOptions.RemoveEmptyEntries);
			string[] result = recipeParts[1].Split(' ');
			(string, int)[] ingredients = Array.ConvertAll(recipeParts[0].Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries), ingredient => {
				string[] ingredientParts = ingredient.Split(' ');
				return (ingredientParts[1], int.Parse(ingredientParts[0]));
			});
			recipes[result[1]] = ingredients;
			recipeGains[result[1]] = int.Parse(result[0]);
		}

		//Construct a queue of all the resources I need and a dictionary of everything I have left over
		Queue<(string resource, int quantity)> thingsINeed = new Queue<(string, int)>();
		Dictionary<string, int> thingsIHaveLeftOver = new Dictionary<string, int>();
		thingsINeed.Enqueue(("FUEL", 1));
		int oreNeeded = 0;

		//Work through the queue until it's empty
		while (thingsINeed.Count > 0) {
			(string need, int amount) = thingsINeed.Dequeue();
			//ORE goes straight to the counter
			if (need == "ORE") {
				oreNeeded += amount;
				continue;
			}

			//Check if I have any in stock
			if (thingsIHaveLeftOver.TryGetValue(need, out int surplus)) {
				if (amount < surplus) {
					thingsIHaveLeftOver[need] = surplus - amount;
					continue;
				} else {
					amount -= surplus;
					thingsIHaveLeftOver.Remove(need);
				}
			}

			//Convert the rest to materials it's made of
			int recipeAmount = amount / recipeGains[need];
			if (recipeAmount * recipeGains[need] < amount) {
				recipeAmount++;
				thingsIHaveLeftOver[need] = recipeAmount * recipeGains[need] - amount;
			}
			foreach ((string resource, int quantity) in recipes[need]) {
				thingsINeed.Enqueue((resource, recipeAmount * quantity));
			}
		}

		return oreNeeded;
	}

	//Non-elegant brute force solution copy-pasted code
	public static int Solution2() {
		//Read in all the recipes
		string[] recipeStrings = File.ReadAllLines("input14.txt");
		Dictionary<string, (string, int)[]> recipes = new Dictionary<string, (string, int)[]>();
		Dictionary<string, int> recipeGains = new Dictionary<string, int>();
		foreach (string recipe in recipeStrings) {
			string[] recipeParts = recipe.Split(new string[] { " => " }, StringSplitOptions.RemoveEmptyEntries);
			string[] result = recipeParts[1].Split(' ');
			(string, int)[] ingredients = Array.ConvertAll(recipeParts[0].Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries), ingredient => {
				string[] ingredientParts = ingredient.Split(' ');
				return (ingredientParts[1], int.Parse(ingredientParts[0]));
			});
			recipes[result[1]] = ingredients;
			recipeGains[result[1]] = int.Parse(result[0]);
		}

		//Construct a queue of all the resources I need and a dictionary of everything I have left over
		Queue<(string resource, int quantity)> thingsINeed = new Queue<(string, int)>();
		Dictionary<string, int> thingsIHaveLeftOver = new Dictionary<string, int>();
		long oreLeft = 1000000000000;
		int fuelMade = 0;

		while (true) {
			thingsINeed.Enqueue(("FUEL", 1));

			//Work through the queue until it's empty
			while (thingsINeed.Count > 0) {
				(string need, int amount) = thingsINeed.Dequeue();
				//ORE goes straight to the counter
				if (need == "ORE") {
					oreLeft -= amount;
					if (oreLeft < 0) {
						return fuelMade;
					}
					continue;
				}

				//Check if I have any in stock
				if (thingsIHaveLeftOver.TryGetValue(need, out int surplus)) {
					if (amount < surplus) {
						thingsIHaveLeftOver[need] = surplus - amount;
						continue;
					} else {
						amount -= surplus;
						thingsIHaveLeftOver.Remove(need);
					}
				}

				//Convert the rest to materials it's made of
				int recipeAmount = amount / recipeGains[need];
				if (recipeAmount * recipeGains[need] < amount) {
					recipeAmount++;
					thingsIHaveLeftOver[need] = recipeAmount * recipeGains[need] - amount;
				}
				foreach ((string resource, int quantity) in recipes[need]) {
					thingsINeed.Enqueue((resource, recipeAmount * quantity));
				}
			}

			fuelMade++;
		}
	}
}
