# Sphynx #

Sphynx aims to combat nonhuman use of ASP.Net Core applications by injecting middleware into the request pipeline.

## Features ##

Currently, Sphynx supports these features:
* rate-based request rejection

## Installation ##

Nuget Gallery: https://www.nuget.org/packages/RhoMicro.Sphynx

Package Manager: `Install-Package RhoMicro.Sphynx -Version 1.0.1`

.Net CLI: `dotnet add package RhoMicro.Sphynx --version 1.0.1`

## How To Use ##

Rate based request rejection allocates an amount of requests to individual ip addresses. This amount is decremented everytime a request is received from the address and incremented according to the `RecoveryRate`. Whenever a given allocated amount is depleted, requests from the corresponding address will be rejected until `RecoveryTime` has passed at least once. 

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
