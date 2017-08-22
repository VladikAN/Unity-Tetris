using BricksGame.Extensions;
using Xunit;

namespace BricksGame.Tests
{
    public class FigureTests
    {
        [Fact]
        public void Copy_Figure_Copied()
        {
            var figure = new Figure(new byte[,] { { 1, 1 } });
            figure.BuildNew(0, 0);

            var copy = figure.Copy();
            
            Assert.Equal(figure.X, copy.X);
            Assert.Equal(figure.Y, copy.Y);
            Assert.Equal(figure.Points, copy.Points);
        }

        [Fact]
        public void Rotate_Once_Rotated()
        {
            var figure = new Figure(new byte[,] { { 1, 1 } });
            var expected = new byte[,] { { 1 }, { 1 } };
            
            figure.Rotate();
            
            Assert.True(ArrayExtensions.AreEqual(figure.Points, expected));
        }

        [Fact]
        public void Rotate_Twice_Rotated()
        {
            var figure = new Figure(new byte[,] { { 1, 1 } });
            var expected = new byte[,] { { 1, 1 } };
            
            figure.Rotate(2);
            
            Assert.True(ArrayExtensions.AreEqual(figure.Points, expected));
        }
        
        [Fact]
        public void MoveLeftIfAllowed_ScreenEdge_False()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1, 1 }, { 1, 1 } });
            figure.X = 0;
            
            var result = figure.MoveLeftIfAllowed(field);

            Assert.False(result);
            Assert.Equal((uint)0, figure.X);
        }
        
        [Fact]
        public void MoveLeftIfAllowed_Locked_False()
        {
            var map = new byte[,]
            {
                { 1, 0, 0 },
                { 1, 0, 0 },
                { 1, 0, 0 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1, 1 }, { 1, 1 } });
            figure.X = 1;
            
            var result = figure.MoveLeftIfAllowed(field);
            
            Assert.False(result);
            Assert.Equal((uint)1, figure.X);
        }
        
        [Fact]
        public void MoveLeftIfAllowed_Allowed_True()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1, 1 }, { 1, 1 } });
            figure.X = 1;
            
            var result = figure.MoveLeftIfAllowed(field);
            
            Assert.True(result);
            Assert.Equal((uint)0, figure.X);
        }
        
        [Fact]
        public void MoveRightIfAllowed_ScreenEdge_False()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1, 1 }, { 1, 1 } });
            figure.X = 1;
            
            var result = figure.MoveRightIfAllowed(field);
            
            Assert.False(result);
            Assert.Equal((uint)1, figure.X);
        }
        
        [Fact]
        public void MoveRightIfAllowed_Locked_False()
        {
            var map = new byte[,]
            {
                { 0, 0, 1 },
                { 0, 0, 1 },
                { 0, 0, 1 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1, 1 }, { 1, 1 } });
            figure.X = 0;
            
            var result = figure.MoveLeftIfAllowed(field);
            
            Assert.False(result);
            Assert.Equal((uint)0, figure.X);
        }
        
        [Fact]
        public void MoveRightIfAllowed_Allowed_True()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1, 1 }, { 1, 1 } });
            figure.X = 0;
            
            var result = figure.MoveRightIfAllowed(field);
            
            Assert.True(result);
            Assert.Equal((uint)1, figure.X);
        }
        
        [Fact]
        public void MoveDownIfAllowed_ScreenEdge_False()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1, 1 }, { 1, 1 } });
            figure.Y = 1;
            
            var result = figure.MoveDownIfAllowed(field);
            
            Assert.False(result);
            Assert.Equal((uint)1, figure.Y);
        }
        
        [Fact]
        public void MoveDownIfAllowed_Locked_False()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 1, 1, 1 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1, 1 }, { 1, 1 } });
            figure.Y = 0;
            
            var result = figure.MoveDownIfAllowed(field);
            
            Assert.False(result);
            Assert.Equal((uint)0, figure.Y);
        }
        
        [Fact]
        public void MoveDownIfAllowed_Allowed_True()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1, 1 }, { 1, 1 } });
            figure.Y = 0;
            
            var result = figure.MoveDownIfAllowed(field);
            
            Assert.True(result);
            Assert.Equal((uint)1, figure.Y);
        }

        [Fact]
        public void RotateIfAllowed_ScreenEdge_False()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1 }, { 1 }, { 1 } });
            figure.X = 1;

            var expected = new byte[,] { { 1 }, { 1 }, { 1 } };
            
            var result = figure.RotateIfAllowed(field);
            
            Assert.False(result);
            Assert.True(ArrayExtensions.AreEqual(figure.Points, expected));
        }

        [Fact]
        public void RotateIfAllowed_Allowed_True()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1 }, { 1 }, { 1 } });
            figure.X = 0;

            var expected = new byte[,] { { 1, 1, 1 } };
            
            var result = figure.RotateIfAllowed(field);
            
            Assert.True(result);
            Assert.True(ArrayExtensions.AreEqual(figure.Points, expected));
        }

        [Fact]
        public void RotateIfAllowed_Locked_False()
        {
            var map = new byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 1 }
            };
            var field = new Field(map);
            
            var figure = new Figure(new byte[,] { { 1 }, { 1 }, { 1 } });
            figure.X = 0;

            var expected = new byte[,] { { 1 }, { 1 }, { 1 } };
            
            var result = figure.RotateIfAllowed(field);
            
            Assert.False(result);
            Assert.True(ArrayExtensions.AreEqual(figure.Points, expected));
        }
    }
}