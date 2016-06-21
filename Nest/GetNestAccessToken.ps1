# Access Token generator for Nest API
# Usage : GetNestAccessToken.ps1 clientId clientSecret
# Version : 1.0 - 11/28/2015#  
# Copyright 2014-2015 Sebastien Warin <http://sebastien.warin.fr>
# All Rights Reserved.  

param(	
    [Parameter(Mandatory=$True,Position=1)]
	[string]$clientId,
    [Parameter(Mandatory=$True,Position=2)]
	[string]$clientSecret)

$rnd = Get-Random
$requestUri = "https://home.nest.com/login/oauth2?client_id=" + $clientId + "&state=ConstellationToken" + $rnd
# Open webbrowser to approve Nest product
Start-Process $requestUri
# Ask PIN code
$code = Read-Host 'What is the PIN code?'
# Get the access token
$httpResponse = Invoke-WebRequest -Uri https://api.home.nest.com/oauth2/access_token -Method POST -Body @{code=$code;client_id=$clientId;client_secret=$clientSecret;grant_type='authorization_code'}
$jsonResponse = ConvertFrom-Json $httpResponse.Content
# Print and copy access token to clipboard
$jsonResponse.access_token | clip.exe
Write-Host $jsonResponse.access_token
Write-Host "Access Token copied to the Windows clipboard !"