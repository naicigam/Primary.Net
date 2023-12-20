# Primary.NET
> C# bindings for [Primary.API](http://api.primary.com.ar).

Provides a .NET friendly interface to interact with the Primary API. 

**This project is still under development.**

[![Build Status](https://github.com/naicigam/Primary.Net/workflows/CI%20Build/badge.svg)](https://github.com/naicigam/Primary.Net/actions)
![Code Coverage](https://img.shields.io/badge/Code%20Coverage-78%25-success?style=flat)
[![NuGet Package](https://buildstats.info/nuget/Primary.Net?includePreReleases=true)](https://www.nuget.org/packages/Primary.Net/)

Documentation: https://finanzascodificadas.com/Primary.Net/

## Supported API features
- Currently traded assets list.
- Historical market data.
- Real-time market data.
- Real-time order data.
- Submit, update and cancel orders.
- Accounts: available accounts, positions and statements.

# Roadmap
- Remove dependencies.
- Performance improvements.

## Building

This project targets .NET Core 6.0. 

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
- CI is provided by [Github Actions](https://github.com/features/actions).
- The documentation is generated with [DocFX](https://dotnet.github.io/docfx/) and was deployed using [this tutorial](https://blog.taranissoftware.com/document-your-net-code-with-docfx-and-github-actions).
- This README was based on this [README template](https://github.com/jehna/readme-best-practices).

## Licensing

The code in this project is licensed under the [MIT license](https://choosealicense.com/licenses/mit/). 
