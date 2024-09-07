FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src/

COPY ./Tasker.DAL/Tasker.DAL.csproj ./Tasker.DAL/Tasker.DAL.csproj
COPY ./Tasker.BLL/Tasker.BLL.csproj ./Tasker.BLL/Tasker.BLL.csproj
COPY ./Tasker.API/Tasker.API.csproj ./Tasker.API/Tasker.API.csproj
RUN dotnet restore ./Tasker.API/

COPY ./Tasker.DAL/ ./Tasker.DAL/
COPY ./Tasker.BLL/ ./Tasker.BLL/
COPY ./Tasker.API/ ./Tasker.API/
RUN dotnet publish -c Release -o ./app/ ./Tasker.API/ --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app/
COPY --from=build /src/app/ .
ENV ASPNETCORE_HTTP_PORTS=5001
ENTRYPOINT [ "dotnet", "Tasker.API.dll" ]
