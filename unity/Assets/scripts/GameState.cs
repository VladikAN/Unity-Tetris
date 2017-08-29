using System;

[Serializable]
public class GameState
{
    public int Score { get; set; }
    public byte[,] Field { get; set; }
    public byte[,] Figure { get; set; }
    public uint FigureX { get; set; }
    public uint FigureY { get; set; }
    public byte[,] Next { get; set; }
    public uint NextX { get; set; }
    public uint NextY { get; set; }
}