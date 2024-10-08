FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SpecFlow.PoC.Caching/SpecFlow.PoC.Caching.csproj", "SpecFlow.PoC.Caching/"]
RUN dotnet restore "SpecFlow.PoC.Caching/SpecFlow.PoC.Caching.csproj"
COPY . .
WORKDIR "/src/SpecFlow.PoC.Caching"
RUN dotnet build "SpecFlow.PoC.Caching.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpecFlow.PoC.Caching.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpecFlow.PoC.Caching.dll"]
