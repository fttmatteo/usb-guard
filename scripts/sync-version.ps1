# sync-version.ps1
# Sincroniza la versión del .csproj con los archivos de documentación (.md)

$projectFile = "$PSScriptRoot/../src/USBGuard.App/USBGuard.App.csproj"
if (!(Test-Path $projectFile)) {
    Write-Error "No se encontró el archivo del proyecto en $projectFile"
    exit 1
}

# 1. Obtener versión del .csproj
[xml]$xml = Get-Content $projectFile
$version = $xml.Project.PropertyGroup.Version
if (!$version) {
    Write-Error "No se encontró el tag <Version> en el .csproj"
    exit 1
}

# Definir versión de mantenimiento (e.g. 1.1.x)
$versionParts = $version.Split('.')
$maintenanceVersion = "$($versionParts[0]).$($versionParts[1]).x"

Write-Host "🚀 USB Guard: Sincronizando versión $version (mantenimiento: $maintenanceVersion)..." -ForegroundColor Cyan

# Archivos a actualizar
$files = @(
    "README.md",
    "README.en.md",
    "SECURITY.md",
    "SECURITY.en.md"
)

foreach ($file in $files) {
    $filePath = "$PSScriptRoot/../$file"
    if (!(Test-Path $filePath)) {
        continue
    }

    $content = Get-Content $filePath -Raw -Encoding utf8

    # Actualizar Badge (shields.io)
    # Patrón: .NET-10.0-512BD4 (Ejemplo, pero buscaremos por versión si existiera badge de versión)
    # Para USB Guard, buscaremos el badge que diga "Version" o "Versión"
    $newBadgeName = if ($file -like "*.en.md") { "Version" } else { "Versión" }
    
    # Si no existe el badge de versión todavía, lo añadiremos o actualizaremos uno existente
    # Usaremos una lógica similar a la de AI Guardian para buscar patterns de Shields.io
    $badgeRegex = "img\.shields\.io/badge/(?:Versi[óo]n|Version)-[\d.]+-blue\.svg"
    $newBadge = "img.shields.io/badge/$newBadgeName-$version-blue.svg"
    
    if ($content -match $badgeRegex) {
        $content = $content -replace $badgeRegex, $newBadge
    }

    # Actualizar Tabla de Seguridad (e.g. | v1.1.x | ✅ |)
    $securityRegex = "\|[ ]*(v?[\d.x<>]+)[ ]*\|[ ]*(✅|❌|:white_check_mark:|:x:)[ ]*\|"
    
    if ($content -match $securityRegex) {
        # Reemplazamos la primera ocurrencia que contenga .x
        # Nota: PowerShell -replace usa regex.
        # Buscaremos específicamente la fila que ya tiene una versión con .x
        $found = $false
        $content = [regex]::Replace($content, $securityRegex, {
            param($m)
            if (!$found -and $m.Groups[1].Value -like "*.x") {
                $found = $true
                return "| v$maintenanceVersion | :white_check_mark: |"
            }
            return $m.Value
        })
    }

    $content | Set-Content $filePath -Encoding utf8
    Write-Host "✅ Actualizado: $file" -ForegroundColor Green
}

Write-Host "✨ Sincronización de documentación completada." -ForegroundColor Yellow
