FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ChipFactorySimulator.csproj", "./"]
RUN dotnet restore "ChipFactorySimulator.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ChipFactorySimulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChipFactorySimulator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir /generator_config
ENTRYPOINT ["dotnet", "ChipFactorySimulator.dll"]
