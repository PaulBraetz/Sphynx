# Sphynx #

Sphynx aims to combat nonhuman use of ASP.Net Core applications by injecting middleware into the request pipeline.

## Features ##
Currently, Sphynx supports these features:
* rate-based request rejection

## How To Use ##

Inject Sphynx using extensions on `IServiceCollection` an `IApplicationBuilder`
```cs
using Sphynx;

services.ConfigureSphynx(optionsBuilder => {
	optionsBuilder.Capacity = 25;
	optionsBuilder.RecoveryRate = TimeSpan.FromMillis(1000);
});

app.UseDefaultSphynx();
```