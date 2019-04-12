$root = (Get-Item $PSScriptRoot).Parent.FullName

$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
if ($msbuild) {
  $msbuild = join-path $msbuild 'MSBuild\15.0\Bin\MSBuild.exe'
}


$tlbexp = "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.2 Tools\TlbExp.exe"

function Build-Installer {
  & $msbuild "$root\Gldd.AmazonWaterLog.sln" /p:Configuration=Release /p:AllowedReferenceRelatedFileExtensions=.pdb
  
  $glddAmazonWaterLogOutputDir = "$root\Gldd.AmazonWaterLog\bin\Release"


  $AmazonBubblerInteropOutputDir = "$root\Gldd.AmazonWaterLogInterop\bin\Release"
  $AmazonBubblerInteropDll = "$AmazonBubblerInteropOutputDir\AmazonBubblerInterop.dll"
  $AmazonBubblerInteropTlb = "$AmazonBubblerInteropOutputDir\AmazonBubblerInterop.tlb"
  & $tlbexp $AmazonBubblerInteropDll /out:$AmazonBubblerInteropTlb
  heat file $AmazonBubblerInteropTlb -cg "AmazonBubblerInteropTLB" -ag -sfrag -srd -dr INSTALLFOLDER -out "AmazonBubblerInterop.tlb.wxs" -var var.AmazonBubblerInteropSourceDir
  heat file $AmazonBubblerInteropDll -cg "AmazonBubblerInteropDLL" -ag -sfrag -srd -dr INSTALLFOLDER -out "AmazonBubblerInterop.dll.wxs" -var var.AmazonBubblerInteropSourceDir



  heat dir "$glddAmazonWaterLogOutputDir" -cg ProductComponentsFragment -ag -sreg -sfrag -srd -dr INSTALLFOLDER -out "ProductComponentsFragment.wxs" -var var.GlddAmazonWaterLogSourceDir

  $wixOutputDir = "bin\Release"
  candle -out "$wixOutputDir\" Product.wxs ProductComponentsFragment.wxs GlddUI_Minimal.wxs AmazonBubblerInterop.tlb.wxs AmazonBubblerInterop.dll.wxs -ext WixUIExtension -dGlddAmazonWaterLogSourceDir="$glddAmazonWaterLogOutputDir" -dAmazonBubblerInteropSourceDir="$AmazonBubblerInteropOutputDir"
  light -out "$wixOutputDir\AmazonBubbler.msi" "$wixOutputDir\Product.wixobj" "$wixOutputDir\ProductComponentsFragment.wixobj" "$wixOutputDir\GlddUI_Minimal.wixobj" $wixOutputDir\AmazonBubblerInterop.tlb.wixobj $wixOutputDir\AmazonBubblerInterop.dll.wixobj -ext WixUIExtension
}