# Login to Azure.
Login-AzureRmAccount

# Use the SuperSport subscription.
Set-AzureRmContext -SubscriptionId "1b355cd4-4b64-4890-a31b-840bafbb63b1"

$deploymentResourceGroup = "SuperSport-SSDE-QA"
$vmSize = "Standard_D2s_v3"
$vmName = "awe-ssde-q-sis1"
## UPDATE THIS IF THE BASE IMAGE CHANGES ##
$baseImageUri = "/subscriptions/1b355cd4-4b64-4890-a31b-840bafbb63b1/resourceGroups/SuperSport-SSDE-Common/providers/Microsoft.Compute/images/SSDE-BASE-IMAGE-20181025101839"
$storageAccountName = "awessdeqstorage"

# Create a new Network Security Group
$nsg = New-AzureRmNetworkSecurityGroup `
    -ResourceGroupName $deploymentResourceGroup `
    -Location "West Europe" `
    -Name $vmName

# Create the VM in the existing Virtual Network.
New-AzureRmResourceGroupDeployment `
    -Name "deployment" `
    -ResourceGroupName $deploymentResourceGroup `
    -TemplateUri 201-vm-different-rg-vnet/azuredeploy.json `
    -vmName $vmName `
    -newStorageAccountName $storageAccountName `
    -adminUsername "SuperSportDataEngine" `
    -virtualNetworkName "SuperSport-API-Vnet" `
    -virtualNetworkResourceGroup "SuperSport-API-Backend-Prod" `
    -subnet1Name "default" `
    -nicName $vmName `
    -vmSize $vmSize `
    -publicIPName $vmName `
    -baseImageUri $baseImageUri

$vmName = "awe-ssde-q-sis2"

# Create a new Network Security Group
$nsg = New-AzureRmNetworkSecurityGroup `
    -ResourceGroupName $deploymentResourceGroup `
    -Location "West Europe" `
    -Name $vmName

# Create the VM in the existing Virtual Network.
New-AzureRmResourceGroupDeployment `
    -Name "deployment" `
    -ResourceGroupName $deploymentResourceGroup `
    -TemplateUri 201-vm-different-rg-vnet/azuredeploy.json `
    -vmName $vmName `
    -newStorageAccountName $storageAccountName `
    -adminUsername "SuperSportDataEngine" `
    -virtualNetworkName "SuperSport-API-Vnet" `
    -virtualNetworkResourceGroup "SuperSport-API-Backend-Prod" `
    -subnet1Name "default" `
    -nicName $vmName `
    -vmSize $vmSize `
    -publicIPName $vmName `
    -baseImageUri $baseImageUri