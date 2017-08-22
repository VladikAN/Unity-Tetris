using System.Net;

namespace BricksGame
{
	public class Figure
	{
		public uint X { get; set; }
		public uint Y { get; set; }
		public byte[,] Points { get; private set; }
		public int Height => Points.GetLength(0);
		public int Width => Points.GetLength(1);

		public Figure()
		{
		}

		public Figure(byte[,] points)
		{
			Set(points);
		}

		public void BuildNew(int figure, int rotateTimes)
		{
			Set(FigureType.GetFigure(figure));
			Rotate(rotateTimes);
		}

		public Figure Copy()
		{
			return new Figure
			{
				Points = Points,
				X = X,
				Y = Y
			};
		}

		public void Rotate(int times)
		{
			while (times > 0)
			{
				Rotate();
				times--;
			}
		}
		
		public void Rotate()
		{
			var rotatedFigure = new byte[Width, Height];
			for (var i = 0; i < Height; i++)
			{
				for (var j = 0; j < Width; j++)
				{
					rotatedFigure[Width - j - 1, i] = Points[i, j];
				}
			}

			Set(rotatedFigure);
		}

		public void Set(byte[,] points)
		{
			Points = points;
		}

		public bool MoveLeftIfAllowed(Field field)
		{
			if (IsAllowedToMoveLeft(field))
			{
				X -= 1;
				return true;
			}

			return false;
		}

		public bool MoveRightIfAllowed(Field field)
		{
			if (IsAllowedToMoveRight(field))
			{
				X += 1;
				return true;
			}

			return false;
		}

		public bool MoveDownIfAllowed(Field field)
		{
			if (IsAllowedToMoveDown(field))
			{
				Y += 1;
				return true;
			}

			return false;
		}

		public bool IsAllowedToMoveDown(Field field)
		{
			if (Y + Height >= field.Height)
			{
				return false;
			}

			return IsPossibleToMove(field, X, Y + 1);
		}

		public bool RotateIfAllowed(Field field)
		{
			if (IsAllowedToRotate(field))
			{
				Rotate();
				return true;
			}

			return false;
		}
		
		public bool IsAllowedToRotate(Field field)
		{
			var fullSize = Width > Height ? Width : Height;
			if (X + fullSize >= field.Width + 1)
			{
				return false;
			}

			for (var i = Y; i < Y + fullSize; i++)
			{
				for (var j = X; j < X + fullSize; j++)
				{
					if (field.Map[i, j] == 1)
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool IsAllowedToMoveLeft(Field field)
		{
			if (X == 0)
			{
				return false;
			}

			return IsPossibleToMove(field, X - 1, Y);
		}

		public bool IsAllowedToMoveRight(Field field)
		{
			if (X + Width >= field.Width)
			{
				return false;
			}

			return IsPossibleToMove(field, X + 1, Y);
		}
		
		private bool IsPossibleToMove(Field field, uint x, uint y)
		{
			var tmp = Copy();
			tmp.X = x;
			tmp.Y = y;
			
			return !field.CheckCollision(tmp);
		}
	}
}
