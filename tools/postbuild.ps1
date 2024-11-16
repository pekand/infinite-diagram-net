# Assign parameters to variables
param (
    [string]$ConfigurationName,
    [string]$SolutionDir,
    [string]$OutDir,
    [string]$TargetPath
)

if ($ConfigurationName -eq "Release") {

# Output the parameters for debugging
Write-Host "Post build:"
Write-Host "ConfigurationName=>$ConfigurationName< "
Write-Host "SolutionDir=>$SolutionDir<"
Write-Host "OutDir=>$OutDir<"
Write-Host "TargetPath=>$TargetPath<"
$CERT_CODE = $env:CERT_CODE
Write-Host "CERT_CODE=>$CERT_CODE<"
$CERT_PWD = $env:CERT_PWD
Write-Host "CERT_PWD=>$CERT_PWD<"
$CERT_STRONG_NAME = $env:CERT_STRONG_NAME
Write-Host "CERT_STRONG_NAME=>$CERT_STRONG_NAME<"

$TARGET1="${SolutionDir}Diagram\${OutDir}InfiniteDiagram.exe"
Write-Host "TARGET1=>$TARGET1<"
$TARGET2="${SolutionDir}Diagram\${OutDir}InfiniteDiagram.dll"
Write-Host "TARGET2=>$TARGET2<"

### $snPath = 'sn.exe'
### & $snPath -R "$TARGET2" $CERT_STRONG_NAME
### & $snPath -v "$TARGET2"

if (Test-Path $CERT_CODE) {

$signtoolPath = 'signtool.exe'
& $signtoolPath sign /fd SHA256 /f "$CERT_CODE" /p $CERT_PWD /tr http://timestamp.digicert.com /td sha256 /v "$TARGET1"
& $signtoolPath sign /fd SHA256 /f "$CERT_CODE" /p $CERT_PWD /tr http://timestamp.digicert.com /td sha256 /v "$TARGET2"
& $signtoolPath verify /pa /v "$TARGET1"
& $signtoolPath verify /pa /v "$TARGET2"

}

}