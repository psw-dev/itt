FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
#EXPOSE 80
EXPOSE 2200
ENV ASPNETCORE_URLS http://*:2200

# Copy csproj and restore as distinct layers
#COPY src/ ./
COPY *.sln ./
# Copy project folders
COPY ./src/ ./src
COPY ./lib/ ./lib
COPY ./test/ ./test

#RUN dotnet restore ./psw.auth.common/*.csproj
#RUN dotnet restore ./psw.auth.api/*.csproj
RUN dotnet restore

#build
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
# RUN ls certs
ENTRYPOINT ["dotnet", "psw.itt.api.dll"]