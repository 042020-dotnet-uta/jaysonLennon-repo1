FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-alpine
COPY StoreApp/bin/Release/netcoreapp3.1/linux-musl-x64 /app
WORKDIR /app
ENTRYPOINT [ "./StoreApp" ]