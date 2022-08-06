# Sphynx #

Sphynx aims to combat nonhuman use of ASP.Net Core applications by injecting middleware into the request pipeline.

## Features ##

Currently, Sphynx supports these features:
* rate-based request rejection

## Installation ##

Nuget Gallery: https://www.nuget.org/packages/RhoMicro.Sphynx

Package Manager: `Install-Package RhoMicro.Sphynx -Version 2.0.0`

.Net CLI: `dotnet add package RhoMicro.Sphynx --version 2.0.0`

## Versioning ##

Sphynx uses [Semantic Versioning 2.0.0](https://semver.org/).

## How To Use ##

Rate based request rejection allocates a capacity of requests to individual ip addresses. This capacity is decremented everytime a request is received from the address and incremented for each passing of the `RecoveryRate`, not exceeding the `InitialCapacity`. Whenever a given capacity is depleted, requests from the corresponding address will be rejected until `RecoveryTime` has passed at least once.

For example, given an `InitialCapacity` of 2 and a `RecoveryRate` of 1000ms, the capacity would never exceed 2 and recover by 1 every second. Sphynx would reject requests like so:

Delay Before Request | Capacity | Rejected
-------------------- | -------- | --------
0ms		     | 2        | No
0ms		     | 1        | No
0ms		     | 0        | Yes
1000ms		     | 1        | No
500ms		     | 0        | Yes
500ms                | 1        | No


### Default Sphynx ###

Inject Sphynx using extensions on `IServiceCollection` and `IApplicationBuilder`. Note that `DefaultSphynx` requires a `ILogger<DefaultSphynx>` to be registered for injection.
```cs
using Sphynx;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSphynx(optionsBuilder => {
	optionsBuilder.InitialCapacity = 25;
	optionsBuilder.RecoveryRate = TimeSpan.FromMilliseconds(1000);
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//more code

var app = builder.Build();

app.UseDefaultSphynx();
```
