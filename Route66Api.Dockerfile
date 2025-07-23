FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY Api ./Api
COPY Services ./Services
COPY Util ./Util
RUN dotnet restore ./Util/Util.csproj
RUN dotnet restore ./Services/Services.csproj
WORKDIR /app/Api
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Api.dll"]