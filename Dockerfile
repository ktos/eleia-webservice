FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Eleia.WebService.csproj", "Eleia.WebService/"]
RUN dotnet restore "Eleia.WebService/Eleia.WebService.csproj"
COPY . "/src/Eleia.WebService"
WORKDIR "/src/Eleia.WebService"
RUN dotnet build "Eleia.WebService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Eleia.WebService.csproj" --no-restore -c Release -o /app

FROM base AS final
#RUN apt-get update && apt-get install -y --no-install-recommends curl

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Eleia.WebService.dll"]

#HEALTHCHECK CMD curl --fail http://localhost/health || exit