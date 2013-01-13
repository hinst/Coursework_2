$env:Path = $env.Path + ';C:\Users\hinst\Docs\Pro\PowerShellScripts'
. My7z.ps1
$ScriptPath = $MyInvocation.MyCommand.Path
$ScriptFolderPath = Split-Path $ScriptPath
Write-Host $ScriptFolderPath
Archive7z $ScriptFolderPath $VisualStudioProject