# Login to Azure.
Login-AzureRmAccount

# Use the SuperSport subscription.
Set-AzureRmContext -SubscriptionId "1b355cd4-4b64-4890-a31b-840bafbb63b1"

$resourceGroup = "SuperSport-SSDE-Prod"
$sqlServerName = "awe-ssde-p-sql-server"
$location = "West Europe"
$credentials = Get-Credential

New-AzureRmSqlServer `
    -ResourceGroupName $resourceGroup `
    -ServerName $sqlServerName `
    -SqlAdministratorCredentials $credentials `
    -Location $location

New-AzureRmSqlDatabase `
    -DatabaseName "SuperSportDataEngine_Hangfire" `
    -ResourceGroupName $resourceGroup `
    -CollationName $collationName `
    -ServerName $sqlServerName

New-AzureRmSqlDatabase `
    -DatabaseName "SuperSportDataEngine_PublicSportData" `
    -ResourceGroupName $resourceGroup `
    -CollationName $collationName `
    -ServerName $sqlServerName

New-AzureRmSqlDatabase `
    -DatabaseName "SuperSportDataEngine_SystemSportData" `
    -ResourceGroupName $resourceGroup `
    -CollationName $collationName `
    -ServerName $sqlServerName