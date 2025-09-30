# PowerShell script to create warehouse data
$server = "DESKTOP-58PLL8E"
$database = "FertilizerWarehouseDB"

# Read SQL script
$sqlScript = Get-Content "Data/SimpleWarehouseData.sql" -Raw

# Split by GO statements
$statements = $sqlScript -split "GO|go" | Where-Object { $_.Trim() -ne "" }

# Execute each statement
foreach ($statement in $statements) {
    if ($statement.Trim() -ne "") {
        try {
            Invoke-Sqlcmd -ServerInstance $server -Database $database -Query $statement.Trim()
            Write-Host "Executed: $($statement.Substring(0, [Math]::Min(50, $statement.Length)))..."
        }
        catch {
            Write-Host "Error executing statement: $($_.Exception.Message)"
        }
    }
}

Write-Host "Warehouse data creation completed!"
