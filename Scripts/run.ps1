#Run

$Password = $Env:bot_nastya_remote_password
$User = $Env:bot_nastya_remote_user
$Port = $Env:bot_nastya_remote_port
$ComputerName = $Env:bot_nastya_remote_server
#E:\RGoolo\Nastya\Scripts\copy_to_server.ps1

$RemotePC = "$User@$ComputerName" + ":"
$RemotePC
ssh -p $Port $User@$ComputerName "dotnet /home/user/NightBot/Nastya.dll"