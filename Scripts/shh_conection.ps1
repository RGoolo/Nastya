$Password = $Env:bot_nastya_remote_password
$User = $Env:bot_nastya_remote_user
$Port = $Env:bot_nastya_remote_port
$ComputerName = $Env:bot_nastya_remote_server
$Password 
$User
$Port
$ComputerName 
echo "ssh -o StrictHostKeyChecking=no -p $Port $User@$ComputerName"
ssh -o StrictHostKeyChecking=no -p $Port $User@$ComputerName