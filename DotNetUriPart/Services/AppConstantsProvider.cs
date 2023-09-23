namespace DotNetUriPart.Services;

/// <summary>
/// アプリケーション定数プロバイダ
/// </summary>
public interface IAppConstantsProvider
{
    /// <summary>アプリケーション名</summary>
    string AppName { get; }

    /// <summary>アプリケーションバージョン</summary>
    string AppVersion { get; }

    /// <summary>アプリケーションライセンス表記</summary>
    string AppLicense { get; }

    /// <summary>コピーライト年</summary>
    string CopyYear { get; }

    /// <summary>作者名</summary>
    string Author { get; }

    /// <summary>起動時サンプルＵＲＬ</summary>
    string SampleUri { get; }
}

/// <summary>
/// アプリケーション定数プロバイダ
/// </summary>
public class AppConstantsProvider : IAppConstantsProvider
{
    // 構築
    #region コンストラクタ
    /// <summary>デフォルトコンストラクタ</summary>
    public AppConstantsProvider()
    {
        this.AppName = this.GetType().Assembly.GetName().Name ?? "Unknown";
        this.AppVersion = this.GetType().Assembly.GetName().Version?.ToString(fieldCount: 3) ?? "-";
        this.AppLicense = "MIT License";
        this.CopyYear = "2023";
        this.Author = "toras9000";
        this.SampleUri = @"https://user:pass@example.com/path/to/resource?param1=X&param2=Y#section";
    }
    #endregion

    // 公開プロパティ
    #region 定数
    /// <inheritdoc />
    public string AppName { get; }

    /// <inheritdoc />
    public string AppVersion { get; }

    /// <inheritdoc />
    public string AppLicense { get; }

    /// <inheritdoc />
    public string CopyYear { get; }

    /// <inheritdoc />
    public string Author { get; }

    /// <inheritdoc />
    public string SampleUri { get; }
    #endregion
}
