param (
[string] $buildNo = $null,
[string] $buildBranch = $null
)

function GetContentTypeFromExtension([string]$extension)
{   
	switch ($extension)
	{
		".mp3"  { return "audio/mp3" }
		".ogg"  { return "audio/ogg" }
		".wav"  { return "audio/wave" }
		".png"  { return "image/png" }
		".htm"  { return "text/html" }
		".pfx"  { return "application/x-pkcs12" }
		".xml"  { return "text/xml" }
		".css"  { return "text/css" }
		".jpg"  { return "image/jpeg" }
		".jpeg" { return "image/jpeg" }
		".bmp"  { return "image/bmp" }
		".js"   { return "text/x-javascript" }
		".zip"  { return "application/zip" }
	}

	return "application/octet-stream"
}

function GetFileDest($sbase, $dbase) {
	foreach ($file in $input) {
		$dest = $file.Name
		if ($sbase -ne $null) {
			$dest = $file.FullName -replace [regex]::Escape($sbase)
		}
		
		return join-path $dbase $dest
	}
}

function PutFile($sbase, $dbase) {
	foreach ($file in $input) {
		$dest = $file | GetFileDest -sbase $sbase -dbase $dbase
		$blobProperties = @{"ContentType" = GetContentTypeFromExtension($file.extension)}
		$blob = Set-AzureStorageBlobContent -Blob $dest -Container $ContainerName -File $file.FullName -Context $context -Force -Properties $blobProperties		
		$blob.Name
	}
}

function PutFileWithUrl($sbase, $dbase) {
	foreach ($file in $input) {
		$dest = $file | GetFileDest -sbase $sbase -dbase $dbase
		$blobProperties = @{"ContentType" = GetContentTypeFromExtension($file.extension)}
		Set-AzureStorageBlobContent -Blob $dest -Container $ContainerName -File $file.FullName -Context $context -Force -Properties $blobProperties		
		$blobState = Get-AzureStorageBlob -blob $dest -Container $containerName -Context $context
    $blobState.ICloudBlob.uri.AbsoluteUri
	}
}

function PutDir($dbase) {
	foreach ($d in $input) {
		$dir = $d.FullName
		Get-ChildItem $dir -force -recurse | Where-Object {$_.mode -match "-a---"} | PutFile -sbase $dir -dbase $dbase
	}
}

function PutContentDir($dbase, $exclude) {
  foreach ($d in $input) {
		$dir = $d.FullName
		Get-ChildItem $dir -force -recurse | Where-Object {$_.mode -match "-a---"} | Where-Object { $_.FullName -notmatch $exclude } | PutFile -sbase $dir -dbase $dbase
	}
}

Function Set-AzureSettings($publishsettings, $subscription, $storageaccount){
    Import-AzurePublishSettingsFile -PublishSettingsFile $publishsettings
 
    Set-AzureSubscription -SubscriptionName $subscription -CurrentStorageAccount $storageaccount
 
    Select-AzureSubscription -SubscriptionName $subscription
}

Function Upgrade-Deployment($package_url, $service, $slot, $config){
    $setdeployment = Set-AzureDeployment -Upgrade -Slot $slot -Package $package_url -Configuration $config -ServiceName $service -Force
}
 
Function Check-Deployment($service, $slot){
    $completeDeployment = Get-AzureDeployment -ServiceName $service -Slot $slot
    $completeDeployment.deploymentid
}

try{
  Write-Host "Initializing publish artifacts; build $buildNo, branch $buildBranch"

  Import-Module "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Azure.psd1"

  $publishsettings = ".\ChalkableAzure.publishsettings"
  Write-Host "Importing publish profile and setting subscription"
  [xml] $xml = Get-Content "./PublishArtifacts.config.xml"
  Set-AzureSettings -publishsettings $publishsettings -subscription $xml.settings.subscription -storageaccount $xml.settings.name
 

  Write-Host "Starting publish artifacts"  
  $context = New-AzureStorageContext -StorageAccountName $xml.settings.name -StorageAccountKey $xml.settings.key

  # Artifacts container
  $ContainerName = $xml.settings.artifacts

  # Put package
  $cspkg_urls = Get-Item ".\Chalkable.Azure\bin\Release\app.publish\Chalkable.Azure.cspkg" | PutFileWithUrl -dbase $buildNo

  # Put service configurations
  Get-Item ".\Chalkable.Azure\ServiceConfiguration.*.cscfg" | Where-Object {!($_.Name -like "*Local*")} | PutFile -dbase $buildNo

  if ($buildNo -ne $null) {
    # Statics container
    $ContainerName = $xml.settings.statics + $buildNo
    $ContainerName | New-AzureStorageContainer -Permission Container -Context $context

    Get-Item "Chalkable.Web\app\*App.compiled.js" | PutFile -dbase "app"
    Get-Item "Chalkable.Web\app\chlk\shared.js" | PutFile -dbase "app\chlk"
    Get-Item "Chalkable.Web\app\chlk\chlk-messages.js" | PutFile -dbase "app\chlk"
    Get-Item "Chalkable.Web\app\chlk\chlk-constants.js" | PutFile -dbase "app\chlk"

    Get-Item "Chalkable.Web\app\chlk\index" | PutDir -dbase "app\chlk\index"
    Get-Item "Chalkable.Web\app\jquery" | PutDir -dbase "app\jquery"
    Get-Item "Chalkable.Web\app\lib" | PutDir -dbase "app\lib"
    Get-Item "Chalkable.Web\app\highcharts" | PutDir -dbase "app\highcharts"
    
    Get-Item "Chalkable.Web\scripts" | PutDir -dbase "scripts"

    Get-Item "Chalkable.Web\Content" | PutContentDir -dbase "Content" -exclude ".*\\(icons-24|icons-32|alerts-icons)\\.*"
  }
  
  # CI
  $service = $null
  $cspkg_url = $cspkg_urls | Where-Object { $_ -match ".*Azure\.cspkg" }    
  $cscfg_url = $null
  if ($buildBranch -eq 'staging') {
    $service = "chalkablestaging"
    $cscfg_url = Get-Item ".\Chalkable.Azure\ServiceConfiguration.Staging.cscfg"
  } elseif ($buildBranch -eq 'qa') {
    $service = "chalkableqa"
    $cscfg_url = Get-Item ".\Chalkable.Azure\ServiceConfiguration.QA.cscfg"
  }
  
  if ($service -ne $null) {
    Write-Host "Starting deployment"
    $slot = "Production"
    $deployment = Get-AzureDeployment -ServiceName $service -Slot $slot

    Write-Host "Upgrading deployment $service in slot $slot."
    Write-Host "Package: $cspkg_url"
    Write-Host "Config: $cscfg_url"
    Upgrade-Deployment -package_url $cspkg_url -service $service -slot $slot -config $cscfg_url
    Write-Host "Upgraded Deployment"

    $deploymentid = Check-Deployment -service $service -slot $slot -Context $context
    Write-Host "Deployed to $service with deployment id $deploymentid"
  }
  exit 0
}
catch [System.Exception] {
    Write-Host $_.Exception.ToString()
    exit 1
}