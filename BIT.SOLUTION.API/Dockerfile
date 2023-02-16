#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BIT.SOLUTION.API/BIT.SOLUTION.API.csproj", "BIT.SOLUTION.API/"]
COPY ["BIT.SOLUTION.BaseApi/BIT.SOLUTION.BaseApi.csproj", "BIT.SOLUTION.BaseApi/"]
COPY ["BIT.SOLUTION.DL/BIT.SOLUTION.DL.csproj", "BIT.SOLUTION.DL/"]
COPY ["BIT.SOLUTION.DbHepper/BIT.SOLUTION.DbHepper.csproj", "BIT.SOLUTION.DbHepper/"]
COPY ["BIT.SOLUTION.Common/BIT.SOLUTION.Common.csproj", "BIT.SOLUTION.Common/"]
COPY ["BIT.SOLUTION.Data/BIT.SOLUTION.Data.csproj", "BIT.SOLUTION.Data/"]
COPY ["BIT.SOLUTION.Lib/BIT.SOLUTION.Lib.csproj", "BIT.SOLUTION.Lib/"]
COPY ["BIT.SOLUTION.MODEL/BIT.SOLUTION.MODEL.csproj", "BIT.SOLUTION.MODEL/"]
COPY ["BIT.SOLUTION.SERVICE/BIT.SOLUTION.SERVICE.csproj", "BIT.SOLUTION.SERVICE/"]
RUN dotnet restore "BIT.SOLUTION.API/BIT.SOLUTION.API.csproj"
COPY . .
WORKDIR "/src/BIT.SOLUTION.API"
RUN dotnet build "BIT.SOLUTION.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BIT.SOLUTION.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BIT.SOLUTION.API.dll"]