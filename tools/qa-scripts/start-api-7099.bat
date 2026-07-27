@echo off
set ASPNETCORE_URLS=https://localhost:7099
set ASPNETCORE_ENVIRONMENT=Development
dotnet run --no-launch-profile --project "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin\src\LAMAMedellin.API\LAMAMedellin.API.csproj" >> "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\api-7099.log" 2>&1
