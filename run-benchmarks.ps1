param ([switch] $KeepBenchmarksFiles)

$globalTools = dotnet tool list -g
$grabTool = $globalTools | Select-String -Pattern "dotnet-grab"
$t4Tool = $globalTools | Select-String -Pattern "dotnet-t4"

if (!$grabTool) {
    dotnet tool install dotnet-grab --global --ignore-failed-sources
}

if (!$t4Tool) {
    dotnet tool install dotnet-t4 --global --ignore-failed-sources
}

$originalDir = (Get-Location).Path
$timestamp = [DateTime]::UtcNow.ToString("yyyyMMdd_hhmmss")

$directoryFound = Get-ChildItem -Path $originalDir -Directory | Select-String -Pattern "tmp"
if (!$directoryFound) {
    New-Item -Name "tmp" -ItemType Directory > $null
}

$directoryFound = Get-ChildItem -Path "$originalDir\\tmp" -Directory | Select-String -Pattern "benchmarks"
if (!$directoryFound) {
    New-Item -Name "benchmarks" -ItemType Directory -Path "$originalDir\\tmp" > $null
}

$directoryFound = Get-ChildItem -Path "$originalDir\\tmp\\benchmarks" -Directory | Select-String -Pattern $timestamp
if (!$directoryFound) {
    New-Item -Name $timestamp -ItemType Directory -Path "$originalDir\\tmp\\benchmarks" > $null
}

$reportDir = "$originalDir\\tmp\\benchmarks\\$timestamp"

# Ensure all packages restored before running benchmarks
dotnet restore rules-framework.sln

# Build benchmarks binaries
dotnet build -c Release .\tests\Rules.Framework.BenchmarkTests\Rules.Framework.BenchmarkTests.csproj -o "$reportDir\\bin" --framework net6.0

Set-Location -Path $reportDir

# Run benchmarks
bin\Rules.Framework.BenchmarkTests.exe -a artifacts

# Determine results file
$filteredResultsFiles = Get-ChildItem -Path "$reportDir/artifacts/results" -File -Filter *.json
if ($filteredResultsFiles) {
    $resultsFile = $filteredResultsFiles.Name

    # Get report dependencies
    grab newtonsoft.json@13.0.2

    # Generate report
    t4 -p:ResultsFile="$reportDir/artifacts/results/$resultsFile" -P="$reportDir/packages/newtonsoft.json/13.0.2/lib/net6.0" -r="Newtonsoft.Json.dll" `
        -o "$reportDir/report.md" "$originalDir/tests/Rules.Framework.BenchmarkTests/Results2Markdown/Report.tt"
}

if (!$KeepBenchmarksFiles) {
    if ($directoryFound = Get-ChildItem -Path $reportDir -Directory | Select-String -Pattern "artifacts") {
        Remove-Item -Path "$reportDir/artifacts" -Recurse > $null
    }
    if ($directoryFound = Get-ChildItem -Path $reportDir -Directory | Select-String -Pattern "bin") {
        Remove-Item -Path "$reportDir/bin" -Recurse > $null
    }
    if ($directoryFound = Get-ChildItem -Path $reportDir -Directory | Select-String -Pattern "packages") {
        Remove-Item -Path "$reportDir/packages" -Recurse > $null
    }
}

Set-Location -Path $originalDir