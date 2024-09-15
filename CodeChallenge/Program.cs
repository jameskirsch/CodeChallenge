using System.Threading.Tasks;
using CodeChallenge.Config;

namespace CodeChallenge;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = await new App().Configure(args);
        await app.RunAsync(); 
    }
}