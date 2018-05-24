FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Grotesque/Grotesque.csproj Grotesque/
RUN dotnet restore Grotesque/Grotesque.csproj
COPY . .
WORKDIR /src/Grotesque
RUN dotnet build Grotesque.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Grotesque.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Grotesque.dll"]
