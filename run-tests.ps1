$globalTools = dotnet tool list -g
$reportGeneratorTool = $globalTools | Select-String -Pattern "dotnet-reportgenerator-globaltool"

If (!$reportGeneratorTool) {
    dotnet tool install --global dotnet-reportgenerator-globaltool
}

$reportTimestamp = [DateTime]::UtcNow.ToString("yyyyMMdd_hhmmss")
$currentDir = (Get-Location).Path
$coverageDir = "$currentDir\\coverage-outputs\\$reportTimestamp\\"

dotnet test .\rules-framework.sln /p:CollectCoverage=true /p:CoverletOutputFormat="opencover%2cjson" /p:CoverletOutput=$coverageDir /p:MergeWith="$coverageDir/coverage.json" -m:1

$coverageFile = "$($coverageDir)coverage.opencover.xml"
$coverageReportFolder = "$($currentDir)\\coverage-outputs\\report\\"
reportgenerator -reports:$coverageFile -targetdir:$coverageReportFolder
# start "$($coverageReportFolder)index.htm"