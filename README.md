# MinimumAPI

ASP.NET Core Minimal API の最小サンプルです。`/weatherforecast` を返す API と、開発時の OpenAPI 公開が含まれます。

## ファイル/フォルダの役割

- `Program.cs`  
  アプリのエントリーポイント。サービス登録、ミドルウェア設定、`/weatherforecast` のエンドポイント定義があります。
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
