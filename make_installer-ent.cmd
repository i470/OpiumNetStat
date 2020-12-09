@ECHO OFF

pushd "%~dp0"

if exist Debug rd /s /q Debug
if exist Release rd /s /q Release
if exist Artifacts rd /s /q Artifacts

SET msbuild="%programfiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe"

SET configuration=Release

%msbuild% OpiumNetStat.sln /nologo /p:Configuration=Release /p:Platform="x86" /t:Clean,Build /verbosity:minimal /flp:verbosity=diagnostic

 
CALL xcopy "%~dp0\Opium.Installer\bin\Release\*.msi" "Artifacts\" /s /e /Y

:exit
popd
@echo on
