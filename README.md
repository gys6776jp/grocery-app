# 買い物アプリ

## 構成

```
grocery-app/
├── .github/workflows/deploy.yml  # GitHub Actions 自動デプロイ
├── apache/
│   └── grocery.yoshihamkorori.work.conf  # Apache バーチャルホスト設定
├── docker/
│   ├── api/Dockerfile
│   ├── frontend/Dockerfile
│   └── db/init.sql
├── nginx/
│   ├── nginx.conf       # 本番用（Apache からプロキシ受け）
│   └── nginx.dev.conf   # 開発用（自己署名SSL）
├── src/                 # バックエンド C# クリーンアーキテクチャ
│   ├── GroceryApp.Domain/
│   ├── GroceryApp.Application/
│   ├── GroceryApp.Infrastructure/
│   └── GroceryApp.API/
├── frontend/
│   └── GroceryApp/      # Blazor WebAssembly
├── docker-compose.yml      # 本番用
├── docker-compose.dev.yml  # ローカル開発用
├── .env.example
└── generate-cert.ps1    # 開発用自己署名証明書生成（Windows）
```

---

## ① ローカル開発（Windows Docker Desktop）

### 1. 証明書を生成
```powershell
.\generate-cert.ps1
```

### 2. 起動
```powershell
docker compose -f docker-compose.dev.yml up --build
```

### 3. アクセス
ブラウザで `https://localhost` を開く。
自己署名証明書の警告が出たら「詳細設定」→「続行」。

初回の登録コード: `dev_register_code_change_me`
（docker-compose.dev.yml の `Auth__RegisterCode` で変更可）

---

## ② VPS 本番デプロイ（初回セットアップ）

### 1. DNS設定
ドメイン管理画面で Aレコードを追加する。
```
grocery.yoshihamkorori.work  →  VPSのIPアドレス
```
反映まで数分〜数時間かかる場合がある。

### 2. VPS に必要なパッケージをインストール
```bash
sudo apt update && sudo apt install -y docker.io docker-compose-plugin certbot
sudo systemctl enable --now docker
sudo usermod -aG docker $USER  # 再ログインで反映
```

### 3. Apache モジュールを有効化
```bash
sudo a2enmod proxy proxy_http headers ssl
```

### 4. Apache バーチャルホストを追加（既存サービスに影響なし）
```bash
sudo cp /path/to/grocery-app/apache/grocery.yoshihamkorori.work.conf \
        /etc/apache2/sites-available/

sudo a2ensite grocery.yoshihamkorori.work.conf
sudo systemctl reload apache2
```

### 5. Let's Encrypt 証明書を取得
```bash
sudo certbot --apache -d grocery.yoshihamkorori.work \
  --non-interactive --agree-tos -m your-email@example.com
```
certbot が自動で Apache の HTTPS 設定を追記し、80→443リダイレクトも設定してくれる。

### 6. デプロイ先ディレクトリを作成
```bash
sudo mkdir -p /opt/grocery-app
sudo chown $USER:$USER /opt/grocery-app
```

### 7. .env を作成（サーバ上で直接作成・Git管理しない）
```bash
cp /opt/grocery-app/.env.example /opt/grocery-app/.env
nano /opt/grocery-app/.env
```

`.env` の中身（各値を必ず変更すること）:
```env
DB_ROOT_PASSWORD=【強いパスワード】
DB_USER=grocery_user
DB_PASSWORD=【強いパスワード】
JWT_KEY=【32文字以上のランダム文字列】
JWT_ISSUER=https://grocery.yoshihamkorori.work
REGISTER_CODE=【登録用の秘密コード】
```

### 8. 初回コンテナ起動
```bash
cd /opt/grocery-app
docker compose up -d --build
```

### 9. 動作確認
ブラウザで `https://grocery.yoshihamkorori.work` にアクセス。

---

## ③ GitHub Actions で自動デプロイ設定

### 1. GitHub にリポジトリを作成（プライベート）
```bash
# ローカルのプロジェクトフォルダで
git init
git add .
git commit -m "initial commit"
git remote add origin https://github.com/【ユーザー名】/grocery-app.git
git push -u origin main
```

### 2. GitHub Secrets を設定
リポジトリの Settings → Secrets and variables → Actions → New repository secret で以下を登録：

| Secret名 | 値 |
|---|---|
| `VPS_HOST` | VPSのIPアドレス（例: 123.456.789.0） |
| `VPS_USER` | SSHログインユーザー名 |
| `VPS_SSH_KEY` | SSH秘密鍵の中身（`cat ~/.ssh/id_rsa`） |
| `DEPLOY_DIR` | `/opt/grocery-app` |
| `GH_REPO` | `【ユーザー名】/grocery-app` |
| `GH_PAT` | GitHubのPersonal Access Token（後述） |

### 3. GitHub Personal Access Token (PAT) を作成
GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
- Expiration: 任意
- Scope: `repo` にチェック
- 生成されたトークンを `GH_PAT` に設定

### 4. SSH鍵をVPSに登録（まだの場合）
```bash
# ローカルで鍵生成
ssh-keygen -t ed25519 -C "github-actions"

# VPSの authorized_keys に公開鍵を追加
ssh-copy-id -i ~/.ssh/id_ed25519.pub user@VPSのIP

# 秘密鍵の中身をGitHub Secretsの VPS_SSH_KEY に貼り付け
cat ~/.ssh/id_ed25519
```

### 5. 自動デプロイの動作確認
```bash
git commit --allow-empty -m "test deploy"
git push origin main
```
GitHub → Actions タブでデプロイの進行状況が確認できる。

---

## Let's Encrypt 証明書の自動更新

certbot は `/etc/cron.d/certbot` に自動更新のcronを登録する。
手動確認する場合:
```bash
sudo certbot renew --dry-run
```

---

## API エンドポイント一覧

| Method | Path | 認証 | 説明 |
|---|---|---|---|
| POST | /api/auth/login | - | ログイン |
| POST | /api/auth/register | - | 新規登録（登録コード必須） |
| GET | /api/shoppinglist | ✓ | リスト取得 |
| POST | /api/shoppinglist/items | ✓ | アイテム追加 |
| PATCH | /api/shoppinglist/items/{id}/toggle | ✓ | チェック切替 |
| PUT | /api/shoppinglist/items/{id} | ✓ | アイテム編集 |
| DELETE | /api/shoppinglist/items/{id} | ✓ | アイテム削除 |
| DELETE | /api/shoppinglist/reset | ✓ | チェック済み削除 |
| GET | /api/masteritems | ✓ | よく買うもの一覧 |
| POST | /api/masteritems | ✓ | よく買うもの追加 |
| DELETE | /api/masteritems/{id} | ✓ | よく買うもの削除 |
\n<!-- Update: Adding contributing and PR template -->
