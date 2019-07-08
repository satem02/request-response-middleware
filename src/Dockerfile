FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["request-response-middleware.csproj", "./"]
RUN dotnet restore "./request-response-middleware.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "request-response-middleware.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "request-response-middleware.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "request-response-middleware.dll"]
