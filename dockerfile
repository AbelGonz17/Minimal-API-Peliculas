# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY appsettings.json .

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80
EXPOSE 443


ENTRYPOINT ["dotnet", "MinimalAPIPelicula.dll"]