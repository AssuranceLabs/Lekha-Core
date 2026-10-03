var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.LekhaCore_API>("lekhacore-api");

builder.Build().Run();
