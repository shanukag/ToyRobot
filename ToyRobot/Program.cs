using System.Text;
using ToyRobot;

ToyRobotGame toyRobot = new ToyRobotGame();

Run();

void Run()
{
    WriteInstructions();
    
    while (true)
    {  
        var consoleInput = ReadFromConsole();
        if (string.IsNullOrWhiteSpace(consoleInput)) continue;
  
        try
        {
            if (HandleExitCommand(consoleInput))
            {
                ExitCommandHandler();
            }
            else
            {
                string result = Execute(consoleInput);
                WriteToConsole(result);
            }
        }
        catch (Exception ex)
        {
            WriteToConsole(ex.Message);
        }
    }
}

bool HandleExitCommand(string input)
{
    return (input.Equals("exit", StringComparison.InvariantCultureIgnoreCase) ||
        input.Equals("y", StringComparison.InvariantCultureIgnoreCase) ||
        input.Equals("n", StringComparison.InvariantCultureIgnoreCase)) ;
}

void ExitCommandHandler()
{
    WriteToConsole("Are you sure you want to exit? Type Y / N");
    string input = ReadFromConsole();

    if (!string.IsNullOrEmpty(input) && input.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
        Environment.Exit(0);
}

string Execute(string input)
{
    var result = toyRobot.ProcessInput(input);

    return result.Success ? result.Message : result.ErrorMessage;
}


void WriteToConsole(string message = "")
{
    if(!string.IsNullOrEmpty(message))
    {
        Console.WriteLine(message);
        Console.WriteLine();
    }
}


const string _readPrompt = "console> ";
string ReadFromConsole(string promptMessage = "")
{
    // Show a prompt, and get input:
    Console.Write(_readPrompt + promptMessage);
    return Console.ReadLine();
}

void WriteInstructions()
{
    StringBuilder builder = new StringBuilder();
            
    builder.Append("Welcome to ToyRobot. To continue, enter one of the following commands : ");
    builder.AppendLine();
    builder.Append("To place the robot : Place 3,2,North");
    builder.AppendLine();
    builder.Append("To move the robot : move");
    builder.AppendLine();
    builder.Append("To rotate the robot to the left: left");
    builder.AppendLine();
    builder.Append("To rotate the robot to the right: right");
    builder.AppendLine();
    builder.Append("To get the current location of the robot: report");
    builder.AppendLine();
    builder.Append("To exit : exit");
    
    WriteToConsole(builder.ToString());
}