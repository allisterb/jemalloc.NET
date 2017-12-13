@echo off
@setlocal
set ERROR_CODE=0
dotnet restore jemalloc.NET.sln
if not %ERRORLEVEL%==0  (
    echo Error restoring NuGet packages for jemalloc.NET.sln.
    set ERROR_CODE=1
    goto End
)
if [%1]==[] (
    msbuild jemalloc\msvc\projects\vc2017\jemalloc\jemalloc.vcxproj /p:Configuration=Debug /p:Platform=x64
) else (
	msbuild jemalloc.NET.sln /p:Configuration=Benchmark /p:Platform=x64;%*
)
if not %ERRORLEVEL%==0  (
    echo Error building jemalloc.NET.sln.
    set ERROR_CODE=2
    goto End
)

:End
@endlocal
exit /B %ERROR_CODE%

