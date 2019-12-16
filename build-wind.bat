rem Build on Windows
cd dotnet
dotnet build
certutil -hashfile bin\Debug\netcoreapp3.1\levante-se.exe >bin\Debug\netcoreapp3.1\lev_hash.txt
pause
