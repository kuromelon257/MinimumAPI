// Web アプリのビルダーを生成し、ホスト/設定/DI の基盤を作る
var builder = WebApplication.CreateBuilder(args);

// サービス(DI)を登録する場所
// OpenAPI の設定については https://aka.ms/aspnet/openapi を参照
builder.Services.AddOpenApi();

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

// 天気のダミー文言
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// GET /weatherforecast で 5 日分の予報を返す
app.MapGet("/weatherforecast", () =>
{
    // 今日から 5 日分のランダムなデータを生成
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            // 日付、気温(C)、概要をセット
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    // 生成した配列をそのまま返す（JSON 化される）
    return forecast;
})
// エンドポイント名を付ける（ルートの識別子）
.WithName("GetWeatherForecast");

// アプリの受信開始
app.Run();

// 予報データのモデル（レコード型）
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    // 華氏に換算した派生プロパティ
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
