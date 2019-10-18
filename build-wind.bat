rem Build on Windows
cd dotnet
dotnet build
certutil -hashfile bin\Debug\netcoreapp3.0\alzarsi.exe >bin\Debug\netcoreapp3.0\a_hash.txt
pause
