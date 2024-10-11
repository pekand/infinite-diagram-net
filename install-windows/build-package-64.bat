rem @echo off
rmdir /S /Q .\files
rmdir /S /Q .\plugins

set RELEASE=..\Diagram\bin\x64\Release\net8.0-windows


mkdir .\files
mkdir .\plugins

copy ..\Diagram\InfiniteDiagram.ico .\files\
copy %RELEASE%\InfiniteDiagram.exe .\files\
copy %RELEASE%\InfiniteDiagram.dll .\files\
copy %RELEASE%\InfiniteDiagram.deps.json .\files\
copy %RELEASE%\InfiniteDiagram.pdb .\files\
copy %RELEASE%\InfiniteDiagram.runtimeconfig.json .\files\

mkdir plugins\FindUidPlugin
mkdir plugins\CreateDiagramPlugin
mkdir plugins\CreateDirectoryPlugin
mkdir plugins\CalcPlugin
mkdir plugins\ScriptingPlugin
mkdir plugins\CScriptingPlugin

copy %RELEASE%\plugins\CreateDiagramPlugin\CreateDiagramPlugin.deps.json .\plugins\CreateDiagramPlugin\
copy %RELEASE%\plugins\CreateDiagramPlugin\CreateDiagramPlugin.dll .\plugins\CreateDiagramPlugin\
copy %RELEASE%\plugins\CreateDiagramPlugin\CreateDiagramPlugin.pdb .\plugins\CreateDiagramPlugin\

copy %RELEASE%\plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.deps.json .\plugins\CreateDirectoryPlugin\
copy %RELEASE%\plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.dll .\plugins\CreateDirectoryPlugin\
copy %RELEASE%\plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.pdb .\plugins\CreateDirectoryPlugin\

copy %RELEASE%\plugins\FindUidPlugin\FindUidPlugin.deps.json .\plugins\FindUidPlugin\
copy %RELEASE%\plugins\FindUidPlugin\FindUidPlugin.dll .\plugins\FindUidPlugin\
copy %RELEASE%\plugins\FindUidPlugin\FindUidPlugin.pdb .\plugins\FindUidPlugin\

copy %RELEASE%\plugins\CalcPlugin\CalcPlugin.dll .\plugins\CalcPlugin\
copy %RELEASE%\plugins\CalcPlugin\CalcPlugin.deps.json .\plugins\CalcPlugin\
copy %RELEASE%\plugins\CalcPlugin\CalcPlugin.pdb .\plugins\CalcPlugin\
copy %RELEASE%\plugins\CalcPlugin\Microsoft.CodeAnalysis.CSharp.Scripting.dll .\plugins\CalcPlugin\
copy %RELEASE%\plugins\CalcPlugin\Microsoft.CodeAnalysis.CSharp.dll .\plugins\CalcPlugin\
copy %RELEASE%\plugins\CalcPlugin\Microsoft.CodeAnalysis.Scripting.dll .\plugins\CalcPlugin\
copy %RELEASE%\plugins\CalcPlugin\Microsoft.CodeAnalysis.dll .\plugins\CalcPlugin\

copy %RELEASE%\plugins\CScriptingPlugin\CScriptingPlugin.deps.json .\plugins\CScriptingPlugin\
copy %RELEASE%\plugins\CScriptingPlugin\CScriptingPlugin.dll .\plugins\CScriptingPlugin\
copy %RELEASE%\plugins\CScriptingPlugin\CScriptingPlugin.pdb .\plugins\CScriptingPlugin\
copy %RELEASE%\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.CSharp.Scripting.dll .\plugins\CScriptingPlugin\
copy %RELEASE%\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.CSharp.dll .\plugins\CScriptingPlugin\
copy %RELEASE%\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.Scripting.dll .\plugins\CScriptingPlugin\
copy %RELEASE%\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.dll .\plugins\CScriptingPlugin\

copy %RELEASE%\plugins\ScriptingPlugin\IronPython.Modules.dll .\plugins\ScriptingPlugin\
copy %RELEASE%\plugins\ScriptingPlugin\IronPython.SQLite.dll .\plugins\ScriptingPlugin\
copy %RELEASE%\plugins\ScriptingPlugin\IronPython.Wpf.dll .\plugins\ScriptingPlugin\
copy %RELEASE%\plugins\ScriptingPlugin\IronPython.dll .\plugins\ScriptingPlugin\
copy %RELEASE%\plugins\ScriptingPlugin\IronPython.zip .\plugins\ScriptingPlugin\
copy %RELEASE%\plugins\ScriptingPlugin\Microsoft.Dynamic.dll .\plugins\ScriptingPlugin\
copy %RELEASE%\plugins\ScriptingPlugin\Microsoft.Scripting.Metadata.dll .\plugins\ScriptingPlugin\
copy %RELEASE%\plugins\ScriptingPlugin\Microsoft.Scripting.dll .\plugins\ScriptingPlugin\
copy %RELEASE%\plugins\ScriptingPlugin\Mono.Unix.dll .\plugins\ScriptingPlugin\
copy %RELEASE%\plugins\ScriptingPlugin\ScriptingPlugin.deps.json .\plugins\ScriptingPlugin\
copy %RELEASE%\plugins\ScriptingPlugin\ScriptingPlugin.dll .\plugins\ScriptingPlugin\
copy %RELEASE%\plugins\ScriptingPlugin\ScriptingPlugin.pdb .\plugins\ScriptingPlugin\

rem call subscribe "files\InfiniteDiagram.exe"
rem call subscribe "files\InfiniteDiagram.dll"
rem call subscribe "plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.dll"
rem call subscribe "plugins\FindUidPlugin\FindUidPlugin.dll"
rem call subscribe "plugins\NcalcPlugin\NcalcPlugin.dll"
rem call subscribe "plugins\ScriptingPlugin\ScriptingPlugin.dll"
rem call subscribe "plugins\CScriptingPlugin\CScriptingPlugin.dll"

iscc /q create-installation-package-64.iss

rem call subscribe "output\infinite-diagram-install.exe"

sha256sum "output\infinite-diagram-install.exe" > "output\signature.txt"

pause
