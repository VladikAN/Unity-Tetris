namespace BricksGame.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] GetSlice<T>(this T[,] array, int index)
        {
            var size = array.GetLength(1);
            var result = new T[size];
            for (var i = 0; i < size; i++)
            {
                result[i] = array[index, i];
            }

            return result;
        }

        public static bool AreEqual<T>(T[,] first, T[,] second)
        {
            if (first.GetLength(0) != second.GetLength(0))
            {
                return false;
            }

            if (first.GetLength(1) != second.GetLength(1))
            {
                return false;
            }

            var size1 = first.GetLength(0);
            var size2 = first.GetLength(1);
            for (var i = 0; i < size1; i++)
            {
                for (var j = 0; j < size2; j++)
                {
                    if (!first[i, j].Equals(second[i, j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}