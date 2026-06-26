var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.QubeFin_Gateway>("qubefin-gateway");

builder.AddProject<Projects.QubeFin_Auth_Api>("qubefin-auth-api");

builder.AddProject<Projects.QubeFin_App_Api>("qubefin-app-api");

builder.AddProject<Projects.QubeFin_Global_Api>("qubefin-global-api");

builder.Build().Run();
