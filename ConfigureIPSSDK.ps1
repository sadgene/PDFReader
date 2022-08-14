# Этот скрипт конфигурирует IPS SDK


# переменные окружения, используемые в IPS SDK
# TODO: укажите пути к каталогам сервера приложений и клиента IPS
$env:IPS_VERSION = 6
$env:IPS_SERVER_DIR = Resolve-Path "\\ta03\c`$\Program Files\IPS\Server"
$env:IPS_CLIENT_DIR = Resolve-Path "C:\Program Files\IPS03\Client"
