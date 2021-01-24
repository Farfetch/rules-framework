$globalTools = dotnet tool list -g
$reportGeneratorTool = $globalTools | Select-String -Pattern "dotnet-reportgenerator-globaltool"

If (!$reportGeneratorTool) {
    dotnet tool install --global dotnet-reportgenerator-globaltool
}

$reportTimestamp = [DateTime]::UtcNow.ToString("yyyyMMdd_hhmmss")
$currentDir = (Get-Location).Path
$coverageDir = "$currentDir\\coverage-outputs\\$reportTimestamp\\"

dotnet test .\rules-framework.sln --collect:"XPlat Code Coverage" --results-directory:"$coverageDir" -m:1

reportgenerator -reports:"$($coverageDir)*\\*.xml" -targetdir:"$($coverageDir)" -reporttypes:Cobertura

$coverageFile = "$($coverageDir)*.xml"
$coverageReportFolder = "$($currentDir)\\coverage-outputs\\report\\"
reportgenerator -reports:$coverageFile -targetdir:$coverageReportFolder
# start "$($coverageReportFolder)index.htm"