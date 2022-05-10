[CmdletBinding()]
param()

try {
    Push-Location "$PSScriptRoot/src"
    dotnet lambda package --configuration Release --function-architecture arm64 --output-package bin/lambda-package.zip
}
finally {
    Pop-Location
}