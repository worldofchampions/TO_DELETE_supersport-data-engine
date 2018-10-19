# Login to Azure.
Login-AzureRmAccount

# Use the SuperSport subscription.
Set-AzureRmContext -SubscriptionId "1b355cd4-4b64-4890-a31b-840bafbb63b1"

# Variables used for Creation of App Service.
$stagingResourceGroupName = "SuperSport-SSDE-Staging"
$appServicePlanName = "awe-ssde-s-appserviceplan"
$location = "West Europe"
$appServiceTier = "Standard"

####
# This script will create an App Service with 1 worker with 4 Legacy Feeds on it.
# Two staging feeds, and two QA feeds (is this even necessary?)
# Cost will be 1x the cost of the App Service.
####

# Create a new Azure App Service for Staging/QA.
New-AzureRmAppServicePlan `
    -ResourceGroupName $stagingResourceGroupName `
    -Location $location `
    -Tier $appServiceTier `
    -Name $appServicePlanName

# Create Staging Legacy Feed instance 1
New-AzureRmWebApp `
    -ResourceGroupName $stagingResourceGroupName `
    -Name 'awe-ssde-s-legacy-feed1' `
    -Location $location `
    -AppServicePlan $appServicePlanName

# Create Staging Legacy Feed instance 2
New-AzureRmWebApp `
    -ResourceGroupName $stagingResourceGroupName `
    -Name 'awe-ssde-s-legacy-feed2' `
    -Location $location `
    -AppServicePlan $appServicePlanName

# Create QA Legacy Feed instance 1
New-AzureRmWebApp `
    -ResourceGroupName $stagingResourceGroupName `
    -Name 'awe-ssde-q-legacy-feed1' `
    -Location $location `
    -AppServicePlan $appServicePlanName

# Create QA Legacy Feed instance 2
New-AzureRmWebApp `
    -ResourceGroupName $stagingResourceGroupName `
    -Name 'awe-ssde-q-legacy-feed2' `
    -Location $location `
    -AppServicePlan $appServicePlanName