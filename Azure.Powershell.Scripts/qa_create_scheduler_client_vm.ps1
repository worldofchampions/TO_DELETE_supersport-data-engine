# Login to Azure.
Login-AzureRmAccount

# Use the SuperSport subscription.
Set-AzureRmContext -SubscriptionId "1b355cd4-4b64-4890-a31b-840bafbb63b1"

$deploymentResourceGroup = "SuperSport-SSDE-QA"
$vmName = "awe-ssde-q-sc"
$vmSize = "Standard_F4s_v2"

# Create a new Network Security Group which 
# allows port 9622 to be opened and accessed.

$httprule = New-AzureRmNetworkSecurityRuleConfig `
    -Name "allow_hangfire_dashboard_access" `
    -Description "Allow HTTP" `
    -Access "Allow" `
    -Protocol "Tcp" `
    -Direction "Inbound" `
    -Priority "100" `
    -SourceAddressPrefix "*" `
    -SourcePortRange * `
    -DestinationAddressPrefix * `
    -DestinationPortRange 9622

$nsg = New-AzureRmNetworkSecurityGroup `
    -ResourceGroupName $deploymentResourceGroup `
    -Location "West Europe" `
    -Name $vmName `
    -SecurityRules $httprule

# Create the VM in the existing Virtual Network.
New-AzureRmResourceGroupDeployment `
    -Name "deployment" `
    -ResourceGroupName $deploymentResourceGroup `
    -TemplateUri 201-vm-different-rg-vnet/azuredeploy.json `
    -vmName $vmName `
    -newStorageAccountName "awessdeqstorage" `
    -adminUsername "SuperSportDataEngine" `
    -virtualNetworkName "SuperSport-API-Vnet" `
    -virtualNetworkResourceGroup "SuperSport-API-Backend-Prod" `
    -subnet1Name "default" `
    -nicName $vmName `
    -vmSize $vmSize `
    -publicIPName $vmName