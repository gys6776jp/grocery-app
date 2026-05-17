FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY GroceryApp/*.csproj GroceryApp/
RUN dotnet restore GroceryApp/GroceryApp.csproj
COPY . .
RUN dotnet publish GroceryApp/GroceryApp.csproj -c Release -o /app/publish

FROM nginx:alpine AS runtime
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
