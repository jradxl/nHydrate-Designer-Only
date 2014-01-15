@Echo Off
for /d /r . %%d in (bin,obj,.svn) do @if exist "%%d" rd /s/q "%%d"
pause

