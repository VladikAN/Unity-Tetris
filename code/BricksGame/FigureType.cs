namespace BricksGame
{
	public class FigureType
	{
		private static readonly byte[,] I = { { 1, 1, 1, 1 } };

		private static readonly byte[,] J = { { 1, 0, 0 },
											  { 1, 1, 1 } };

		private static readonly byte[,] L = { { 0, 0, 1 },
											  { 1, 1, 1 } };

		private static readonly byte[,] O = { { 1, 1 },
											  { 1, 1 } };

		private static readonly byte[,] S = { { 0, 1, 1 },
											  { 1, 1, 0 } };

		private static readonly byte[,] T = { { 0, 1, 0 },
											  { 1, 1, 1 } };

		private static readonly byte[,] Z = { { 1, 1, 0 },
											  { 0, 1, 1 } };

		public static byte[,] GetFigure(int index)
		{
			switch (index)
			{
				case 0: return I;
				case 1: return J;
				case 2: return L;
				case 3: return O;
				case 4: return S;
				case 5: return T;
				case 6: return Z;
				default: return I;
			}
		}
	}
}
