cd ..

########################################

function Copy-WithFullPath {
    param (
        [string]$Source,
        [string]$Destination
    )

    # Ensure the destination directory exists
    $destinationDir = Split-Path -Path $Destination
    if (!(Test-Path -Path $destinationDir)) {
        New-Item -ItemType Directory -Path $destinationDir -Force | Out-Null
    }

    # Copy the file and overwrite if necessary
    Copy-Item -Path $Source -Destination $Destination -Force
}
########################################

function New-SHA256Checksum {
    param(
        [Parameter(Mandatory=$true)]
        [string]$FilePath
    )

    if (-not (Test-Path -Path $FilePath -PathType Leaf)) {
        throw "Súbor nenájdený: $FilePath"
    }

    $h = Get-FileHash -Path $FilePath -Algorithm SHA256
    $hex = $h.Hash.ToLower()

    $fileName = [System.IO.Path]::GetFileName($FilePath)

    $outPath = "$FilePath.SHA256"

    $line = "{0}  {1}" -f $hex, $fileName

    $line | Out-File -FilePath $outPath -Encoding Ascii -Force

    return $outPath
}

########################################

& 7z a install-windows\IronPython.zip .\packages\ironpython.stdlib\3.4.1\content\lib\*

Copy-WithFullPath -Source "install-windows\IronPython.zip" -Destination "\Diagram\bin\x64\Debug\net9.0-windows7.0\plugins\ScriptingPlugin\IronPython.zip"
Copy-WithFullPath -Source "install-windows\IronPython.zip" -Destination "\Diagram\bin\x64\Release\net9.0-windows7.0\plugins\ScriptingPlugin\IronPython.zip"

########################################


$signtoolPath = 'signtool.exe'
$innoInstallPath = 'iscc'

$CERT_CODE = $env:CERT_CODE
Write-Host "CERT_CODE=>$CERT_CODE<"
$CERT_PWD = $env:CERT_PWD
Write-Host "CERT_PWD=>$CERT_PWD<"
$tag = git describe --tags --abbrev=0
Write-Output "TAG=>$tag<"

$paths = @(
	"Diagram\bin\x64\Release\net9.0-windows7.0\plugins\CalcPlugin\CalcPlugin.dll"
	"Diagram\bin\x64\Release\net9.0-windows7.0\plugins\CScriptingPlugin\CScriptingPlugin.dll"
	"Diagram\bin\x64\Release\net9.0-windows7.0\plugins\CreateDiagramPlugin\CreateDiagramPlugin.dll"
	"Diagram\bin\x64\Release\net9.0-windows7.0\plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.dll"
	"Diagram\bin\x64\Release\net9.0-windows7.0\plugins\EmptyPlugin\EmptyPlugin.dll"
	"Diagram\bin\x64\Release\net9.0-windows7.0\plugins\FindUidPlugin\FindUidPlugin.dll"
	"Diagram\bin\x64\Release\net9.0-windows7.0\plugins\FirstPlugin\FirstPlugin.dll"
	"Diagram\bin\x64\Release\net9.0-windows7.0\plugins\ScriptingPlugin\ScriptingPlugin.dll"
	"Diagram\bin\x64\Release\net9.0-windows7.0\InfiniteDiagram.dll"
	"Diagram\bin\x64\Release\net9.0-windows7.0\InfiniteDiagram.exe"
)

foreach ($path in $paths) {
    Write-Output "Processing: $path"
    if (Test-Path $path) {
        & $signtoolPath sign /fd SHA256 /f "$CERT_CODE" /p $CERT_PWD /tr http://timestamp.digicert.com /td sha256 /v "$path"
		& $signtoolPath verify /pa /v "$path"
    } else {
        Write-Output "Path does not exist: $path"
    }
}

########################################

$RELEASE="Diagram\bin\x64\Release\net9.0-windows7.0"

Copy-WithFullPath -Source "Diagram\InfiniteDiagram.ico"        -Destination "install-windows\files\InfiniteDiagram.ico"
Copy-WithFullPath -Source "$RELEASE\InfiniteDiagram.exe"                -Destination "install-windows\files\InfiniteDiagram.exe"               
Copy-WithFullPath -Source "$RELEASE\InfiniteDiagram.dll"                -Destination "install-windows\files\InfiniteDiagram.dll"               
Copy-WithFullPath -Source "$RELEASE\InfiniteDiagram.deps.json"          -Destination "install-windows\files\InfiniteDiagram.deps.json"         
Copy-WithFullPath -Source "$RELEASE\InfiniteDiagram.pdb"                -Destination "install-windows\files\InfiniteDiagram.pdb"               
Copy-WithFullPath -Source "$RELEASE\InfiniteDiagram.runtimeconfig.json" -Destination "install-windows\files\InfiniteDiagram.runtimeconfig.json"

Copy-WithFullPath -Source "$RELEASE\plugins\CreateDiagramPlugin\CreateDiagramPlugin.deps.json" -Destination "install-windows\plugins\CreateDiagramPlugin\CreateDiagramPlugin.deps.json"
Copy-WithFullPath -Source "$RELEASE\plugins\CreateDiagramPlugin\CreateDiagramPlugin.dll"       -Destination "install-windows\plugins\CreateDiagramPlugin\CreateDiagramPlugin.dll"
Copy-WithFullPath -Source "$RELEASE\plugins\CreateDiagramPlugin\CreateDiagramPlugin.pdb"       -Destination "install-windows\plugins\CreateDiagramPlugin\CreateDiagramPlugin.pdb"

Copy-WithFullPath -Source "$RELEASE\plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.deps.json" -Destination "install-windows\plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.deps.json"
Copy-WithFullPath -Source "$RELEASE\plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.dll"       -Destination "install-windows\plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.dll"
Copy-WithFullPath -Source "$RELEASE\plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.pdb"       -Destination "install-windows\plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.pdb"

Copy-WithFullPath -Source "$RELEASE\plugins\FindUidPlugin\FindUidPlugin.deps.json"  -Destination "install-windows\plugins\FindUidPlugin\FindUidPlugin.deps.json"
Copy-WithFullPath -Source "$RELEASE\plugins\FindUidPlugin\FindUidPlugin.dll"        -Destination "install-windows\plugins\FindUidPlugin\FindUidPlugin.dll"
Copy-WithFullPath -Source "$RELEASE\plugins\FindUidPlugin\FindUidPlugin.pdb"        -Destination "install-windows\plugins\FindUidPlugin\FindUidPlugin.pdb"

Copy-WithFullPath -Source "$RELEASE\plugins\CalcPlugin\CalcPlugin.dll"                              -Destination "install-windows\plugins\CalcPlugin\CalcPlugin.dll"                             
Copy-WithFullPath -Source "$RELEASE\plugins\CalcPlugin\CalcPlugin.deps.json" 						-Destination "install-windows\plugins\CalcPlugin\CalcPlugin.deps.json" 						
Copy-WithFullPath -Source "$RELEASE\plugins\CalcPlugin\CalcPlugin.pdb" 								-Destination "install-windows\plugins\CalcPlugin\CalcPlugin.pdb" 								
Copy-WithFullPath -Source "$RELEASE\plugins\CalcPlugin\Microsoft.CodeAnalysis.CSharp.Scripting.dll" -Destination "install-windows\plugins\CalcPlugin\Microsoft.CodeAnalysis.CSharp.Scripting.dll"
Copy-WithFullPath -Source "$RELEASE\plugins\CalcPlugin\Microsoft.CodeAnalysis.CSharp.dll" 			-Destination "install-windows\plugins\CalcPlugin\Microsoft.CodeAnalysis.CSharp.dll" 			
Copy-WithFullPath -Source "$RELEASE\plugins\CalcPlugin\Microsoft.CodeAnalysis.Scripting.dll" 		-Destination "install-windows\plugins\CalcPlugin\Microsoft.CodeAnalysis.Scripting.dll" 		
Copy-WithFullPath -Source "$RELEASE\plugins\CalcPlugin\Microsoft.CodeAnalysis.dll" 					-Destination "install-windows\plugins\CalcPlugin\Microsoft.CodeAnalysis.dll" 					

Copy-WithFullPath -Source "$RELEASE\plugins\CScriptingPlugin\CScriptingPlugin.deps.json"                  -Destination "install-windows\plugins\CScriptingPlugin\CScriptingPlugin.deps.json"                  
Copy-WithFullPath -Source "$RELEASE\plugins\CScriptingPlugin\CScriptingPlugin.dll"                        -Destination "install-windows\plugins\CScriptingPlugin\CScriptingPlugin.dll"                        
Copy-WithFullPath -Source "$RELEASE\plugins\CScriptingPlugin\CScriptingPlugin.pdb"                        -Destination "install-windows\plugins\CScriptingPlugin\CScriptingPlugin.pdb"                        
Copy-WithFullPath -Source "$RELEASE\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.CSharp.Scripting.dll" -Destination "install-windows\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.CSharp.Scripting.dll" 
Copy-WithFullPath -Source "$RELEASE\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.CSharp.dll"           -Destination "install-windows\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.CSharp.dll"           
Copy-WithFullPath -Source "$RELEASE\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.Scripting.dll"        -Destination "install-windows\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.Scripting.dll"        
Copy-WithFullPath -Source "$RELEASE\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.dll"                  -Destination "install-windows\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.dll"                  

Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\IronPython.Modules.dll"           -Destination "install-windows\plugins\ScriptingPlugin\IronPython.Modules.dll"          
Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\IronPython.SQLite.dll"            -Destination "install-windows\plugins\ScriptingPlugin\IronPython.SQLite.dll"           
Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\IronPython.Wpf.dll"               -Destination "install-windows\plugins\ScriptingPlugin\IronPython.Wpf.dll"              
Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\IronPython.dll"                   -Destination "install-windows\plugins\ScriptingPlugin\IronPython.dll"                  
Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\IronPython.zip"                   -Destination "install-windows\plugins\ScriptingPlugin\IronPython.zip"                  
Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\Microsoft.Dynamic.dll"            -Destination "install-windows\plugins\ScriptingPlugin\Microsoft.Dynamic.dll"           
Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\Microsoft.Scripting.Metadata.dll" -Destination "install-windows\plugins\ScriptingPlugin\Microsoft.Scripting.Metadata.dll"
Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\Microsoft.Scripting.dll"          -Destination "install-windows\plugins\ScriptingPlugin\Microsoft.Scripting.dll"         
Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\Mono.Unix.dll"                    -Destination "install-windows\plugins\ScriptingPlugin\Mono.Unix.dll"                   
Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\ScriptingPlugin.deps.json"        -Destination "install-windows\plugins\ScriptingPlugin\ScriptingPlugin.deps.json"       
Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\ScriptingPlugin.dll"              -Destination "install-windows\plugins\ScriptingPlugin\ScriptingPlugin.dll"             
Copy-WithFullPath -Source "$RELEASE\plugins\ScriptingPlugin\ScriptingPlugin.pdb"              -Destination "install-windows\plugins\ScriptingPlugin\ScriptingPlugin.pdb"             

########################################



& $innoInstallPath /q install-windows\Diagram.iss -dAppVersion=%TAG%

########################################

$TARGET1="install-windows\Output\infinite-diagram-install.exe"
& $signtoolPath sign /fd SHA256 /f "$CERT_CODE" /p $CERT_PWD /tr http://timestamp.digicert.com /td sha256 /v "$TARGET1"
& $signtoolPath verify /pa /v "$TARGET1"

New-SHA256Checksum -FilePath $TARGET1 

Read-Host "Press Enter to continue"
