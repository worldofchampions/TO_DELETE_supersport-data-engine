# Login to Azure.
Login-AzureRmAccount

# Use the SuperSport subscription.
Set-AzureRmContext -SubscriptionId "1b355cd4-4b64-4890-a31b-840bafbb63b1"

$resourceGroup = "SuperSport-SSDE-Prod"
$redisName = "awe-ssde-p-redis"
$location = "West Europe"
$redisSize = "C1"
$redisSku = "Standard"

New-AzureRmRedisCache `
    -ResourceGroupName $resourceGroup `
    -Name $redisName `
    -Location $location `
    -Size $redisSize `
    -Sku $redisSku