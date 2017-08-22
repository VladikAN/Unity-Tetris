using BricksGame.Extensions;
using Xunit;

namespace BricksGame.Tests
{
    public class FieldTests
    {
        [Fact]
        public void CheckCollision_EmptyFields_False()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            var field = new Field(map);
            var figure = new Figure(new byte[,] { { 1, 1 }, { 1, 1 } });

            var result = field.CheckCollision(figure);
            
            Assert.False(result);
        }
        
        [Fact]
        public void CheckCollision_LockedField_True()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 0 }
            };
            var field = new Field(map);
            var figure = new Figure(new byte[,] { { 1, 1 }, { 1, 1 } });

            var result = field.CheckCollision(figure);
            
            Assert.True(result);
        }
        
        [Fact]
        public void Merge_ZeroPosition_Merged()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 1, 1, 1 }
            };
            var field = new Field(map);

            var points = new byte[,]
            {
                { 1, 1 },
                { 1, 1 }
            };
            var figure = new Figure(points);
            figure.X = 0;
            figure.Y = 0;
            
            var expected = new byte[,]
            {
                { 1, 1, 0 },
                { 1, 1, 0 },
                { 1, 1, 1 }
            };
            
            field.Merge(figure);
            
            Assert.True(ArrayExtensions.AreEqual(field.Map, expected));
        }
        
        [Fact]
        public void Merge_NonZeroPosition_Merged()
        {
            var map = new byte[,]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 }
            };
            var field = new Field(map);

            var points = new byte[,]
            {
                { 1, 1 },
                { 1, 1 }
            };
            var figure = new Figure(points);
            figure.X = 2;
            figure.Y = 1;
            
            var expected = new byte[,]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 1, 1 },
                { 0, 0, 1, 1 },
                { 1, 1, 1, 1 }
            };
            
            field.Merge(figure);
            
            Assert.True(ArrayExtensions.AreEqual(field.Map, expected));
        }
        
        [Fact]
        public void RemoveFullLines_FullMap_Removed()
        {
            var map = new byte[,]
            {
                { 1, 0, 0 },
                { 1, 1, 1 },
                { 1, 0, 1 },
                { 1, 1, 1 }
            };
            var expected = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 1, 0, 0 },
                { 1, 0, 1 }
            };
            var field = new Field(map);

            field.RemoveFullLines();
            
            Assert.True(ArrayExtensions.AreEqual(field.Map, expected));
        }
        
        [Fact]
        public void Reset_Map_Empty()
        {
            var field = new Field(new byte[,] { { 0, 1 }, { 1, 0 } });
            var expected = new byte[,] { { 0, 0 }, { 0, 0 }};
            
            field.Reset();
            
            Assert.True(ArrayExtensions.AreEqual(field.Map, expected));
        }
    }
}