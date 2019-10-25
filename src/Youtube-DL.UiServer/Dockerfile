FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Youtube-DL.UiServer/Youtube-DL.UiServer.csproj", "Youtube-DL.UiServer/"]
RUN dotnet restore "Youtube-DL.UiServer/Youtube-DL.UiServer.csproj"
COPY . .
WORKDIR "/src/Youtube-DL.UiServer"
RUN dotnet build "Youtube-DL.UiServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Youtube-DL.UiServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Youtube-DL.UiServer.dll"]