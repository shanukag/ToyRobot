namespace ToyRobot.Models;

public class GridSize
{
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }
    public int MinWidth { get; set; }
    public int MinHeight { get; set; }

    public GridSize(int maxWidth, int maxHeight)
    {
        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
        MinHeight = 0;
        MinWidth  = 0;
    }
}