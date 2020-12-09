@ECHO OFF

CALL rmdir "%~dp0artifacts\" /s /q

set DOTNET_FRAMEWORK=4.0.30319
SET msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"

::"%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"

SET configuration=Release


:: Build the solution. Override the platform to account for running
:: from Visual Studio Tools command prompt (x64). Log quietly to the 
:: console and verbosely to a file.
%msbuild% OpiumNetStat.sln /nologo /p:Configuration=Release /p:Platform="x86" /t:Clean,Build /verbosity:minimal /flp:verbosity=diagnostic

:: get build output and copy out to root 
CALL xcopy "%~dp0\Opium.Installer\bin\Release\*.msi" "%~dp0artifacts\" /s /e /Y

IF NOT ERRORLEVEL 0 EXIT /B %ERRORLEVEL%