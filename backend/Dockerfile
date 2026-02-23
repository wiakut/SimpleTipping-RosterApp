FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TippingApp.slnx ./
COPY src/TippingApp.Domain/TippingApp.Domain.csproj src/TippingApp.Domain/
COPY src/TippingApp.Application/TippingApp.Application.csproj src/TippingApp.Application/
COPY src/TippingApp.Infrastructure/TippingApp.Infrastructure.csproj src/TippingApp.Infrastructure/
COPY src/TippingApp.Api/TippingApp.Api.csproj src/TippingApp.Api/
COPY tests/TippingApp.Tests/TippingApp.Tests.csproj tests/TippingApp.Tests/
RUN dotnet restore TippingApp.slnx

COPY . .
RUN dotnet publish src/TippingApp.Api/TippingApp.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TippingApp.Api.dll"]
