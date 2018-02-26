## NuGet
### Publishing to NuGet
When you are ready to publish to NuGet follow these steps prior to publishing to the repository

1. Navigate to [https://ci.appveyor.com/project/ahoefling/CircleButtonMenu](https://ci.appveyor.com/project/ahoefling/CircleButtonMenu) and retrieve the latest `.nupkg` from the build
2. Publish `.nupkg` to NuGet

### Local Builds
When you run the cake build `build.ps1` there will be a `.nupkg` file generated in the Build directory which can be referenced from the solution for testing