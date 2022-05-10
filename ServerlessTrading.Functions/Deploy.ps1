[CmdletBinding()]
param(
    [Parameter()]
    [ValidateSet(
        "serverless-trading-create-sample-trades",
        "serverless-trading-start-manual-trade-approval")]
    [string]$FunctionName,

    [Parameter()]
    [string]$Profile,

    [Parameter()]
    [string]$Region
)

try {
    Push-Location "$PSScriptRoot/src"
    dotnet lambda deploy-function `
        --profile $Profile `
        --region $Region `
        --configuration Release `
        --function-architecture arm64 `
        --function-name $FunctionName `
        --function-runtime dotnet6
}
finally {
    Pop-Location
}