FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SpecFlow.Benchmark/SpecFlow.Benchmark.csproj", "SpecFlow.Benchmark/"]
RUN dotnet restore "SpecFlow.Benchmark/SpecFlow.Benchmark.csproj"
COPY . .
WORKDIR "/src/SpecFlow.Benchmark"
RUN dotnet build "SpecFlow.Benchmark.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpecFlow.Benchmark.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpecFlow.Benchmark.dll"]
