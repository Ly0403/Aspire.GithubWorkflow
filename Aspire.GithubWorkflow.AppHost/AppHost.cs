var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("compose");

var db = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("workflow");


builder.AddProject<Projects.OrderApi>("orderapi")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
