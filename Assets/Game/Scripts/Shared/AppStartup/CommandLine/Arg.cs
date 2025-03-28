namespace Shared.AppStartup.CommandLine
{
    public class Arg<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
    }
}