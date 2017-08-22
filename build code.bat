:: run tests before build
cd code\BricksGame.Tests
dotnet xunit
cd ..\..

:: build sln from code/ and put it into unity assets
dotnet build code\BricksGame\BricksGame.csproj -c=Release -o="..\..\unity\Assets\Scripts\CustomAssemblies" -f=net35

:: wait 3 seconds just to review build output
timeout /t 3