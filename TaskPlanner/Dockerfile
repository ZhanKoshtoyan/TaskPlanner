FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TaskPlanner/TaskPlanner.csproj", "TaskPlanner/"]
RUN dotnet restore "TaskPlanner/TaskPlanner.csproj"
COPY . .
WORKDIR "/src/TaskPlanner"
RUN dotnet build "TaskPlanner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskPlanner.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskPlanner.dll"]
