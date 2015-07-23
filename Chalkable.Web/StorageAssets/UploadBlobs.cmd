@echo off
setlocal 
%~d0
cd "%~dp0"
set installUtilDir=%WINDIR%\Microsoft.NET\Framework\v2.0.50727
powershell.exe -NonInteractive -command ".\UploadBlobs.ps1"