# Sphynx #

Sphynx aims to combat nonhuman use of ASP.Net Core applications by injecting middleware into the request pipeline.

## Features ##

Currently, Sphynx supports these features:
* rate-based request rejection

## How To Use ##

### Default Sphynx ###

Inject Sphynx using extensions on `IServiceCollection` and `IApplicationBuilder`
```cs
using Sphynx;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSphynx(optionsBuilder => {
	optionsBuilder.Capacity = 25;
	optionsBuilder.RecoveryRate = TimeSpan.FromMillis(1000);
});

//more code

var app = builder.Build();

app.UseDefaultSphynx();
```

### Logging Sphynx ###

Sphynx provides an implementation capable of logging request rejections:
```cs
using Sphynx;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSphynx(optionsBuilder => {
	optionsBuilder.Capacity = 25;
	optionsBuilder.RecoveryRate = TimeSpan.FromMillis(1000);
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//more code

var app = builder.Build();

app.UseLoggingSphynx();
```