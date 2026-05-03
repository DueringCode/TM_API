FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TMAPI_Backend.csproj", "./"]
RUN dotnet restore "TMAPI_Backend.csproj"

COPY . .
RUN dotnet publish "TMAPI_Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TMAPI_Backend.dll"]