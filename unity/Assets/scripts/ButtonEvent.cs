using System;

[Flags]
public enum ButtonEvent
{
    None = 0,
    Left = 1,
    Right = 2,
    Down = 4,
    Rotate = 8,
    PauseAndResume = 16,
    SaveAndExit = 32
}