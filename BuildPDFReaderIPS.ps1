# ���� ������ ����������� ����������� solution, ��������� ������������ IPS SDK �� 
# ����� ConfigureIPSSDK.ps1 � ����� � ������ solution.

# ��������!!! �� �������������!!!
# ���������� ����� ����� ��������� ������������� � ����� ���� �������� � ����� ������.


$sdkDir = $env:IPS_SDK_DIR
if (!$sdkDir -or !(Test-Path $sdkDir)) { throw "IPS SDK is not found. Please install the IPS SDK and configure it at first." }
$sdkCommandsDir = Join-Path $sdkDir "Automation\v1\Commands"


# ���������� ���������� ����� ������� (dot sourcing)
. (Join-Path $sdkCommandsDir "Lib.ps1")


# ��������� ��� �������� - ��� ������������� ����� ����� ������� (��� ��������� Build � � ����������� .sln)
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$solutionFileName = (Set-Extension (Split-Path -Leaf $MyInvocation.MyCommand.Definition) ".sln") -replace @("^Build", "")
$solutionPath = Join-Path $scriptDir $solutionFileName


# ���������� ��������
if (!(Test-Path $solutionPath)) { throw "The solution $solutionPath is not found." }
& (Join-Path $sdkCommandsDir "BuildSolution.ps1") $solutionPath @args
