param(
    [string]$RentalBase = "http://localhost:8080",
    [string]$BillingBase = "http://localhost:8082",
    [string]$LocationId = "LOC-1",
    [int]$TrailerNumber = 1,
    [string]$CustomerId = "CUST-123",
    [string]$DesiredStartTime = "2026-01-14T18:00:00",
    [string]$ReturnTime = "2026-01-14T20:00:00",
    [switch]$LateReturn
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$rentalPath = Join-Path $root "services\rental-service\src\RentalService.Api"
$billingPath = Join-Path $root "services\billing-service\src\BillingService.Api"

function Kill-Port {
    param([int]$Port)
    $conns = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
    foreach ($c in $conns) {
        try { Stop-Process -Id $c.OwningProcess -Force -ErrorAction Stop } catch {}
    }
}

function Wait-Port {
    param([string]$Url)
    $uri = [Uri]$Url
    $host = $uri.Host
    $port = $uri.Port
    for ($i = 1; $i -le 30; $i++) {
        if (Test-NetConnection -ComputerName $host -Port $port -InformationLevel Quiet) { return }
        Start-Sleep -Milliseconds 500
    }
    throw "Timed out waiting for $Url"
}

function Start-ServiceProc {
    param([string]$Path, [string]$Name)
    Write-Host "Starting $Name ..."
    $proc = Start-Process -FilePath "dotnet" -ArgumentList "run" -WorkingDirectory $Path -PassThru -NoNewWindow
    return $proc
}

Write-Host "Stopping anything on 8080/8082..."
Kill-Port 8080
Kill-Port 8082

Write-Host "Restore & build billing..."
dotnet restore $billingPath | Out-Null
dotnet build $billingPath | Out-Null

Write-Host "Restore & build rental..."
dotnet restore $rentalPath | Out-Null
dotnet build $rentalPath | Out-Null

$billingProc = $null
$rentalProc = $null

try {
    $billingProc = Start-ServiceProc -Path $billingPath -Name "Billing (8082)"
    Wait-Port $BillingBase

    $rentalProc = Start-ServiceProc -Path $rentalPath -Name "Rental (8080)"
    Wait-Port $RentalBase

    Write-Host "Services ready. Running demo calls..."

    $returnTimeToUse = $ReturnTime
    if ($LateReturn) {
        $returnTimeToUse = (Get-Date $DesiredStartTime).AddHours(30).ToString("s")
        Write-Host "Using late return time: $returnTimeToUse"
    }

    $bookingBody = @{
        locationId       = $LocationId
        trailerNumber    = $TrailerNumber
        customerId       = $CustomerId
        desiredStartTime = $DesiredStartTime
        insuranceSelected = $true
    } | ConvertTo-Json

    $booking = Invoke-RestMethod -Method Post -Uri "$RentalBase/bookings" -ContentType "application/json" -Body $bookingBody
    $rentalId = $booking.rentalId
    Write-Host "Booking created. rentalId=$rentalId allowedEndTime=$($booking.allowedEndTime)"

    $start = Invoke-RestMethod -Method Post -Uri "$RentalBase/rentals/$rentalId/start"
    Write-Host "Rental started. status=$($start.status)"

    $returnBody = @{ returnTime = $returnTimeToUse } | ConvertTo-Json
    $returned = Invoke-RestMethod -Method Post -Uri "$RentalBase/rentals/$rentalId/return" -ContentType "application/json" -Body $returnBody
    Write-Host "Rental returned. status=$($returned.status) isLate=$($returned.isLate)"

    Write-Host "Demo complete. Press Ctrl+C to stop services manually, or wait for script to stop them."
}
finally {
    if ($rentalProc) {
        try { Stop-Process -Id $rentalProc.Id -Force } catch {}
    }
    if ($billingProc) {
        try { Stop-Process -Id $billingProc.Id -Force } catch {}
    }
}
