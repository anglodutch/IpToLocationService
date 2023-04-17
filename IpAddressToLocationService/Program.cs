using IpAddressToLocation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.ResolveServiceProviderFactory
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDistributedMemoryCache();

// given more time I would create a way for other assemblies or classes to be inject automatically using a class implementing a specific interface rather than needing to specify things specifically here
// there are pros and cons to this. the Pros being that it makes each assembly autonomous and therefor  consumption in other apps easier but the cons being that the web app looses some control. 
// My opinion would be though that for internal assemblies it is appropriate. 
builder.ConfigureIpAddressToLocation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("MyRedisConStr");
        options.InstanceName = "SampleInstance";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
