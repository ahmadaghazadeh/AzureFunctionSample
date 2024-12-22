using FunctionApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
        Console.WriteLine("Startup.Configure() running!");

        //var keyVaultUrl = new Uri(Environment.GetEnvironmentVariable("KeyVaultUrl"));
        //var secretClient = new SecretClient(keyVaultUrl, new DefaultAzureCredential());
        //var cs = secretClient.GetSecret("sql").Value.Value;
        var connectionString = Environment.GetEnvironmentVariable("Default") ?? "";

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
    })
    .ConfigureServices(services => { })
    .Build();

host.Run();