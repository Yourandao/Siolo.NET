FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Siolo.NET/Siolo.NET.csproj", "Siolo.NET/"]
RUN dotnet restore "Siolo.NET/Siolo.NET.csproj"
COPY . .
WORKDIR "/src/Siolo.NET"
RUN dotnet build "Siolo.NET.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Siolo.NET.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Siolo.NET.dll"]