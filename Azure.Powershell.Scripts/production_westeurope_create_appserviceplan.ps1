# Login to Azure.
Login-AzureRmAccount

# Use the SuperSport subscription.
Set-AzureRmContext -SubscriptionId "1b355cd4-4b64-4890-a31b-840bafbb63b1"

# Variables used for Creation of App Service.
$resourceGroupName = "SuperSport-SSDE-Prod"
$appServicePlanName = "awe-ssde-p-appserviceplan"
$location = "West Europe"
$appServiceTier = "Standard"

# Create a new Azure App Service for production.
New-AzureRmAppServicePlan `
    -ResourceGroupName $resourceGroupName `
    -Location $location `
    -Tier $appServiceTier `
    -Name $appServicePlanName `
    -NumberofWorkers 2

# Create Production Legacy Feed instance
New-AzureRmWebApp `
    -ResourceGroupName $resourceGroupName `
    -Name 'awe-ssde-p-legacy-feed' `
    -Location $location `
    -AppServicePlan $appServicePlanName