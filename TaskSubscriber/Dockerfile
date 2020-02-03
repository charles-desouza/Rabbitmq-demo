FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["TaskSubscriber.csproj", "TaskSubscriber/"]
RUN dotnet restore "TaskSubscriber/TaskSubscriber.csproj"
COPY . "TaskSubscriber"
WORKDIR "/src/TaskSubscriber"
RUN dotnet build "TaskSubscriber.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TaskSubscriber.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TaskSubscriber.dll"]