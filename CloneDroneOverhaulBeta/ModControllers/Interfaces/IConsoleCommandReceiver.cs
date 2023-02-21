namespace CDOverhaul
{
    public interface IConsoleCommandReceiver
    {
        string[] Commands();

        string OnCommandRan(string[] command);
    }
}
