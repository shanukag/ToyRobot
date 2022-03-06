using System.ComponentModel;
using System.Text;
using Results;
using ToyRobot.Models;
using ToyRobot.Results;
using ToyRobot.Utilities;

namespace ToyRobot;

public class ToyRobotGame
{
    private Command Command { get; set; }
    private GridSize GridSize { get; set; }
    private Robot Robot { get; set; }
    private GameStatus GameStatus { get; set; }
    private InputProcessor InputProcessor { get; set; }
    
    //A collection of delegates to execute mapped to command types
    private readonly Dictionary<Command, Func<IInputValidationResult, ProcessingResult>> CommandHandlers = new Dictionary<Command, Func<IInputValidationResult, ProcessingResult>>();

    /// <summary>
    /// Initialize a new instance of the game
    /// Set the next expected instruction to setup the arena 
    /// </summary>
    public ToyRobotGame()
    {
        Command = Command.Place;
        GameStatus = GameStatus.GameInitialized;
        GridSize = new GridSize(5, 5);
        this.InputProcessor = new InputProcessor(GameStatus);
        
        CommandHandlers = new Dictionary<Command, Func<IInputValidationResult, ProcessingResult>>()
        {
            {Command.Move, MoveRobot},
            {Command.Place, PlaceRobot},
            {Command.Rotate, RotateRobot},
            {Command.Report, ReportLocation}
        };
    }
    
    /// <summary>
    /// Take an input of a string, validate, detect the command type and execute accordingly
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public ProcessingResult ProcessInput(string input)
    {
        ProcessingResult result = new ProcessingResult();
        InputProcessor.SetInput(input);
        
        //Validate the input
        var validationResult = InputProcessor.ValidateInput();
        
        if(validationResult.Success)
        {
            this.Command = validationResult.Command;

            //If the game has been initialized, the first command must be to place a robot
            if (GameStatus == GameStatus.GameInitialized && Command != Command.Place)
            {
                result.ErrorMessage = "A robot must be placed first before executing any other command. Eg : Place 3,2,North";
            }
            else
            {
                //Detect the command type and execute relevant routines
                result = CommandHandlers[this.Command](validationResult);
            }
        }
        else
        {
            result.ErrorMessage = validationResult.ErrorMessage;
        }

        return result;
    }

    //Place the robot at a given co-ordinate facing the given direction
    private ProcessingResult PlaceRobot(IInputValidationResult inputValidationResult)
    {
        ProcessingResult result = new ProcessingResult();

        var resultConverted = (inputValidationResult as RobotPlaceInstructionValidationResult);
        int x = resultConverted.X, y = resultConverted.Y;
        var bearing = resultConverted.BearingType;

        //If a robot has already been placed and a bearing has not been provided in a subsequent place command, set the bearing to the current direction
        if (GameStatus == GameStatus.RobotPlaced && bearing == null)
            bearing = Robot.GetCurrentLocation().BearingType;
        
        var position = new Position(x, y, bearing.Value);
        var addValidationResult = ValidateRobotPlaceInstruction(position);
        
        if (addValidationResult.Success)
        {
            //If the game is in the initializing stage, place a new robot, if not, move the robot on the grid to the provided co-ordinates
            if (GameStatus == GameStatus.GameInitialized)
            {
                Robot = new Robot(position); 
            }
            else
            {
                Robot.Move(position);
            }

            this.GameStatus = GameStatus.RobotPlaced;
            InputProcessor.SetGameStatus(GameStatus);
            result.Success = true;
            result.Message = $"Robot placed successfully. Location - {Robot.GetCurrentLocation().X}, {Robot.GetCurrentLocation().Y}, {Robot.GetCurrentLocation().BearingType}";
        }
        else
        {
            result.ErrorMessage = addValidationResult.ErrorMessage;
        }

        return result;
    }

    /// <summary>
    /// Move the robot one unit forward from the current position
    /// Return an error message if the robot would fall out of the grid if moved 
    /// </summary>
    /// <param name="inputValidationResult"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private ProcessingResult MoveRobot(IInputValidationResult inputValidationResult)
    {
        ProcessingResult result = new ProcessingResult();
        int x = Robot.GetCurrentLocation().X, y = Robot.GetCurrentLocation().Y;
        
        switch (Robot.GetCurrentLocation().BearingType)
        {
            case BearingType.North:
                y++;
                break;
            case BearingType.East:
                x++;
                break;
            case BearingType.South:
                y--;
                break;
            case BearingType.West:
                x--;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (x > GridSize.MaxWidth || y > GridSize.MaxHeight || x < GridSize.MinWidth || y < GridSize.MinHeight)
        {
            result.ErrorMessage = "Moving from current location will push the robot over the grid, try changing direction using LEFT, RIGHT or Move the robot to a new location using PLACE X,Y,Direction";
        }
        else
        {
            Position position = new Position(x, y, Robot.GetCurrentLocation().BearingType);
            Robot.Move(position);
            result.Success = true;
            result.Message = $"Robot moved successfully. Location - {Robot.GetCurrentLocation().X}, {Robot.GetCurrentLocation().Y}, {Robot.GetCurrentLocation().BearingType}";

        }

        return result;
    }

    /// <summary>
    /// Rotate the robot to the given direction
    /// </summary>
    /// <param name="inputValidationResult"></param>
    /// <returns></returns>
    private ProcessingResult RotateRobot(IInputValidationResult inputValidationResult)
    {
        ProcessingResult result = new ProcessingResult();

        var direction = (inputValidationResult as RotateCommandValidationResult).Direction;
        try
        {
            Robot.Rotate(direction);
            result.Success = true;
            result.Message = $"Robot rotated successfully. Location - {Robot.GetCurrentLocation().X}, {Robot.GetCurrentLocation().Y}, {Robot.GetCurrentLocation().BearingType}";
        }
        catch (Exception e)
        {
            result.Message = e.Message;
        }
        
        return result;
    }

    /// <summary>
    /// Report the current location of the robot
    /// </summary>
    /// <param name="validationResult"></param>
    /// <returns></returns>
    private ProcessingResult ReportLocation(IInputValidationResult validationResult)
    {
        ProcessingResult result = new ProcessingResult
        {
            Success = true,
            Message = $"{Robot.GetCurrentLocation().X},{Robot.GetCurrentLocation().Y},{Robot.GetCurrentLocation().BearingType}"
        };

        return result;
    }

   
    /// <summary>
    /// Validates the command given to place a robot against the grid size 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private IInputValidationResult ValidateRobotPlaceInstruction(Position position)
    {
        IInputValidationResult inputValidationResult = new InputValidationResult();

        if (position.X > GridSize.MaxWidth)
        {
            inputValidationResult.ErrorMessage = $"Robot's X co-ordinate must be less than the grid width - {GridSize.MaxWidth}";
        }
        else if (position.Y > GridSize.MaxHeight)
        {
            inputValidationResult.ErrorMessage = $"Robot's Y co-ordinate must be less than the grid height - {GridSize.MaxHeight}";
        }
        else
        {
            inputValidationResult.Success = true;
        }

        return inputValidationResult;
    }
}

public enum BearingType
{
    North,
    East,
    South,
    West
}

public enum Command
{
    Place,
    Move,
    Rotate,
    Report
}

public enum Direction
{
    Left,
    Right
}

public enum GameStatus
{
    GameInitialized,
    RobotPlaced
}