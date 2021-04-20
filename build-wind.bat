rem Build on Windows
cd dotnet
dotnet build
certutil -hashfile bin\Debug\net6.0\levante-se.exe >bin\Debug\net6.0\lev_hash.txt
pause
