# Login to Azure.
Login-AzureRmAccount

# Use the SuperSport subscription.
Set-AzureRmContext -SubscriptionId "1b355cd4-4b64-4890-a31b-840bafbb63b1"

$resourceGroup = "SuperSport-SSDE-QA"
$sqlServerName = "awe-ssde-q-sql-server"
$location = "West Europe"
$credentials = Get-Credential
$collationName = "SQL_Latin1_General_CP1_CI_AS"

New-AzureRmSqlServer `
    -ResourceGroupName $resourceGroup `
    -ServerName $sqlServerName `
    -SqlAdministratorCredentials $credentials `
    -Location $location

New-AzureRmSqlDatabase `
    -DatabaseName "SuperSportDataEngine_Hangfire" `
    -ResourceGroupName $resourceGroup `
    -CollationName $collationName `
    -ServerName $sqlServerName `
    -Edition $edition

New-AzureRmSqlDatabase `
    -DatabaseName "SuperSportDataEngine_PublicSportData" `
    -ResourceGroupName $resourceGroup `
    -CollationName $collationName `
    -ServerName $sqlServerName `
    -Edition $edition

New-AzureRmSqlDatabase `
    -DatabaseName "SuperSportDataEngine_SystemSportData" `
    -ResourceGroupName $resourceGroup `
    -CollationName $collationName `
    -ServerName $sqlServerName `
    -Edition $edition

    $storageAccountKey = ###INSERT KEY###
    $hangfireStorageUri = ###INSERT URI### 
    $publicSportDataStorageUri = ###INSERT URI### 
    $systemSportDataStorageUri = ###INSERT URI### 
    $adminUsername = ###ADMIN USERNAME###
    $adminPassword = ###ADMIN USERNAME###
    
    # Import request for SuperSportDataEngine_Hangfire
    $importRequest = New-AzureRmSqlDatabaseImport -ResourceGroupName "SuperSport-SSDE-Prod" `
        -ServerName $sqlServerName `
        -DatabaseName "SuperSportDataEngine_Hangfire" `
        -DatabaseMaxSizeBytes "262144000" `
        -StorageKeyType "StorageAccessKey" `
        -StorageKey $storageAccountKey `
        -StorageUri $hangfireStorageUri `
        -Edition "Standard" `
        -ServiceObjectiveName "P6" `
        -AdministratorLogin $adminUsername `
        -AdministratorLoginPassword $(ConvertTo-SecureString -String $adminPassword -AsPlainText -Force)
    
    # Track the status of importing SuperSportDataEngine_Hangfire
    $importStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
    [Console]::Write("Importing SuperSportDataEngine_Hangfire")
    while ($importStatus.Status -eq "InProgress")
    {
        $importStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
        [Console]::Write(".")
        Start-Sleep -s 10
    }
    [Console]::WriteLine("")
    $importStatus
    
    $importRequest = New-AzureRmSqlDatabaseImport -ResourceGroupName "SuperSport-SSDE-Prod" `
        -ServerName $sqlServerName `
        -DatabaseName "SuperSportDataEngine_PublicSportData" `
        -DatabaseMaxSizeBytes "262144000" `
        -StorageKeyType "StorageAccessKey" `
        -StorageKey $storageAccountKey `
        -StorageUri $publicSportDataStorageUri `
        -Edition "Standard" `
        -ServiceObjectiveName "P6" `
        -AdministratorLogin $adminUsername `
        -AdministratorLoginPassword $(ConvertTo-SecureString -String $adminPassword -AsPlainText -Force)
    
    $importStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
    [Console]::Write("Importing SuperSportDataEngine_PublicSportData")
    while ($importStatus.Status -eq "InProgress")
    {
        $importStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
        [Console]::Write(".")
        Start-Sleep -s 10
    }
    [Console]::WriteLine("")
    $importStatus
    
    # Import request for SuperSportDataEngine_SystemSportData
    $importRequest = New-AzureRmSqlDatabaseImport -ResourceGroupName "SuperSport-SSDE-Prod" `
        -ServerName $sqlServerName `
        -DatabaseName "SuperSportDataEngine_SystemSportData" `
        -DatabaseMaxSizeBytes "262144000" `
        -StorageKeyType "StorageAccessKey" `
        -StorageKey $storageAccountKey `
        -StorageUri $systemSportDataStorageUri `
        -Edition "Standard" `
        -ServiceObjectiveName "P6" `
        -AdministratorLogin $adminUsername `
        -AdministratorLoginPassword $(ConvertTo-SecureString -String $adminPassword -AsPlainText -Force)
    
    # Track status for importing SuperSportDataEngine_SystemSportData
    $importStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
    [Console]::Write("Importing SuperSportDataEngine_SystemSportData")
    while ($importStatus.Status -eq "InProgress")
    {
        $importStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
        [Console]::Write(".")
        Start-Sleep -s 10
    }
    [Console]::WriteLine("")
    $importStatus