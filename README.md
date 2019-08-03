# Primary.NET
> C# bindings for [Primary.API](http://api.primary.com.ar).

Provides a .NET friendly interface to interact with the Primary API. 
*NOTE: This project is still under development.*

[![Build status](https://ci.appveyor.com/api/projects/status/pm7payoayg80hr45?svg=true)](https://ci.appveyor.com/project/naicigam/primary-net)

## Features
- Supported assets list.
- Historical market data.
- Real-time market data.
- Submit and cancel orders.

## Short-term roadmap
- Nuget package.
- Documentation.
- Performance improvements.

### Building

This project targets .NET Core 2.2. 

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
- CI provided by [AppVeyor](https://www.appveyor.com/).
- This README was based on this [README template](https://github.com/jehna/readme-best-practices).

## Licensing

The code in this project is licensed under the [MIT license](https://choosealicense.com/licenses/mit/).