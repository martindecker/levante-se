rem Build on Windows
cd dotnet
dotnet build
certutil -hashfile bin\Debug\netcoreapp3.0\levante-se.exe >bin\Debug\netcoreapp3.0\lev_hash.txt
pause
