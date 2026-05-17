# generate-cert.ps1
# Windows PowerShell で実行してください
# 実行方法: .\generate-cert.ps1

$certsDir = "$PSScriptRoot\nginx\certs"

if (-not (Test-Path $certsDir)) {
    New-Item -ItemType Directory -Path $certsDir | Out-Null
}

Write-Host "自己署名証明書を生成します..." -ForegroundColor Cyan

# openssl が使えるか確認（Git for Windows / WSL に含まれる）
$openssl = $null
if (Get-Command openssl -ErrorAction SilentlyContinue) {
    $openssl = "openssl"
} elseif (Test-Path "C:\Program Files\Git\usr\bin\openssl.exe") {
    $openssl = "C:\Program Files\Git\usr\bin\openssl.exe"
} else {
    Write-Host "ERROR: openssl が見つかりません。" -ForegroundColor Red
    Write-Host "Git for Windows をインストールするか、WSL から実行してください。" -ForegroundColor Yellow
    Write-Host "WSL の場合: bash generate-cert.sh" -ForegroundColor Yellow
    exit 1
}

& $openssl req -x509 -nodes -days 365 -newkey rsa:2048 `
    -keyout "$certsDir\key.pem" `
    -out "$certsDir\cert.pem" `
    -subj "/C=JP/ST=Tokyo/L=Tokyo/O=Dev/CN=localhost" `
    -addext "subjectAltName=DNS:localhost,IP:127.0.0.1"

if ($LASTEXITCODE -eq 0) {
    Write-Host "証明書を生成しました:" -ForegroundColor Green
    Write-Host "  $certsDir\cert.pem"
    Write-Host "  $certsDir\key.pem"
    Write-Host ""
    Write-Host "次のステップ:" -ForegroundColor Cyan
    Write-Host "  docker compose -f docker-compose.dev.yml up --build"
    Write-Host ""
    Write-Host "ブラウザで https://localhost にアクセスしてください。" -ForegroundColor Cyan
    Write-Host "（自己署名のため警告が出ます。詳細設定から続行してください）" -ForegroundColor Yellow
} else {
    Write-Host "証明書の生成に失敗しました" -ForegroundColor Red
}
