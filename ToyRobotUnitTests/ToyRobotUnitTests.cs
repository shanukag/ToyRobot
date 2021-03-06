using NUnit.Framework;
using ToyRobot;
using ToyRobot.Results;
using ToyRobot.Utilities;

namespace ToyRobotUnitTests;

public class Tests
{
    public ToyRobotGame ToyRobotGame { get; set; }
    
    [SetUp]
    public void Setup()
    {
        ToyRobotGame = new ToyRobotGame();
    }

    /// <summary>
    /// Tests the game from end to end. Providing a place command, move, report and asserting the final location of the robot
    /// </summary>
    /// <param name="robotPlaceCommand"></param>
    /// <param name="robotMoveCommand"></param>
    /// <param name="reportCommand"></param>
    /// <param name="robotPosition"></param>
    [Test]
    [TestCase( "Place 0,0,North", "move", "report", "0,1,North")]
    [TestCase( "Place 0,0,North",  "left", "report", "0,0,West")]
    public void TestGame(string robotPlaceCommand, string robotMoveCommand, string reportCommand, string robotPosition)
    {
        ToyRobotGame.ProcessInput(robotPlaceCommand);
        ToyRobotGame.ProcessInput(robotMoveCommand);
        var processingResult = ToyRobotGame.ProcessInput(reportCommand);

        Assert.That(processingResult.Message, Is.EqualTo(robotPosition));
    }

    /// <summary>
    /// Tests the input validation of the place command
    /// </summary>
    /// <param name="input"></param>
    /// <param name="gameStatus"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="bearingType"></param>
    /// <param name="inputProcessingError"></param>
    [Test]
    [TestCase("Place 1,2,East", GameStatus.GameInitialized, 1, 2, BearingType.East, false)]
    [TestCase("place 1,2,east", GameStatus.GameInitialized, 1, 2, BearingType.East, false)]
    [TestCase("Place 1,2", GameStatus.GameInitialized, 1, 2, BearingType.East, true)]
    [TestCase("Place 1,2", GameStatus.RobotPlaced, 1, 2, null, false)]
    [TestCase("Placeqwe 1,2,East", GameStatus.GameInitialized, 1, 2, BearingType.East, true)]
    public void TestRobotPlaceInstruction(string input, GameStatus gameStatus, int x, int y, BearingType? bearingType, bool inputProcessingError)
    {
        InputProcessor inputProcessor = new InputProcessor(GameStatus.GameInitialized);
        inputProcessor.SetInput(input);
        inputProcessor.SetGameStatus(gameStatus);

        RobotPlaceInstructionValidationResult result = (inputProcessor.ValidateInput() as RobotPlaceInstructionValidationResult);
        
        if (inputProcessingError)
        {
            Assert.That(result.Success, Is.EqualTo(false));
            Assert.That(result.ErrorMessage, Is.Not.Null);
        }
        else
        {
            Assert.That(result.X, Is.EqualTo(x));
            Assert.That(result.Y, Is.EqualTo(y));
            Assert.That(result.BearingType, Is.EqualTo(bearingType));    
        }
    }
    
    /// <summary>
    /// Tests the input validation of move command
    /// </summary>
    /// <param name="input"></param>
    /// <param name="inputProcessingError"></param>
    [Test]
    [TestCase("move", false)]
    [TestCase("move ", false)]
    [TestCase("mova ", true)]
    public void TestRobotMoveCommand(string input, bool inputProcessingError)
    {
        GameStatus gameStatus = GameStatus.GameInitialized;
        InputProcessor inputProcessor = new InputProcessor(gameStatus);
        inputProcessor.SetInput(input);
        inputProcessor.SetGameStatus(gameStatus);

        var inputValidationResult = inputProcessor.ValidateInput();

        if (inputProcessingError)
        {
            Assert.That(inputValidationResult.Success, Is.EqualTo(false));
            Assert.That(inputValidationResult.ErrorMessage, Is.Not.Null);
        }
        else
        {
            Assert.That(inputValidationResult.Command, Is.EqualTo(Command.Move));
        }
    }
    
    /// <summary>
    /// Tests the input validation of the left/right commands
    /// </summary>
    /// <param name="input"></param>
    /// <param name="expectedCommand"></param>
    /// <param name="expectedDirection"></param>
    /// <param name="inputProcessingError"></param>
    [Test]
    [TestCase("left", Command.Rotate, Direction.Left, false)]
    [TestCase("right", Command.Rotate, Direction.Right, false)]
    [TestCase("left ", Command.Rotate, Direction.Left, false)]
    [TestCase("lefta", Command.Rotate, Direction.Left, true)]
    public void TestRobotRotateCommand(string input, Command expectedCommand, Direction expectedDirection, bool inputProcessingError)
    {
        GameStatus gameStatus = GameStatus.GameInitialized;
        InputProcessor inputProcessor = new InputProcessor(gameStatus);
        inputProcessor.SetInput(input);
        inputProcessor.SetGameStatus(gameStatus);

        var inputValidationResult = (inputProcessor.ValidateInput() as RotateCommandValidationResult);

        if (inputProcessingError)
        {
            Assert.That(inputValidationResult.Success, Is.EqualTo(false));
            Assert.That(inputValidationResult.ErrorMessage, Is.Not.Null);
        }
        else
        {
            Assert.That(inputValidationResult.Command, Is.EqualTo(expectedCommand));
            Assert.That(inputValidationResult.Direction, Is.EqualTo(expectedDirection));
        }
    }
    
    /// <summary>
    /// Tests the input validation of the report command
    /// </summary>
    /// <param name="input"></param>
    /// <param name="inputProcessingError"></param>
    [Test]
    [TestCase("report", false)]
    [TestCase("report ", false)]
    [TestCase("reporte", true)]
    public void TestRobotReportCommand(string input, bool inputProcessingError)
    {
        GameStatus gameStatus = GameStatus.GameInitialized;
        InputProcessor inputProcessor = new InputProcessor(gameStatus);
        inputProcessor.SetInput(input);
        inputProcessor.SetGameStatus(gameStatus);

        var inputValidationResult = inputProcessor.ValidateInput();

        if (inputProcessingError)
        {
            Assert.That(inputValidationResult.Success, Is.EqualTo(false));
            Assert.That(inputValidationResult.ErrorMessage, Is.Not.Null);
        }
        else
        {
            Assert.That(inputValidationResult.Command, Is.EqualTo(Command.Report));
        }
    }
}