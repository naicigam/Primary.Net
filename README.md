# Primary.NET
> C# bindings for [Primary.API](http://api.primary.com.ar).

Provides a .NET friendly interface to interact with the Primary API. 

**This project is still under development.**

[![Build Status](https://ci.appveyor.com/api/projects/status/pm7payoayg80hr45?svg=true)](https://ci.appveyor.com/project/naicigam/primary-net)
[![NuGet Package](https://buildstats.info/nuget/Primary.Net?includePreReleases=true)](https://www.nuget.org/packages/Primary.Net/)

Documentation: https://finanzascodificadas.com/Primary.Net/

## Supported API features
- Currently traded assets list.
- Historical market data.
- Real-time market data.
- Real-time order data.
- Submit and cancel orders.

# Short-term roadmap
- Remove dependencies.
- Performance improvements.

## Building

This project targets .NET Core 3.1. 

You can build it using Visual Studio 2019, or using the command line:

```shell
dotnet restore
dotnet build
```

## Contributing

If you would like to contribute, please fork the repository and use a feature branch. Pull requests are welcomed.
Relevant test cases must be included in the PR.

## Aknowledgements
- The API is developed and maintained by [Primary](http://www.primary.com.ar).
- CI is provided by [AppVeyor](https://www.appveyor.com/).
- The documentation is generated with [DocFX](https://dotnet.github.io/docfx/) and was deployed using [this tutorial](https://blog.markvincze.com/build-and-publish-documentation-and-api-reference-with-docfx-for-net-core-projects/).
- This README was based on this [README template](https://github.com/jehna/readme-best-practices).

## Licensing

The code in this project is licensed under the [MIT license](https://choosealicense.com/licenses/mit/).