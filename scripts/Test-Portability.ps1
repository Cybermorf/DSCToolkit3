$ErrorActionPreference = 'Stop'
$tracked = git ls-files
$forbiddenFiles = $tracked | Where-Object { $_ -match '(^|/)(manual.*\.pdf|.*\.pfx|.*\.snk|.*\.dscampaign\.json)$' }
if ($forbiddenFiles) { throw "Private or generated files are tracked: $($forbiddenFiles -join ', ')" }

$textFiles = $tracked | Where-Object { $_ -match '\.(cs|xaml|csproj|json|md|yml|yaml|ps1)$' }
$machinePaths = foreach ($file in $textFiles) {
    Select-String -Path $file -Pattern '[A-Za-z]:\\Users\\|C:\\darksun\\' -SimpleMatch:$false
}
if ($machinePaths) { throw "Machine-specific path found: $($machinePaths.Path -join ', ')" }

Write-Host 'Portability checks passed.'
