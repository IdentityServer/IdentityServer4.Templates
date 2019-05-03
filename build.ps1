$CakeVersion = "0.30.0"
$DotNetVersion = "2.1.504";
$DotNetInstallerUri = "https://dot.net/v1/dotnet-install.ps1";

$TOOLS_DIR = Join-Path $PSScriptRoot "tools"
$NUGET_EXE = Join-Path $TOOLS_DIR "nuget.exe"
$NUGET_URL = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

# Make sure tools folder exists
$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
$ToolPath = Join-Path $PSScriptRoot "tools"
if (!(Test-Path $ToolPath)) {
    Write-Verbose "Creating tools directory..."
    New-Item -Path $ToolPath -Type directory | out-null
}

# Attempt to set highest encryption available for SecurityProtocol.
# PowerShell will not set this by default (until maybe .NET 4.6.x). This
# will typically produce a message for PowerShell v2 (just an info
# message though)
try {
    # Set TLS 1.2 (3072), then TLS 1.1 (768), then TLS 1.0 (192), finally SSL 3.0 (48)
    # Use integers because the enumeration values for TLS 1.2 and TLS 1.1 won't
    # exist in .NET 4.0, even though they are addressable if .NET 4.5+ is
    # installed (.NET 4.5 is an in-place upgrade).
    [System.Net.ServicePointManager]::SecurityProtocol = 3072 -bor 768 -bor 192 -bor 48
  } catch {
    Write-Output 'Unable to set PowerShell to use TLS 1.2 and TLS 1.1 due to old .NET Framework installed. If you see underlying connection closed or trust errors, you may need to upgrade to .NET Framework 4.5+ and PowerShell v3'
  }

###########################################################################
# INSTALL NUGET
###########################################################################

# Try find NuGet.exe in path if not exists
if (!(Test-Path $NUGET_EXE)) {
    Write-Verbose -Message "Trying to find nuget.exe in PATH..."
    $existingPaths = $Env:Path -Split ';' | Where-Object { (![string]::IsNullOrEmpty($_)) -and (Test-Path $_ -PathType Container) }
    $NUGET_EXE_IN_PATH = Get-ChildItem -Path $existingPaths -Filter "nuget.exe" | Select -First 1
    if ($NUGET_EXE_IN_PATH -ne $null -and (Test-Path $NUGET_EXE_IN_PATH.FullName)) {
        Write-Verbose -Message "Found in PATH at $($NUGET_EXE_IN_PATH.FullName)."
        $NUGET_EXE = $NUGET_EXE_IN_PATH.FullName
    }
}

# Try download NuGet.exe if not exists
if (!(Test-Path $NUGET_EXE)) {
    Write-Verbose -Message "Downloading NuGet.exe..."
    try {
        (New-Object System.Net.WebClient).DownloadFile($NUGET_URL, $NUGET_EXE)
    } catch {
        Throw "Could not download NuGet.exe."
    }
}

# Save nuget.exe path to environment to be available to child processed
$ENV:NUGET_EXE = $NUGET_EXE

###########################################################################
# INSTALL CAKE
###########################################################################

# Make sure Cake has been installed.
$CakePath = Join-Path $ToolPath ".store\cake.tool\$CakeVersion"
$CakeExePath = (Get-ChildItem -Path $ToolPath -Filter "dotnet-cake*" -File| ForEach-Object FullName | Select-Object -First 1)

if ((!(Test-Path -Path $CakePath -PathType Container)) -or (!(Test-Path $CakeExePath -PathType Leaf))) {
    & dotnet tool install --tool-path $ToolPath --version $CakeVersion Cake.Tool
    if ($LASTEXITCODE -ne 0)
    {
        'Failed to install cake'
        exit 1
    }
    $CakeExePath = (Get-ChildItem -Path $ToolPath -Filter "dotnet-cake*" -File| ForEach-Object FullName | Select-Object -First 1)
}

###########################################################################
# RUN BUILD SCRIPT
###########################################################################
Write-Host "build template code..."

& "$CakeExePath" ./build.cake --bootstrap
if ($LASTEXITCODE -eq 0)
{
    & "$CakeExePath" ./build.cake -Target="Build"
}

Write-Host "clean..."
Invoke-Expression "git clean -xdf ./src"
Invoke-Expression "git clean -xdf ./feed"
Invoke-Expression "git clean -xdf ./UI"

Write-Host "Downloading quickstart UI..."
cd .\UI
iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/master/getmaster.ps1'))
cd ..

& "$CakeExePath" ./build.cake --bootstrap
if ($LASTEXITCODE -eq 0)
{
    & "$CakeExePath" ./build.cake $args
}
exit $LASTEXITCODE