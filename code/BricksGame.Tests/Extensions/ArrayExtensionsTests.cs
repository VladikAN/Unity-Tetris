using BricksGame.Extensions;
using Xunit;

namespace BricksGame.Tests.Extensions
{
    public class ArrayExtensionsTests
    {
        [Fact]
        public void GetSlice_FirstSlice_Result()
        {
            var array = new byte[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            
            var result = array.GetSlice(1);
            
            Assert.Equal(3, result.Length);
            Assert.Equal(4, result[0]);
            Assert.Equal(5, result[1]);
            Assert.Equal(6, result[2]);
        }
        
        [Fact]
        public void GetSlice_SecondSlice_Result()
        {
            var array = new byte[,] { { 1, 2, 3 }, { 4, 5, 6 } };

            var result = array.GetSlice(1);
            
            Assert.Equal(3, result.Length);
            Assert.Equal(4, result[0]);
            Assert.Equal(5, result[1]);
            Assert.Equal(6, result[2]);
        }

        [Fact]
        public void AreEqual_DifferentSize1_False()
        {
            var first = new byte[,] { { 1, 1 } };
            var second = new byte[,] { { 1, 1 }, { 1, 1 } };

            var result = ArrayExtensions.AreEqual(first, second);
            
            Assert.False(result);
        }

        [Fact]
        public void AreEqual_DifferentSize2_False()
        {
            var first = new byte[,] { { 1 }, { 1 } };
            var second = new byte[,] { { 1, 1 }, { 1, 1 } };

            var result = ArrayExtensions.AreEqual(first, second);
            
            Assert.False(result);
        }
    
        [Fact]
        public void AreEqual_DifferentContent_False()
        {
            var first = new byte[,] { { 1, 2 }, { 1, 2 } };
            var second = new byte[,] { { 3, 1 }, { 3, 1 } };

            var result = ArrayExtensions.AreEqual(first, second);
            
            Assert.False(result);
        }

        [Fact]
        public void AreEqual_Identical_True()
        {
            var first = new byte[,] { { 1, 2 }, { 3, 4 } };
            var second = new byte[,] { { 1, 2 }, { 3, 4 } };

            var result = ArrayExtensions.AreEqual(first, second);
            
            Assert.True(result);
        }
    }
}