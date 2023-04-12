using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PokemonCore;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        s.AddDbContext<PokemonDbContext>(Options =>
        {
            Options.UseSqlite(Environment.GetEnvironmentVariable("ConnectionStrings"));
        });
        s.AddScoped<PokemonDbContext>();
    })
    .Build();

host.Run();

