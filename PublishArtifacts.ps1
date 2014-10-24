param (
[string] $buildNo = $null
)

function GetContentTypeFromExtension([string]$extension)
{   
	switch ($extension)
	{
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
	$dest = $input.Name
	if ($sbase -ne $null) {
		$dest = $input.FullName -replace [regex]::Escape($sbase)
	}
	
	return join-path $dbase $dest
}

function PutFile($sbase, $dbase) {
	foreach ($file in $input) {
		$dest = $file | GetFileDest -sbase $sbase -dbase $dbase
		$blobProperties = @{"ContentType" = GetContentTypeFromExtension($file.extension)}
		Set-AzureStorageBlobContent -Blob $dest -Container $ContainerName -File $file.FullName -Context $context -Force -Properties $blobProperties
	}
}

function PutDir($dbase) {
	foreach ($d in $input) {
		$dir = $d.FullName
		Get-ChildItem $dir -force -recurse | Where-Object {$_.mode -match "-a---"} | PutFileFilter -sbase $dir -dbase $dbase
	}
}

#Get-Item ".\to-publish" | PutDir -dbase $buildNo

[xml] $xml = Get-Content "./PublishArtifacts.config.xml"

Select-AzureSubscription -SubscriptionName $xml.settings.subscription
$context = New-AzureStorageContext -StorageAccountName $xml.settings.name -StorageAccountKey $xml.settings.key

# Artifacts container
$ContainerName = $xml.settings.artifacts

# Put package
Get-Item ".\Chalkable.Azure\bin\Release\app.publish\Chalkable.Azure.cspkg" | PutFile -dbase $buildNo

# Put service configurations
Get-Item ".\Chalkable.Azure\ServiceConfiguration.*.cscfg" | Where-Object {!($_.Name -like "*Local*")} | PutFile -dbase $buildNo


# Statics container
$ContainerName = $xml.settings.statics + $buildNo

$ContainerName | New-AzureStorageContainer -Permission Container -Context $context

Get-Item "Chalkable.Web\app\*App.compiled.js" | PutFile -dbase "app"
Get-Item "Chalkable.Web\app\chlk\shared.js" | PutFile -dbase "app\chlk"
Get-Item "Chalkable.Web\app\chlk\chlk-messages.js" | PutFile -dbase "app\chlk"
Get-Item "Chalkable.Web\app\chlk\chlk-constants.js" | PutFile -dbase "app\chlk"
Get-Item "Chalkable.Web\app\api\chlk-post-message-api.js" | PutFile -dbase "app\api"

Get-Item "Chalkable.Web\app\chlk\index" | PutDir -dbase "app\chlk\index"
Get-Item "Chalkable.Web\app\jquery" | PutDir -dbase "app\jquery"
Get-Item "Chalkable.Web\app\highcharts" | PutDir -dbase "app\highcharts"

Get-Item "Chalkable.Web\Content" | PutDir -dbase "Content"
