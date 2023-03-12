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
$filteredResultsFiles = Get-ChildItem -Path artifacts/results -File -Filter *.md
if ($filteredResultsFiles) {
    $resultsFile = $filteredResultsFiles.Name

    # Copy results file
    Copy-Item -Path artifacts/results/$resultsFile -Destination .

    # Rename file
    Rename-Item -Path $resultsFile -NewName report.md
}

if (!$KeepBenchmarksFiles) {
    if ($directoryFound = Get-ChildItem -Path $reportDir -Directory | Select-String -Pattern "artifacts") {
        Remove-Item -Path artifacts -Recurse > $null
    }
    if ($directoryFound = Get-ChildItem -Path $reportDir -Directory | Select-String -Pattern "bin") {
        Remove-Item -Path bin -Recurse > $null
    }
}

Set-Location -Path $originalDir