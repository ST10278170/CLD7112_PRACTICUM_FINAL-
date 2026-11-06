# ============================================================
# AgriStreamIoT - Local Autoscaling Policy Simulation
# ============================================================

$CurrentInstances = 1
$MinimumInstances = 1
$MaximumInstances = 5

# Actual result collected from the successful load test
$TotalRequests = 200
$SuccessRate = 100
$AverageResponseTimeMs = 70.24
$MaximumResponseTimeMs = 147.48
$RequestsPerSecond = 12.35

Write-Host ""
Write-Host "============================================"
Write-Host " AGRISTREAM IOT SCALING SIMULATION"
Write-Host "============================================"
Write-Host ""
Write-Host "Current Instances:        $CurrentInstances"
Write-Host "Minimum Instances:        $MinimumInstances"
Write-Host "Maximum Instances:        $MaximumInstances"
Write-Host ""
Write-Host "Observed Load:"
Write-Host "Total Requests:           $TotalRequests"
Write-Host "Success Rate:             $SuccessRate %"
Write-Host "Average Response Time:    $AverageResponseTimeMs ms"
Write-Host "Maximum Response Time:    $MaximumResponseTimeMs ms"
Write-Host "Requests Per Second:      $RequestsPerSecond"
Write-Host ""

if ($MaximumResponseTimeMs -gt 120 -or $RequestsPerSecond -gt 10)
{
    $RecommendedInstances = 2
    $Action = "SCALE OUT"
    $Reason = "Peak response time or request throughput exceeded the local scaling threshold."
}
else
{
    $RecommendedInstances = 1
    $Action = "MAINTAIN"
    $Reason = "Observed load remains within the local performance threshold."
}

if ($RecommendedInstances -gt $MaximumInstances)
{
    $RecommendedInstances = $MaximumInstances
}

if ($RecommendedInstances -lt $MinimumInstances)
{
    $RecommendedInstances = $MinimumInstances
}

Write-Host "Scaling Decision:"
Write-Host "Action:                   $Action"
Write-Host "Current Instances:        $CurrentInstances"
Write-Host "Recommended Instances:    $RecommendedInstances"
Write-Host "Reason:                    $Reason"
Write-Host ""
Write-Host "============================================"
Write-Host " LOCAL SCALING SIMULATION COMPLETED"
Write-Host "============================================"
Write-Host ""

$Report = @"
AGRISTREAM IOT LOCAL SCALING SIMULATION

Current Instances: $CurrentInstances
Minimum Instances: $MinimumInstances
Maximum Instances: $MaximumInstances

Observed Load:
Total Requests: $TotalRequests
Success Rate: $SuccessRate %
Average Response Time: $AverageResponseTimeMs ms
Maximum Response Time: $MaximumResponseTimeMs ms
Requests Per Second: $RequestsPerSecond

Scaling Decision:
Action: $Action
Current Instances: $CurrentInstances
Recommended Instances: $RecommendedInstances
Reason: $Reason

NOTE:
This is a local simulation of an Azure-style scaling policy.
Actual Azure App Service autoscaling was not performed because
an active Azure subscription was not available.
"@

$Report |
    Out-File `
        ".\AgriStream_ScalingSimulation_Results.txt" `
        -Encoding UTF8

Write-Host "Evidence saved to:"
Write-Host "AgriStream_ScalingSimulation_Results.txt"
Write-Host ""