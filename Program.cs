using DevDictionary.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IDictionaryService, DictionaryService>();
builder.Services.AddHttpClient<ITelexService, TelexService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowTelex", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

// Swagger / OpenAPI (optional, dev only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowTelex");


app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/", () => new
{
    service = "DevDictionary Bot",
    status = "running",
    endpoints = new
    {
        webhook = "/telex/webhook",
        health = "/telex/health"
    },
    description = "AI-powered developer dictionary bot for Telex.im"
});

app.Run();
