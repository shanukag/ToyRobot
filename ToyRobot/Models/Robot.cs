namespace ToyRobot.Models;

public class Robot
{
    private Position _currentPosition;

    private Position CurrentPosition
    {
        get => _currentPosition;
        set => _currentPosition = value;
    }

    public Robot(Position currentPosition)
    {
        _currentPosition = currentPosition;
    }

    public Position GetCurrentLocation() => _currentPosition;

    public void SetLocation(int x, int y, BearingType bearing)
    {
        Position position = new Position(x, y, bearing);
        _currentPosition = position;
    }

    /// <summary>
    /// Takes in a command to move left/right and changes the bearing of the robot depending on the current bearing
    /// </summary>
    /// <param name="movementInstruction"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Rotate(Direction direction)
    {
        _currentPosition.BearingType = _currentPosition.BearingType switch
        {
            BearingType.North => direction switch
            {
                Direction.Left => BearingType.West,
                Direction.Right => BearingType.East,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            },
            BearingType.East => direction switch
            {
                Direction.Left => BearingType.North,
                Direction.Right => BearingType.South,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            },
            BearingType.South => direction switch
            {
                Direction.Left => BearingType.East,
                Direction.Right => BearingType.West,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            },
            BearingType.West => direction switch
            {
                Direction.Left => BearingType.South,
                Direction.Right => BearingType.North,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    /// <summary>
    /// Moves the robot to a provided position
    /// </summary>
    /// <param name="movementInstruction"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Move(Position position)
    {
        this.CurrentPosition = position;
    }
}