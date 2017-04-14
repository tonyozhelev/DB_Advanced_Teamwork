namespace BetManager.Client
{
using BetManager.Data;
    class Startup
    {
        static void Main(string[] args)
        {
            var context = new BetManagerContext();
            context.Database.Initialize(true);
        }
    }
}
