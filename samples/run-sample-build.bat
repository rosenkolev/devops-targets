dotnet pack ..\src\DevOps.Targets -c Debug -o .\out
dotnet pack ..\src\DevOps.Terminal -c Debug -o .\out

dotnet script build.csx