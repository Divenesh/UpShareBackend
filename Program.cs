using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

public partial class Program
{
    public static void Main(string[] args)
    {
        Server.Run(args);
    }
}