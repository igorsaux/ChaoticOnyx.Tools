dotnet pack ./src/ChaoticOnyx.Tools.Core/
dotnet tool uninstall --global ChaoticOnyx.Tools
dotnet tool install --global --add-source ./artifacts/bin/ChaoticOnyx.Tools.Core/ ChaoticOnyx.Tools