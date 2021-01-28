rmdir /S /Q .\files
rmdir /S /Q .\plugins
mkdir .\files
mkdir .\plugins

copy ..\Diagram\InfiniteDiagram.ico .\files\
copy ..\Diagram\bin\Release\InfiniteDiagram.exe .\files\
copy ..\Diagram\bin\Release\Fizzler.dll .\files\
copy ..\Diagram\bin\Release\Svg.dll .\files\

mkdir plugins
mkdir plugins\DropPlugin
mkdir plugins\FindUidPlugin
mkdir plugins\CreateDirectoryPlugin
mkdir plugins\NcalcPlugin
mkdir plugins\ScriptingPlugin
mkdir plugins\CScriptingPlugin


copy ..\Diagram\bin\Release\plugins\CScriptingPlugin\CScriptingPlugin.dll .\plugins\CScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.CSharp.Scripting.dll .\plugins\CScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.CSharp.dll .\plugins\CScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.Scripting.dll .\plugins\CScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\CScriptingPlugin\Microsoft.CodeAnalysis.dll .\plugins\CScriptingPlugin\

copy ..\Diagram\bin\Release\plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.dll .\plugins\CreateDirectoryPlugin\
copy ..\Diagram\bin\Release\plugins\DropPlugin\DropPlugin.dll .\plugins\DropPlugin\
copy ..\Diagram\bin\Release\plugins\FindUidPlugin\FindUidPlugin.dll .\plugins\FindUidPlugin\
copy ..\Diagram\bin\Release\plugins\NcalcPlugin\NCalc.dll .\plugins\NcalcPlugin\
copy ..\Diagram\bin\Release\plugins\NcalcPlugin\NcalcPlugin.dll .\plugins\NcalcPlugin\

copy ..\Diagram\bin\Release\plugins\ScriptingPlugin\IronPython.zip .\plugins\ScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\ScriptingPlugin\IronPython.dll .\plugins\ScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\ScriptingPlugin\IronPython.Modules.dll .\plugins\ScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\ScriptingPlugin\IronPython.SQLite.dll .\plugins\ScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\ScriptingPlugin\IronPython.Wpf.dll .\plugins\ScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\ScriptingPlugin\Microsoft.Dynamic.dll .\plugins\ScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\ScriptingPlugin\Microsoft.Scripting.dll .\plugins\ScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\ScriptingPlugin\Microsoft.Scripting.Metadata.dll .\plugins\ScriptingPlugin\
copy ..\Diagram\bin\Release\plugins\ScriptingPlugin\ScriptingPlugin.dll .\plugins\ScriptingPlugin\

call subscribe "files\InfiniteDiagram.exe"
call subscribe "files\InfiniteDiagram.dll"
call subscribe "plugins\CreateDirectoryPlugin\CreateDirectoryPlugin.dll"
call subscribe "plugins\DropPlugin\DropPlugin.dll"
call subscribe "plugins\FindUidPlugin\FindUidPlugin.dll"
call subscribe "plugins\NcalcPlugin\NcalcPlugin.dll"
call subscribe "plugins\ScriptingPlugin\ScriptingPlugin.dll"
call subscribe "plugins\CScriptingPlugin\CScriptingPlugin.dll"

iscc /q create-installation-package-64.iss

call subscribe "output\infinite-diagram-install.exe"

sha256sum "output\infinite-diagram-install.exe" > "output\signature.txt"

pause
