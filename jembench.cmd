@echo off
set OLDPATH=%PATH%
set PATH=%PATH%;%cd%\x64\Debug
dotnet .\x64\Release\netcoreapp2.0\jemalloc.Cli.dll %*
set PATH=%OLDPATH%
:end