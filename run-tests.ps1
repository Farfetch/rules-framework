function FindRulesFrameworkPackageVersion ([string] $projectFilePath) {
    $res = dotnet list $projectFilePath package | Select-String -Pattern '> Rules.Framework' -CaseSensitive -SimpleMatch
    $res -match '[0-9]+\.[0-9]+\.[0-9]+(-beta)?'
    return $matches[0]
}

$globalTools = dotnet tool list -g
$reportGeneratorTool = $globalTools | Select-String -Pattern "dotnet-reportgenerator-globaltool"

If (!$reportGeneratorTool) {
    dotnet tool install dotnet-reportgenerator-globaltool --global --ignore-failed-sources
}

$reportTimestamp = [DateTime]::UtcNow.ToString("yyyyMMdd_hhmmss")
$currentDir = (Get-Location).Path
$coverageDir = "$currentDir\\coverage-outputs\\$reportTimestamp\\"

# Run with current core "Rules.Framework" package version on other packages
dotnet test .\rules-framework.sln -m:1

# Keep original package versions
# $inMemoryOriginalPackageVersion = FindRulesFrameworkPackageVersion('src\Rules.Framework.Providers.InMemory\Rules.Framework.Providers.InMemory.csproj')
# $mongoDbOriginalPackageVersion = FindRulesFrameworkPackageVersion('src\Rules.Framework.Providers.MongoDb\Rules.Framework.Providers.MongoDb.csproj')

# Use project reference to "Rules.Framework" now
dotnet add src\Rules.Framework.Providers.InMemory\Rules.Framework.Providers.InMemory.csproj reference src\Rules.Framework\Rules.Framework.csproj
dotnet add src\Rules.Framework.Providers.MongoDb\Rules.Framework.Providers.MongoDb.csproj reference src\Rules.Framework\Rules.Framework.csproj

# Run again with project reference
dotnet test .\rules-framework.sln --collect:"XPlat Code Coverage" --results-directory:"$coverageDir" -m:1

# Remove project reference to "Rules.Framework"
dotnet remove src\Rules.Framework.Providers.InMemory\Rules.Framework.Providers.InMemory.csproj reference src\Rules.Framework\Rules.Framework.csproj
dotnet remove src\Rules.Framework.Providers.MongoDb\Rules.Framework.Providers.MongoDb.csproj reference src\Rules.Framework\Rules.Framework.csproj

# Restore original package versions
# dotnet add src\Rules.Framework.Providers.InMemory\Rules.Framework.Providers.InMemory.csproj package --version $inMemoryOriginalPackageVersion Rules.Framework
# dotnet add src\Rules.Framework.Providers.MongoDb\Rules.Framework.Providers.MongoDb.csproj package --version $mongoDbOriginalPackageVersion Rules.Framework

reportgenerator -reports:"$($coverageDir)*\\*.xml" -targetdir:"$($coverageDir)" -reporttypes:Cobertura

$coverageFile = "$($coverageDir)*.xml"
$coverageReportFolder = "$($currentDir)\\coverage-outputs\\report\\"
reportgenerator -reports:$coverageFile -targetdir:$coverageReportFolder
# start "$($coverageReportFolder)index.htm"