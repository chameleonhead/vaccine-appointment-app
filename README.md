# ワクチン接種予約Webアプリ

ワクチン接種の予約をするためのアプリケーションです。

## 機能

- 予約枠の一括作成 (日にち、開始時刻、枠の時間、枠当たりの人数、作成個数)
- 確認メール送信 (接続先SMTPの設定、メール本文のカスタマイズ)

## デプロイ方法

Azureのサブスクリプションがあれば、以下のボタンでデプロイできます。

[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fchameleonhead%2Fvaccine-appointment-app%2Fmain%2Fazuredeploy.json)


```powershell
$rg = 'vaapp'
New-AzResourceGroup -Name $rg -Location japaneast -Force
New-AzResourceGroupDeployment -Name 'vaapp-deploy' -ResourceGroupName $rg -TemplateFile 'azuredeploy.json' -resourcePrefix 'vaapp' -webSiteName 'vaccine-appointment-app' -dbAdminName 'vaapp' -dbAdminPassword 'P@ssw0rd'
```

## フィードバック

issue、PRを歓迎します。
