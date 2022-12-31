#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebAPI_HD/WebAPI_HD.csproj", "WebAPI_HD/"]
RUN dotnet restore "WebAPI_HD/WebAPI_HD.csproj"
COPY . .
WORKDIR "/src/WebAPI_HD"
RUN dotnet build "WebAPI_HD.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAPI_HD.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPI_HD.dll"]