# MinimumAPI

ASP.NET Core Minimal API の小〜中規模向けクリーンアーキテクチャ構成です。`/weatherforecast` を返す API と、開発時の OpenAPI 公開が含まれます。

## ファイル/フォルダの役割

- `Program.cs`  
  アプリのエントリーポイント。DI 登録とミドルウェア設定、エンドポイント登録を行います。
- `Application/`  
  ユースケースの境界とアプリケーション層の型を置きます。
- `Domain/`  
  業務概念のモデルを定義します。
- `Infrastructure/`  
  外部依存や具体実装をまとめます。
- `Presentation/`  
  API のエンドポイント定義をまとめます。
- `MinimumAPI.csproj`  
  .NET ターゲットや参照パッケージなど、ビルド設定をまとめたプロジェクトファイルです。
- `MinimumAPI.http`  
  VSCode の REST Client などで API を叩くためのサンプルリクエストです。
- `appsettings.json`  
  環境共通の設定（ログレベルなど）を定義します。
- `appsettings.Development.json`  
  開発環境専用の設定です。本番には適用されません。
- `Properties/launchSettings.json`  
  `dotnet run` で使う起動プロファイル（URL、環境変数など）の設定です。
- `bin/`  
  ビルド成果物が出力されるフォルダです。
- `obj/`  
  中間生成物が出力されるフォルダです。

## ビルドと実行

```bash
dotnet build
dotnet run
```

## テスト

```bash
dotnet test
```

## テストでモックを自動生成する（Moq + AutoFixture.AutoMoq）

Moqは定番のモックライブラリで、AutoFixture.AutoMoqを使うと依存を自動的にモック化してくれます。  
テスト対象のクラスを `fixture.Create<T>()` で生成すると、コンストラクタ依存が自動でモックになります。

例（テストコード）：

```csharp
using Moq;
using AutoFixture;
using AutoFixture.AutoMoq;
using MinimumAPI.Application.Abstractions;

var fixture = new Fixture().Customize(new AutoMoqCustomization());
var queryMock = fixture.Freeze<Mock<IWeatherForecastQuery>>();
queryMock.Setup(query => query.GetForecasts())
    .Returns(new List<WeatherForecast>());

var service = fixture.Create<WeatherForecastService>();
```

具体例は `MinimumAPI.Tests/Application/AutoMockerTests.cs` を参照してください。

## コンテナ化とAWSデプロイ（App Runner 推奨）

### ローカルでコンテナを起動

```bash
docker build -t minimumapi:latest -f MinimumAPI/Dockerfile MinimumAPI
docker run --rm -p 8080:8080 minimumapi:latest
```

```bash
curl http://localhost:8080/weatherforecast
```

### ECRへPush

```bash
# リポジトリを作成
aws ecr create-repository --repository-name minimumapi
# ECRへログイン（ECRにpushできるようにDockerを認証）
aws ecr get-login-password --region ap-northeast-1 | \
  docker login --username AWS --password-stdin <account-id>.dkr.ecr.ap-northeast-1.amazonaws.com

# Dockerイメージをビルド（ローカル実行向け）
docker build -t minimumapi:latest -f MinimumAPI/Dockerfile MinimumAPI
# ECRリポジトリ向けにタグ付け
docker tag minimumapi:latest <account-id>.dkr.ecr.ap-northeast-1.amazonaws.com/minimumapi:latest
# ECRへプッシュ
docker push <account-id>.dkr.ecr.ap-northeast-1.amazonaws.com/minimumapi:latest

# buildx で linux/amd64 をビルドしてそのままpush（Apple Silicon向け）
docker buildx build --platform linux/amd64 \
  -t <account-id>.dkr.ecr.ap-northeast-1.amazonaws.com/minimumapi:latest \
  -f MinimumAPI/Dockerfile \
  --push MinimumAPI
```

`<account-id>` は自分のAWSアカウントIDに置き換えてください。

## ECS + ECR で .NET Minimal API を公開して叩けるようにする手順（詰まりポイント込み）

目的：.NET（Minimum API）のコンテナイメージを ECR に置き、ECS(Fargate) で起動して、Public IP 経由で curl できるようにする。  
リージョン：東京（ap-northeast-1）

### 全体像（最初に理解）
- ECR：Dockerイメージの保管場所（倉庫）
- ECS：Dockerイメージを起動して動かす場所（実行基盤）
- 今回は「ECRに置く」→「ECSで動かす」→「Public IPで叩く」の流れ

### 0. 前提：AWS CLI を使える状態を確認する（CloudShell推奨）

AWSコンソール左下の CloudShell を開いて実行。

#### 0-1. 認証確認（CLIが動くか）

```bash
aws sts get-caller-identity
```

期待：Account / Arn が返ること。

#### 0-2. リージョン確認（東京になっているか）

```bash
aws configure list
```

期待：`region : ap-northeast-1` になっていること。  
この時点で CLI が東京を向いていないと「作ったはずなのにコンソールに見えない」が起きがち。

### 1. ECR リポジトリを作成する

#### 1-1. ECR リポジトリ作成

```bash
aws ecr create-repository --repository-name minimumapi --region ap-northeast-1
```

#### 1-2. 作成できたか確認

```bash
aws ecr describe-repositories --region ap-northeast-1
```

期待：`minimumapi` が出ること。  
AWSコンソールのホーム画面にはECRリポジトリは出ないので、ECR → Repositories を確認。

### 2. Dockerイメージを ECR に push する

#### 2-1. ECRへログイン

```bash
aws ecr get-login-password --region ap-northeast-1 \
| docker login --username AWS --password-stdin <account-id>.dkr.ecr.ap-northeast-1.amazonaws.com
```

#### 2-2. build → tag → push（例）

```bash
cd MinimumAPI
docker build -t minimumapi:latest -f MinimumAPI/Dockerfile .
docker tag minimumapi:latest <account-id>.dkr.ecr.ap-northeast-1.amazonaws.com/minimumapi:latest
docker push <account-id>.dkr.ecr.ap-northeast-1.amazonaws.com/minimumapi:latest
```

### 3. ECRの「イメージが3つある」問題

ECR → minimumapi → イメージ に、例えば次のように 3行 見えることがある。  
`latest`（タイプ：Image Index） / `-`（タイプ：Image） / `-`（タイプ：Image）

これは正しい。  
`latest` は目次（マニフェスト/index）、下の2つは実体のイメージ（arm64/amd64 など）。

### 4. ECSクラスター作成で失敗（サービスリンクロール）

クラスター作成で以下のようなエラーになることがある：  
`Unable to assume the service linked role. Please verify that the ECS service linked role exists.`

原因：ECSが内部で使う Service Linked Role が無い/作れない。

対処（権限がある場合）：

```bash
aws iam create-service-linked-role --aws-service-name ecs.amazonaws.com
```

権限不足なら IAM ポリシー（AmazonECSFullAccess など）付与が必要。

### 5. ECSに「タスク定義」が無い

サービスを作ろうとしても タスク定義 が無いと起動できない。

#### 5-1. タスク定義（minimumapi-task）を作る

ECS → タスク定義 → 新しいタスク定義を作成 → Fargate

推奨設定（学習用の最小）：
- OS/アーキテクチャ：Linux / x86_64（またはARM64）
- CPU：0.25 vCPU
- メモリ：0.5GB
- コンテナ：
  - 画像URI：`<account-id>.dkr.ecr.ap-northeast-1.amazonaws.com/minimumapi:latest`
  - ポートマッピング：`8080`
  - 環境変数：`ASPNETCORE_URLS=http://+:8080`
  - ログ：CloudWatchログを有効化（推奨）

### 6. サービス作成（Fargate）→ Public IP を有効化

ECS → クラスター → minimumapi-cluster → サービス → 作成

重要設定：
- 起動タイプ：Fargate
- 希望タスク数：1
- ネットワーク：
  - Public IP：有効（ON）
  - サブネット：1つ以上
  - セキュリティグループ：後述

### 7. タスクが全部「停止済み」になる（プラットフォーム不一致）

イベント/ログに以下が出る：  
`CannotPullContainerError: image Manifest does not contain descriptor matching platform 'linux/amd64'`

原因：ECS(Fargate)が linux/amd64 で起動しようとしているのに、ECRに amd64 が無い。

対処（どちらか）：
- ECS側を ARM64 に合わせる
- amd64でビルドして push し直す（Apple Silicon でも buildx で可能）

例（amd64でpush）：

```bash
docker buildx build --platform linux/amd64 \
  -t <account-id>.dkr.ecr.ap-northeast-1.amazonaws.com/minimumapi:latest \
  -f MinimumAPI/Dockerfile \
  --push .
```

### 8. pushしても反映されない（ECSは自動で取り直さないことがある）

ECRに新しい latest を push しても、ECSはそのままだと古いタスクを使い続けることがある。  
サービスを更新して「新しいデプロイを強制」をONにする。

### 9. Public IPが取れてもcurlが返らない（SGが閉じてる）

Public IPが取れても、Security GroupのInboundが閉じていると外部から叩けない。  
セキュリティグループで 8080 を 0.0.0.0/0 に許可（学習用。後で自分のIPに絞る推奨）。

### 10. 叩く（curl）

```bash
curl http://<public-ip>:8080/
curl http://<public-ip>:8080/weatherforecast
```

### 11. 後片付け（課金対策）

ECS → サービス → 更新 → 希望するタスク数を 0 → 更新  
必要ならクラスター/サービス削除も行う。

### よくある詰まりポイントまとめ

1. ECRはホーム画面に出ない → ECR画面で確認
2. サービスリンクロール問題 → 権限 or SLR作成
3. タスク定義が無いと起動できない
4. amd64/arm64不一致 → ECSアーキ or イメージを合わせる
5. Public IPが取れてもSGが閉じてると叩けない → 8080をInboundで開ける
6. pushしてもECSは自動で最新化しないことがある → 新しいデプロイを強制

### App Runnerで起動

1. AWSコンソールで App Runner を開き、新規サービス作成
2. ソースに「ECR」を選択し、`minimumapi:latest` を指定
3. ポートに `8080` を設定してデプロイ

デプロイ後に払い出されるURLで `/weatherforecast` を呼べます。

## 本番向け（堅牢性重視）: ECS Fargate + ALB

App Runnerより設定は増えますが、ネットワークやセキュリティを細かく制御できます。

### 前提

- ECRへイメージがpush済み
- AWS CLIにログイン済み

### 手順（初心者向けの最小構成）

1. VPCとサブネットを用意（既存のデフォルトVPCでも可）
2. セキュリティグループ作成
   - ALB: 80/443 をインバウンド許可（0.0.0.0/0）
   - ECSタスク: ALBのセキュリティグループから 8080 を許可
3. ECSクラスタを作成（Fargate）
4. タスク定義を作成
   - 画像: `<account-id>.dkr.ecr.ap-northeast-1.amazonaws.com/minimumapi:latest`
   - ポート: `8080`
   - 必要なら環境変数: `ASPNETCORE_URLS=http://+:8080`
5. サービス作成
   - 起動タイプ: Fargate
   - タスク数: 1（本番は2以上推奨）
   - ロードバランサ: ALBを選択
   - ターゲットグループ: `8080` を指定
6. ALBのDNS名にアクセス
   - `http://<alb-dns-name>/weatherforecast`

### さらに細かい画面手順（AWSコンソール）

1. **VPC確認**: VPC → "Your VPCs" でデフォルトVPCがあるか確認
2. **ALB作成**: EC2 → "Load Balancers" → "Create load balancer" → "Application Load Balancer"
   - スキーム: internet-facing
   - リスナー: HTTP 80
   - サブネット: 2つ以上選択
   - セキュリティグループ: 80/443を許可
3. **ターゲットグループ作成**: EC2 → "Target Groups" → "Create target group"
   - タイプ: IP
   - プロトコル/ポート: HTTP / 8080
4. **ECSクラスタ作成**: ECS → "Clusters" → "Create cluster"
   - インフラ: AWS Fargate
5. **タスク定義作成**: ECS → "Task definitions" → "Create new task definition"
   - 起動タイプ: Fargate
   - コンテナ: `minimumapi`
   - 画像: `<account-id>.dkr.ecr.ap-northeast-1.amazonaws.com/minimumapi:latest`
   - ポートマッピング: 8080
6. **サービス作成**: ECS → クラスタ → "Create service"
   - サービス: Fargate
   - タスク数: 1（本番は2以上推奨）
   - ロードバランサ: 先ほどのALB
   - ターゲットグループ: 先ほどのTG
7. **疎通確認**: ALBのDNS名でアクセス
   - `http://<alb-dns-name>/weatherforecast`

### あると安心な追加設定

- オートスケーリング（CPU/メモリ閾値）
- WAF（ALBの前段で保護）
- CloudWatch Logs / アラーム

`<account-id>` は自分のAWSアカウントIDに置き換えてください。
