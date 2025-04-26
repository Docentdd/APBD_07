namespace Tutorial3_Task;

class ConnectionException : Exception
{
    public ConnectionException() : base("Wrong netowrk name.") { }
}