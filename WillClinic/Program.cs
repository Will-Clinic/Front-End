﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WillClinic.Data;
using WillClinic.Models;

namespace WillClinic
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var host = CreateWebHostBuilder(args).Build();
            // Attempt to seed any tables which require seed data, such as the Library table
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                try
                {
                    //StartupDbInitializer.SeedData(services, userManager);
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                    SeedLibraries.Initialize(services);
                }
                catch
                {
                    services.GetService<ILogger>().LogCritical("Could not seed database!");
                    throw;
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    //Bring in user secrets to grab out Azure Key Vault endpoint
                    builder.AddUserSecrets<Startup>().Build();
                    var config = builder.Build();

                    if (!string.IsNullOrEmpty(config["AzureKeyVaultEndPoint"]))
                    {
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        var keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback));
                        builder.AddAzureKeyVault(
                            config["AzureKeyVaultEndPoint"], keyVaultClient, new DefaultKeyVaultSecretManager());
                    }
                })
                  .ConfigureLogging((context, logging) =>
                  {
                      logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                      logging.AddConsole();
                      logging.AddDebug();
                  })
                    .UseStartup<Startup>();

        //public static IWebHost BuildWebHost(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        // Configure IConfiguration for DI throughout the app. Note that any keys contained in
        //        // Azure Key Vault will be available using key/value pair access on IConfiguration objects.
        //        .ConfigureAppConfiguration((context, config) =>
        //        {
        //            // Bring in the appsettings.json and environment variables to IConfiguration for DI
        //            config.SetBasePath(Directory.GetCurrentDirectory())
        //                .AddJsonFile("appsettings.json", optional: false)
        //                .AddEnvironmentVariables();

        //            // Build the configuration to bring in key vault configuration
        //            var builtConfig = config.Build();

        //            // Add access to Azure Key Vault to IConfiguration for DI within the application.
        //            // Note that the the VaultName, ClientId, and ClientSecret keys must exist under
        //            // the AzureKeyVault object within the appsettings.json file in order to configure
        //            // access to the key vault, and this will throw an exception if your computer is
        //            // not connected to the internet.
        //            //config.AddAzureKeyVault(
        //            //    $"https://{builtConfig["AzureKeyVault:VaultName"]}.vault.azure.net/",
        //            //    builtConfig["AzureKeyVault:ClientId"],
        //            //    builtConfig["AzureKeyVault:ClientSecret"]);
        //        })

        //        .ConfigureLogging((context, logging) =>
        //        {
        //    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
        //    logging.AddConsole();
        //    logging.AddDebug();
        //})
        //        .UseStartup<Startup>()
        //        .Build();
    }
}
