# Assign parameters to variables
param (
    [string]$ConfigurationName,
    [string]$SolutionDir,
    [string]$OutDir,
    [string]$TargetPath
)

# Output the parameters for debugging
Write-Host "Pre build:"
Write-Host "ConfigurationName=>$ConfigurationName< "
Write-Host "SolutionDir=>$SolutionDir<"
Write-Host "OutDir=>$OutDir<"
Write-Host "TargetPath=>$TargetPath<"


### & MSBuild.exe "${SolutionDir}Plugins\CalcPlugin\CalcPlugin.csproj" /p:Configuration=$ConfigurationName /p:SolutionDir=$SolutionDir
### & MSBuild.exe "${SolutionDir}Plugins\CreateDiagramPlugin\CreateDiagramPlugin.csproj" /p:Configuration=$ConfigurationName /p:SolutionDir=$SolutionDir
### & MSBuild.exe "${SolutionDir}Plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.csproj" /p:Configuration=$ConfigurationName /p:SolutionDir=$SolutionDir
### & MSBuild.exe "${SolutionDir}Plugins\CScriptingPlugin\CScriptingPlugin.csproj" /p:Configuration=$ConfigurationName /p:SolutionDir=$SolutionDir
### & MSBuild.exe "${SolutionDir}Plugins\EmptyPlugin\EmptyPlugin.csproj" /p:Configuration=$ConfigurationName /p:SolutionDir=$SolutionDir
### & MSBuild.exe "${SolutionDir}Plugins\FindUidPlugin\FindUidPlugin.csproj" /p:Configuration=$ConfigurationName /p:SolutionDir=$SolutionDir
### & MSBuild.exe "${SolutionDir}Plugins\FirstPlugin\FirstPlugin.csproj" /p:Configuration=$ConfigurationName /p:SolutionDir=$SolutionDir
### & MSBuild.exe "${SolutionDir}Plugins\ScriptingPlugin\ScriptingPlugin.csproj" /p:Configuration=$ConfigurationName /p:SolutionDir=$SolutionDir