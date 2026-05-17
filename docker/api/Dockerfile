FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY GroceryApp.API/GroceryApp.API.csproj GroceryApp.API/
COPY GroceryApp.Application/GroceryApp.Application.csproj GroceryApp.Application/
COPY GroceryApp.Domain/GroceryApp.Domain.csproj GroceryApp.Domain/
COPY GroceryApp.Infrastructure/GroceryApp.Infrastructure.csproj GroceryApp.Infrastructure/
RUN dotnet restore GroceryApp.API/GroceryApp.API.csproj
COPY . .
RUN dotnet publish GroceryApp.API/GroceryApp.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "GroceryApp.API.dll"]
