using System;
using System.IO;

class Day8 {
	const int width = 25;
	const int height = 6;
	const int layerSize = width * height;

	public static int Solution1() {
		byte[] image = ReadImage();

		int leastZeroesInLayer = int.MaxValue;
		int answer = -1;

		for (int layer = 0; layer < image.Length / layerSize; layer++) {
			int[] numbersInLayer = new int[3];
			for (int i = 0; i < layerSize; i++) {
				numbersInLayer[image[layer * layerSize + i]]++;
			}
			
			if (numbersInLayer[0] < leastZeroesInLayer) {
				leastZeroesInLayer = numbersInLayer[0];
				answer = numbersInLayer[1] * numbersInLayer[2];
			}
		}

		return answer;
	}

	public static int Solution2() {
		byte[] data = ReadImage();
		byte[] image = new byte[layerSize];

		for (int i = 0; i < layerSize; i++) {
			for (int layer = 0; layer < data.Length / layerSize; layer++) {
				byte color = data[layer * layerSize + i];
				if (color != 2) {
					image[i] = color;
					break;
				}
			}
		}

		//Render image in console
		for (int i = 0; i < image.Length; i++) {
			Console.Write(image[i] == 0 ? '█' : ' ');
			if (i % width == width - 1) {
				Console.WriteLine();
			}
		}

		return 0;
	}

	static byte[] ReadImage() => Array.ConvertAll(File.ReadAllText("input8.txt").ToCharArray(), x => (byte)(x - 48));
}
