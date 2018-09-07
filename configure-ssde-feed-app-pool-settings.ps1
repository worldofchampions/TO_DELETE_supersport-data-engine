Import-Module WebAdministration

#_________________GLOBAL_VARIABLES___________________________________________________________

$iisAppPoolName = "ssde-legacy-feed-app-pool"
#____________________________________________________________________________________________


#_________________CREATE_APPPOOL_____________________________________________________________
$iisAppPoolDotNetVersion = "v4.0"
$iisAppPoolManagedPipelineMode = "Integrated"

#Navigate to the app pools root
cd IIS:\AppPools\

#Check if our AppPool exists and create if not
if (!(Test-Path $iisAppPoolName -pathType container))
{
    #create the app pool
    $appPool = New-Item $iisAppPoolName 
    $appPool | Set-ItemProperty -Name "managedRuntimeVersion" -Value $iisAppPoolDotNetVersion
    $appPool | Set-ItemProperty -Name "managedPipelineMode" -Value $iisAppPoolManagedPipelineMode

    #Assign created app pool to default site
    Set-ItemProperty 'IIS:\Sites\Default Web Site' applicationPool $iisAppPoolName
}
#________________________________________________________________________________________________

#_________________ADD_APPPOOL_TO_PERFOMANCE_MONITOR_USER GROUP___________________________________

$PerformanceMonitorUsersGroupName = "Performance Monitor Users"
$PerformanceMonitorUsers = Get-LocalGroupMember -Group $PerformanceMonitorUsersGroupName
$isAppPoolAddedToPerfomanceMonitorUsers = $FALSE
ForEach($member in $PerformanceMonitorUsers)
{
    if($member.Name -eq "IIS AppPool\$iisAppPoolName")
    {
        $isAppPoolAddedToPerfomanceMonitorUsers = $TRUE
    }
}

if(!$isAppPoolAddedToPerfomanceMonitorUsers)
{
    $group = [ADSI]"WinNT://$Env:ComputerName/Performance Monitor Users,group"
    $ntAccount = New-Object System.Security.Principal.NTAccount("IIS AppPool\$iisAppPoolName")
    $ntAccountSID = $ntAccount.Translate([System.Security.Principal.SecurityIdentifier])
    $user = [ADSI]"WinNT://$ntAccountSID"
    $group.Add($user.Path)
}
#________________________________________________________________________________________________


