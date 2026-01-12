using MinimumAPI.Application;
using MinimumAPI.Infrastructure;
using MinimumAPI.Presentation.Endpoints;

// Web アプリのビルダーを生成し、ホスト/設定/DI の基盤を作る
var builder = WebApplication.CreateBuilder(args);

// サービス(DI)を登録する場所
// OpenAPI の設定については https://aka.ms/aspnet/openapi を参照
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// ここでアプリをビルドしてミドルウェア/ルーティングの器を作る
var app = builder.Build();

// HTTP リクエストパイプラインを構成
if (app.Environment.IsDevelopment())
{
    // 開発環境のみ OpenAPI エンドポイントを公開
    app.MapOpenApi();
}

// HTTP を HTTPS にリダイレクト
app.UseHttpsRedirection();

app.MapWeatherForecastEndpoints();

// アプリの受信開始
app.Run();
