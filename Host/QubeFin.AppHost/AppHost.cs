var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.QubeFin_Gateway>("qubefin-gateway");

builder.AddProject<Projects.QubeFin_Auth_Api>("qubefin-auth-api");

builder.Build().Run();
