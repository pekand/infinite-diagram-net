cd %~dp0
cd ..\packages\IronPython.StdLib.2.7.11\content\Lib\
zip -r ..\IronPython.zip *
7z a ..\IronPython.zip *
pause

