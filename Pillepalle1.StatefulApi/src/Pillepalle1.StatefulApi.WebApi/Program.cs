// ------------------------------------------------------------------------------------------------
// Configure services here
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSystemd();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
    options.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "Pillepalle1.StatefulApi", Version = "v1"
    });
});
builder.Services.AddApplication();

builder.ConfigureWebApi();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ------------------------------------------------------------------------------------------------
// Database initialization and migrations here
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await dbInitializer.InitializeAsync();
}

// ------------------------------------------------------------------------------------------------
// Endpoints for WebApi here

app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

app.RegisterBookEndpoints();

app.Run();
