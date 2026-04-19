<#
.SYNOPSIS
    Incrementa la versión de USB Guard y sincroniza la documentación.

.PARAMETER Type
    Tipo de incremento: Patch, Minor, o Major.

.EXAMPLE
    .\scripts\bump-version.ps1 -Type Patch
#>
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("Patch", "Minor", "Major")]
    [string]$Type
)

$projectFile = "$PSScriptRoot/../src/USBGuard.App/USBGuard.App.csproj"

if (!(Test-Path $projectFile)) {
    Write-Error "No se encontró el archivo del proyecto."
    exit 1
}

# 1. Leer versión actual
[xml]$xml = Get-Content $projectFile
$currentVersion = $xml.Project.PropertyGroup.Version
if (!$currentVersion) { $currentVersion = "1.0.0" }

$v = [version]$currentVersion
$newVersion = ""

switch ($Type) {
    "Major" { $newVersion = "$($v.Major + 1).0.0" }
    "Minor" { $newVersion = "$($v.Major).$($v.Minor + 1).0" }
    "Patch" { $newVersion = "$($v.Major).$($v.Minor).$($v.Build + 1)" }
}

Write-Host " b  Bumping version: $currentVersion -> $newVersion ($Type)" -ForegroundColor Cyan

# 2. Actualizar .csproj
$xml.Project.PropertyGroup.Version = $newVersion
$xml.Save($projectFile)

# 3. Sincronizar Documentación
& "$PSScriptRoot/sync-version.ps1"

# 4. Recordatorio de Changelog
Write-Host "`n i  No olvides actualizar el CHANGELOG.md para la versión $newVersion" -ForegroundColor Yellow
Write-Host " ✨ Proceso completado correctamente." -ForegroundColor Green
