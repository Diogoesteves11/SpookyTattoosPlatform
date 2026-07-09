FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SpookyTattoos.sln", "./"]
COPY ["SpookyTattoos.API/SpookyTattoos.API.csproj", "SpookyTattoos.API/"]
COPY ["SpookyTattoos.Application/SpookyTattoos.Application.csproj", "SpookyTattoos.Application/"]
COPY ["SpookyTattoos.Domain/SpookyTattoos.Domain.csproj", "SpookyTattoos.Domain/"]
COPY ["SpookyTattoos.Infrastructure/SpookyTattoos.Infrastructure.csproj", "SpookyTattoos.Infrastructure/"]


RUN dotnet restore "SpookyTattoos.sln"

COPY . .
WORKDIR "/src/SpookyTattoos.API"
RUN dotnet publish "SpookyTattoos.API.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SpookyTattoos.API.dll"]