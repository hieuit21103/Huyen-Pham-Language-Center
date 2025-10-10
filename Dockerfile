# Dockerfile cho MsHuyenLC Solution
# Multi-stage build để tối ưu image size

# ============================================
# Stage 1: Build
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["MsHuyenLC.sln", "./"]

# Copy project files (csproj) để tận dụng Docker layer caching
COPY ["src/MsHuyenLC.API/MsHuyenLC.API.csproj", "src/MsHuyenLC.API/"]
COPY ["src/MsHuyenLC.Application/MsHuyenLC.Application.csproj", "src/MsHuyenLC.Application/"]
COPY ["src/MsHuyenLC.Domain/MsHuyenLC.Domain.csproj", "src/MsHuyenLC.Domain/"]
COPY ["src/MsHuyenLC.Infrastructure/MsHuyenLC.Infrastructure.csproj", "src/MsHuyenLC.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/MsHuyenLC.API/MsHuyenLC.API.csproj"

# Copy toàn bộ source code
COPY . .

# Build solution
WORKDIR "/src/src/MsHuyenLC.API"
RUN dotnet build "MsHuyenLC.API.csproj" -c Release -o /app/build

# ============================================
# Stage 2: Publish
# ============================================
FROM build AS publish
RUN dotnet publish "MsHuyenLC.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ============================================
# Stage 3: Runtime
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Tạo non-root user để chạy app (security best practice)
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser

# Copy published files từ publish stage
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "MsHuyenLC.API.dll"]
