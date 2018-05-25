FROM microsoft/aspnetcore:2.0-jessie AS base
WORKDIR /app
COPY app .
EXPOSE 80
ENTRYPOINT ["dotnet", "Grotesque.dll"]
