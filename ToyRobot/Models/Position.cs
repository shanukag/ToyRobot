namespace ToyRobot.Models;

public class Position
{
    public Position(int x, int y, BearingType bearingType)
    {
        X = x;
        Y = y;
        BearingType = bearingType;
    }

    public int X { get; set; }
    public int Y { get; set; }
    public BearingType BearingType { get; set; }
}