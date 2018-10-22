# Login to Azure.
Login-AzureRmAccount

# Use the SuperSport subscription.
Set-AzureRmContext -SubscriptionId "1b355cd4-4b64-4890-a31b-840bafbb63b1"

# Variables used for Creation of App Service.
$stagingResourceGroupName = "SuperSport-SSDE-Staging"
$appServicePlanName = "awe-ssde-s-appserviceplan"
$location = "West Europe"
$appServiceTier = "Standard"
$appServiceSize = "Small"

####
# This script will create an App Service with 2 worker with 2 Legacy Feeds on each one (staging and QA).
# Cost will be 2x the cost of the App Service.
####

# Create a new Azure App Service for Staging/QA.
New-AzureRmAppServicePlan `
    -ResourceGroupName $stagingResourceGroupName `
    -Location $location `
    -Tier $appServiceTier `
    -WorkerSize $appServiceSize `
    -Name $appServicePlanName `
    -NumberOfWorkers 2

# Create Staging Legacy Feed instance
New-AzureRmWebApp `
    -ResourceGroupName $stagingResourceGroupName `
    -Name 'awe-ssde-s-legacy-feed' `
    -Location $location `
    -AppServicePlan $appServicePlanName

# Create QA Legacy Feed instance
New-AzureRmWebApp `
    -ResourceGroupName $stagingResourceGroupName `
    -Name 'awe-ssde-q-legacy-feed' `
    -Location $location `
    -AppServicePlan $appServicePlanName