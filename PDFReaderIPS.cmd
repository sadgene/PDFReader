:: ���� ������ ��������� ����������� solution � Visual Studio, ��������� ������������ IPS SDK ��
:: ����� ConfigureIPSSDK.ps1 � ����� � ������ solution.

:: ��������!!! �� �������������!!!
:: ���������� ����� ����� ��������� ������������� � ����� ���� �������� � ����� ������.

@echo off

:: �������� ���������� ��������� powershell-�������
set __SCRIPTDIR=%~dp0
set __SCRIPTPATH=%~dpn0.ps1
powershell -Version 3 -ExecutionPolicy RemoteSigned -Command "& '%__SCRIPTPATH%'" %*
if errorlevel 1 pause
