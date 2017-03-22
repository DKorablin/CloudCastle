
SET binPath="..\DocBuilder\bin\Debug\DocBuilder.exe"

RMDIR /S /Q ".\Docs\Html"

RMDIR /S /Q ".\Docs\Xml"

%binPath% /IX:bin\TestService.xml /O:".\Docs" /ID:bin\TestService.dll /R:"https://AlphaOmega.somee.com/v100" /T:14

if %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

XCOPY "..\Test\Template" ".\Docs" /S /Y

%binPath% /IX:".\Docs\Xml\T_*.xml" /IT:".\Docs\Xslt\service.xslt" /O:".\Docs\Html" /T:1

if %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

%binPath% /IX:".\Docs\Xml\M_*.xml" /IT:".\Docs\Xslt\method.xslt" /O:".\Docs\Html" /T:1

if %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

for /r ".\Docs\Html\" %%x in (*.xml) do REN "%%x" *.html