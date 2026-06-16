# ---------- Stage 1: build & publish ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + global.json, then each csproj for layered restore caching.
COPY Shopcms.sln global.json ./
COPY Cms.API/Cms.API.csproj Cms.API/
COPY Cms.Service/Cms.Service.csproj Cms.Service/
COPY Cms.Repository/Cms.Repository.csproj Cms.Repository/
RUN dotnet restore Cms.API/Cms.API.csproj

# Copy everything and publish the API project.
COPY . .
RUN dotnet publish Cms.API/Cms.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---------- Stage 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# The aspnet:8.0 image runs as a non-root user and listens on 8080 by default.
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Cms.API.dll"]
