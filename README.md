# vaccine-appointment-app

[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fchameleonhead%2Fvaccine-appointment-app%2Fmain%2Fazuredeploy.json)


```powershell
$rg = 'vaapp'
New-AzResourceGroup -Name $rg -Location japaneast -Force
New-AzResourceGroupDeployment -Name 'vaapp-deploy' -ResourceGroupName $rg -TemplateFile 'azuredeploy.json' -resourcePrefix 'vaapp' -webSiteName 'vaccine-appointment-app' -dbAdminName 'vaapp' -dbAdminPassword 'P@ssw0rd'
```