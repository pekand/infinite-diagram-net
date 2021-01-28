cd %~dp0
cd ..\packages\IronPython.StdLib.2.7.11\content\Lib\
zip -r ..\IronPython.zip *
7z a ..\IronPython.zip *

copy  ..\IronPython.zip ..\..\..\..\Diagram\bin\Debug\plugins\ScriptingPlugin\
copy  ..\IronPython.zip ..\..\..\..\Diagram\bin\Release\plugins\ScriptingPlugin\
pause

