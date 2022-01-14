namespace Rythm
{
    class Program
    {
        static void Main(string[] args)
        {
            new RythmBot().MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
