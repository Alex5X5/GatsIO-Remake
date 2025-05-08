@echo off
setlocal EnableDelayedExpansion

REM Check for Microsoft Visual C++ Redistributable x64
set "VC_NAME=Microsoft Visual C++ 2015-2022 Redistributable (x64)"
set "FOUND="
echo Checking for installed VC++ Redistributable...

REM Query installed programs
for /f "tokens=*" %%i in ('reg query "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall" /s /f "%VC_NAME%" 2^>nul ^| findstr /i "DisplayName"') do (
    set "FOUND=1"
)

if defined FOUND (
    echo Visual C++ Redistributable x64 is already installed.
    goto SKIP_VCPP
)

echo Visual C++ Redistributable x64 not found. Installing latest version...

REM Download latest VC++ x64 installer (2022 version as of 2025)
set "VC_URL=https://aka.ms/vs/17/release/vc_redist.x64.exe"
set "VC_FILE=%TEMP%\vc_redist.x64.exe"

::powershell -Command "Invoke-WebRequest -Uri '%VC_URL%' -OutFile '%VC_FILE%'"
if exist "%VC_FILE%" (
    echo Running installer...
    ::"%VC_FILE%" /quiet /norestart
    echo Installation complete.
) else (
    echo Failed to install VC++ Redistributable.
)

:SKIP_VCPP

echo Checking for .NET 9.0 runtime installation...

REM Find any installed dotnet runtime version 9.0.x
for /f "tokens=*" %%i in ('dotnet --list-runtimes ^| findstr "^Microsoft.NETCore.App 9.0."') do (
echo .NET 9.0 runtime found: %%i
goto :SKIP_DOTNET_INSTALL
)

echo .NET 9.0 runtime not found. Installing the latest version...

REM Define download URL (you can update this if needed)
set "dotnetInstallerUrl=https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-9.0.0-windows-x64-installer"

REM Temporary file path
set "dotnetInstallerPath=%TEMP%\dotnet-runtime-9.0.0-win-x64.exe"

REM Download the installer
powershell -Command "Invoke-WebRequest -Uri '%dotnetInstallerUrl%' -OutFile '%dotnetInstallerPath%'"

REM Run the installer silently
"%dotnetInstallerPath%" /install /quiet /norestart

REM Cleanup
del "%dotnetInstallerPath%"

:SKIP_DOTNET_INSTALL

echo Checking for Git is installation...

REM Try to locate git by running "git --version"
git --version >nul 2>&1
if %errorlevel%==0 (
echo Git is already installed.
goto :SKIP_GIT_INSTALL
)

echo Git is not installed. Downloading and installing the latest version...

REM Set temp installer path
set "gitInstallerPath=%TEMP%\git-latest.exe"

REM Use PowerShell to fetch the latest Git for Windows installer URL from GitHub
for /f "delims=" %%i in ('powershell -Command ^
"$url = Invoke-RestMethod 'https://api.github.com/repos/git-for-windows/git/releases/latest'; ^
$asset = $url.assets | Where-Object { $.name -like '*64-bit.exe' -and $.name -notlike 'portable' }; ^
$asset.browser_download_url"') do (
set "downloadUrl=%%i"
)

echo Downloading from: %downloadUrl%

REM Download the installer
powershell -Command "Invoke-WebRequest -Uri '%downloadUrl%' -OutFile '%gitInstallerPath%'"

REM Install Git silently
"%gitInstallerPath%" /VERYSILENT /NORESTART

REM Delete installer
del "%gitInstallerPath%"

echo Git installation complete.
:SKIP_GIT_INSTALL

REM Define folder and repository
set "GIT_FOLDER=%~dp0ShGame-Client"
set "REPO=https://github.com/Alex5X5/GatsIO-Remake.git"

REM Check if the GIT_FOLDER exists
if exist "%GIT_FOLDER%.git" (
echo Found existing Git repository in %GIT_FOLDER%. Pulling latest changes...
pushd "%GIT_FOLDER%"
git pull
popd
goto :FINISH_GIT_FETCH
)

if exist "%GIT_FOLDER%" (
echo GIT_FOLDER exists but is not a Git repository. Replacing it...
rmdir /s /q "%GIT_FOLDER%"
)

REM Clone fresh
echo Cloning repository into %GIT_FOLDER%...
git clone %REPO% "%GIT_FOLDER%"

:FINISH_GIT_FETCH
echo Pulling Git repository completed.

cd ShGame-Client

echo adding glfw package to ShGame...
dotnet add ShGame.csproj package glfw -v 3.4

echo Done.
pause

