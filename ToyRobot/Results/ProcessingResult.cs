namespace Results;

public class ProcessingResult : IResult
{
    public string Message { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}