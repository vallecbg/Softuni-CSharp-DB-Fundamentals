namespace MyApp.Core.Contracts
{
    public interface ICommandIntepreter
    {
        string Read(string[] inputArgs);
    }
}
