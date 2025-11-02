using DevDictionary.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IDictionaryService, DictionaryService>();
builder.Services.AddHttpClient<ITelexService, TelexService>();




var app = builder.Build();

// Swagger / OpenAPI (optional, dev only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
