namespace Tutorial3_Task;

class EmptySystemException : Exception
{
    public EmptySystemException() : base("Operation system is not installed.") { }
}