# ============================================================
# AgriStreamIoT Load Test
# 10 concurrent users x 20 requests = 200 HTTP requests
# ============================================================

$Url = "https://localhost:7103/Telemetry"

$ConcurrentUsers = 10
$RequestsPerUser = 20
$TotalRequests = $ConcurrentUsers * $RequestsPerUser

Write-Host ""
Write-Host "============================================"
Write-Host " AGRISTREAM IOT LOAD TEST"
Write-Host "============================================"
Write-Host "Target URL:        $Url"
Write-Host "Concurrent Users:  $ConcurrentUsers"
Write-Host "Requests Per User: $RequestsPerUser"
Write-Host "Total Requests:    $TotalRequests"
Write-Host "============================================"
Write-Host ""

$TestStart = Get-Date

$Jobs = @()

for ($User = 1; $User -le $ConcurrentUsers; $User++)
{
    $Jobs += Start-Job -ArgumentList $Url, $RequestsPerUser, $User -ScriptBlock {

        param(
            $Url,
            $RequestsPerUser,
            $UserNumber
        )

        for ($Request = 1; $Request -le $RequestsPerUser; $Request++)
        {
            $RequestStart = Get-Date

            $StatusCode = & curl.exe `
                -k `
                -s `
                -o NUL `
                -w "%{http_code}" `
                $Url

            $RequestEnd = Get-Date

            $ElapsedMilliseconds =
                ($RequestEnd - $RequestStart).TotalMilliseconds

            $IsSuccess = ($StatusCode -eq "200")

            [PSCustomObject]@{
                UserNumber     = $UserNumber
                RequestNumber  = $Request
                Success        = $IsSuccess
                StatusCode     = $StatusCode
                ResponseTimeMs = [Math]::Round($ElapsedMilliseconds, 2)
            }
        }
    }
}

Write-Host "Load test running..."
Write-Host ""

$Jobs | Wait-Job | Out-Null

$Results = @()

foreach ($Job in $Jobs)
{
    $Results += Receive-Job -Job $Job
}

$Jobs | Remove-Job

$TestEnd = Get-Date

$TotalDuration =
    [Math]::Round(
        ($TestEnd - $TestStart).TotalSeconds,
        2
    )

$SuccessfulRequests =
    @(
        $Results |
        Where-Object { $_.Success -eq $true }
    ).Count

$FailedRequests =
    @(
        $Results |
        Where-Object { $_.Success -eq $false }
    ).Count

$SuccessfulResponseTimes =
    @(
        $Results |
        Where-Object { $_.Success -eq $true } |
        Select-Object -ExpandProperty ResponseTimeMs
    )

if ($SuccessfulResponseTimes.Count -gt 0)
{
    $AverageResponseTime =
        [Math]::Round(
            ($SuccessfulResponseTimes |
                Measure-Object -Average).Average,
            2
        )

    $MinimumResponseTime =
        ($SuccessfulResponseTimes |
            Measure-Object -Minimum).Minimum

    $MaximumResponseTime =
        ($SuccessfulResponseTimes |
            Measure-Object -Maximum).Maximum
}
else
{
    $AverageResponseTime = 0
    $MinimumResponseTime = 0
    $MaximumResponseTime = 0
}

$SuccessRate =
    [Math]::Round(
        ($SuccessfulRequests / $TotalRequests) * 100,
        2
    )

if ($TotalDuration -gt 0)
{
    $RequestsPerSecond =
        [Math]::Round(
            $SuccessfulRequests / $TotalDuration,
            2
        )
}
else
{
    $RequestsPerSecond = 0
}

Write-Host ""
Write-Host "============================================"
Write-Host " AGRISTREAM IOT LOAD TEST RESULTS"
Write-Host "============================================"
Write-Host ""
Write-Host "Target URL:             $Url"
Write-Host "Concurrent Users:       $ConcurrentUsers"
Write-Host "Requests Per User:      $RequestsPerUser"
Write-Host "Total Requests:         $TotalRequests"
Write-Host ""
Write-Host "Successful Requests:    $SuccessfulRequests"
Write-Host "Failed Requests:        $FailedRequests"
Write-Host "Success Rate:           $SuccessRate %"
Write-Host ""
Write-Host "Average Response Time:  $AverageResponseTime ms"
Write-Host "Minimum Response Time:  $MinimumResponseTime ms"
Write-Host "Maximum Response Time:  $MaximumResponseTime ms"
Write-Host ""
Write-Host "Total Test Duration:    $TotalDuration seconds"
Write-Host "Requests Per Second:    $RequestsPerSecond"
Write-Host ""
Write-Host "============================================"
Write-Host " LOAD TEST COMPLETED"
Write-Host "============================================"

$Summary = @"
AGRISTREAM IOT LOAD TEST RESULTS

Target URL: $Url
Concurrent Users: $ConcurrentUsers
Requests Per User: $RequestsPerUser
Total Requests: $TotalRequests

Successful Requests: $SuccessfulRequests
Failed Requests: $FailedRequests
Success Rate: $SuccessRate %

Average Response Time: $AverageResponseTime ms
Minimum Response Time: $MinimumResponseTime ms
Maximum Response Time: $MaximumResponseTime ms

Total Test Duration: $TotalDuration seconds
Requests Per Second: $RequestsPerSecond
"@

$Summary |
    Out-File `
        ".\AgriStream_LoadTest_Results.txt" `
        -Encoding UTF8

$Results |
    Export-Csv `
        ".\AgriStream_LoadTest_Details.csv" `
        -NoTypeInformation `
        -Encoding UTF8

Write-Host ""
Write-Host "Evidence files created:"
Write-Host "AgriStream_LoadTest_Results.txt"
Write-Host "AgriStream_LoadTest_Details.csv"
Write-Host ""