using System.Linq;
using BricksGame.Extensions;

namespace BricksGame
{
	public class Field
	{
		public int Width { get; }
		public int Height { get; }
		public byte[,] Map { get; private set; }
		
		public Field(int width = 10, int height = 20)
		{
			Width = width;
			Height = height;
			Map = new byte[Height, Width];
		}

		public Field(byte[,] map)
		{
			Width = map.GetLength(1);
			Height = map.GetLength(0);
			Map = map;
		}

		public bool CheckCollision(Figure figure)
		{
			for (var i = 0; i < figure.Height; i++)
			{
				for (var j = 0; j < figure.Width; j++)
				{
					if (figure.Points[i, j] == 0)
					{
						continue;
					}

					if (Map[figure.Y + i, figure.X + j] == 1)
					{
						return true;
					}
				}
			}

			return false;
		}

		public void Merge(Figure figure)
		{
			for (var i = 0; i < figure.Height; i++)
			{
				for (var j = 0; j < figure.Width; j++)
				{
					if (figure.Points[i, j] == 1)
					{
						Map[figure.Y + i, figure.X + j] = figure.Points[i, j];
					}
				}
			}
		}

		public int RemoveFullLines()
		{
			var lines = GetFullLines();
			var modifier = 0;

			foreach (var line in lines)
			{
				MoveLevelDown(line + modifier);
				modifier++;
			}

			return lines.Length;
		}

		public void Reset()
		{
			Map = new byte[Height, Width];
		}

		private void MoveLevelDown(int level)
		{
			for (var i = level; i > 0; i--)
			{
				for (var j = 0; j < Width; j++)
				{
					Map[i, j] = Map[i - 1, j];
					Map[i - 1, j] = 0;
				}
			}
		}
    
		private int[] GetFullLines()
		{
			var numbers = new int[Height];
			var count = 0;
			
			for (var i = Height - 1; i >= 0; i--)
			{
				var slice = Map.GetSlice(i);
				if (slice.All(x => x == 1))
				{
					numbers[count++] = i;
				}
			}

			return numbers.Take(count).ToArray();
		}
	}
}
