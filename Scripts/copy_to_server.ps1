#E:\RGoolo\Nastya\Scripts\copy_to_server.ps1

$Password = $Env:bot_nastya_remote_password
#$Password = Read-Host

$User = $Env:bot_nastya_remote_user
$Port = $Env:bot_nastya_remote_port
$ComputerName = $Env:bot_nastya_remote_server

$Password 
$User
$Port
$ComputerName 
$RemotePC = "$User@$ComputerName" + ":"
$RemotePC

#crete dir if no exist
echo "Create dir $RemotePC/home/user/NightBot ..."
ssh  -p $Port $User@$ComputerName "mkdir -p /home/user/NightBot"

#Copy to
echo "load files..."
scp –rp E:\Nastya\bin\publish\* –P $Port $RemotePC/home/user/NightBot