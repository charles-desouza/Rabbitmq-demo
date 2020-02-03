FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["TaskPublisher.Api.csproj", "TaskPublisher.Api/"]
RUN dotnet restore "TaskPublisher.Api/TaskPublisher.Api.csproj"
COPY . "TaskPublisher.Api"
WORKDIR "/src/TaskPublisher.Api"
RUN dotnet build "TaskPublisher.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TaskPublisher.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TaskPublisher.Api.dll"]