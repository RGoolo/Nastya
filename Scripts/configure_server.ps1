$Password = $Env:bot_nastya_remote_password
#$Password = Read-Host

$User = $Env:bot_nastya_remote_user
$Port = $Env:bot_nastya_remote_port
$ComputerName = $Env:bot_nastya_remote_server

$Password 
$User
$Port
$ComputerName 

ssh  -p $Port $User@$ComputerName "sudo sh -c 'echo ""deb [arch=amd64] https://apt-mo.trafficmanager.net/repos/dotnet-release/ xenial main"" > /etc/apt/sources.list.d/dotnetdev.list'"
ssh  -p $Port $User@$ComputerName "sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 417A0893"
ssh  -p $Port $User@$ComputerName "sudo apt-get update"
ssh  -p $Port $User@$ComputerName "sudo apt-get install dotnet-dev-1.0.4"


ssh  -p $Port $User@$ComputerName "wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb"
ssh  -p $Port $User@$ComputerName "sudo dpkg -i packages-microsoft-prod.deb"


ssh  -p $Port $User@$ComputerName "sudo add-apt-repository universe"
ssh  -p $Port $User@$ComputerName "sudo apt-get install apt-transport-https"


ssh  -p $Port $User@$ComputerName "sudo apt-get update"
ssh  -p $Port $User@$ComputerName "sudo apt-get install dotnet-sdk-5.0"