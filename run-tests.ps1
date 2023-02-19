param ([switch] $OnlyRunUnitTests)

$globalTools = dotnet tool list -g
$reportGeneratorTool = $globalTools | Select-String -Pattern "dotnet-reportgenerator-globaltool"

If (!$reportGeneratorTool) {
    dotnet tool install dotnet-reportgenerator-globaltool --global --ignore-failed-sources
}

$reportTimestamp = [DateTime]::UtcNow.ToString("yyyyMMdd_hhmmss")
$currentDir = (Get-Location).Path
$coverageDir = "$currentDir\\coverage-outputs\\$reportTimestamp\\"

# Run with current core "Rules.Framework" package version on other packages
if ($OnlyRunUnitTests) {
    dotnet test .\rules-framework.sln -m:1 --filter 'Category=Unit'
} else {
    dotnet test .\rules-framework.sln -m:1
}

# Use project reference to "Rules.Framework" now
dotnet add src\Rules.Framework.Providers.InMemory\Rules.Framework.Providers.InMemory.csproj reference src\Rules.Framework\Rules.Framework.csproj
dotnet add src\Rules.Framework.Providers.MongoDb\Rules.Framework.Providers.MongoDb.csproj reference src\Rules.Framework\Rules.Framework.csproj
dotnet add src\Rules.Framework.WebUI\Rules.Framework.WebUI.csproj reference src\Rules.Framework\Rules.Framework.csproj

# Run again with project reference
if ($OnlyRunUnitTests) {
    dotnet test .\rules-framework.sln --collect:"XPlat Code Coverage" --results-directory:"$coverageDir" -m:1 --filter 'Category=Unit'
} else {
    dotnet test .\rules-framework.sln --collect:"XPlat Code Coverage" --results-directory:"$coverageDir" -m:1
}

# Remove project reference to "Rules.Framework"
dotnet remove src\Rules.Framework.Providers.InMemory\Rules.Framework.Providers.InMemory.csproj reference src\Rules.Framework\Rules.Framework.csproj
dotnet remove src\Rules.Framework.Providers.MongoDb\Rules.Framework.Providers.MongoDb.csproj reference src\Rules.Framework\Rules.Framework.csproj
dotnet remove src\Rules.Framework.WebUI\Rules.Framework.WebUI.csproj reference src\Rules.Framework\Rules.Framework.csproj

reportgenerator -reports:"$($coverageDir)*\\*.xml" -targetdir:"$($coverageDir)" -reporttypes:Cobertura

$coverageFile = "$($coverageDir)*.xml"
$coverageReportFolder = "$($currentDir)\\coverage-outputs\\report\\"
reportgenerator -reports:$coverageFile -targetdir:$coverageReportFolder
# start "$($coverageReportFolder)index.htm"