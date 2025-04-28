using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReqresApi.Client.Model;
using TestAssignment.IService;
using TestAssignment.Service;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<ReqresApiOptions>(context.Configuration.GetSection("ReqresApi"));

        services.AddHttpClient<IExternalUserService, ExternalUserService>();

        services.AddLogging(builder => builder.ClearProviders());
    })
    .Build();

var client = host.Services.GetRequiredService<IExternalUserService>();

Console.WriteLine("Select the API ");
Console.WriteLine("1 for Pagination data ");
Console.WriteLine("2 for Individual user data ");


Start:
Console.WriteLine("Enter 1 to get all users, 2 to get a single user:");
var input = Console.ReadLine();
int type;
if (!int.TryParse(input, out type))
{
    Console.WriteLine("Invalid input. Please enter a number.");
    goto Start;
}

switch (type)
{
    case 1:
        var allUsers = await client.GetAllUsersAsync();
        foreach (var user in allUsers)
        {
            Console.WriteLine($"{user.first_name} {user.last_name} - {user.email}");
        }
        break;

    case 2:
        var singleUser = await client.GetUserByIdAsync(2); 
        Console.WriteLine($" First Name: {singleUser.first_name} \n Last Name:{singleUser.last_name} \n Email: {singleUser.email} \n AvatarUrl: {singleUser.avatar}");
        break;

    default:
        Console.WriteLine("Select Valid API option");
        break;
}

Console.WriteLine("\nDo you want to continue? (y/n):");
var answer = Console.ReadLine();
if (answer?.ToLower() == "y")
{
    goto Start;
}