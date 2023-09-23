using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;

namespace DotNetUriPart.Services;

/// <summary>
/// クリップボードアクセスサービス
/// </summary>
public interface IClipboardAccessor
{
    /// <summary>クリップボードテキストを取得する</summary>
    /// <returns>取得テキスト</returns>
    Task<string?> GetTextAsync();

    /// <summary>クリップボードにテキストを設定する</summary>
    /// <param name="text">設定するテキスト</param>
    Task SetTextAsync(string? text);

    /// <summary>クリップボード内容をクリアする</summary>
    Task ClearAsync();
}

/// <summary>
/// クリップボードアクセスサービス Avalonia実装
/// </summary>
public class AvaloniaClipboardAccessor : IClipboardAccessor
{
    // 構築
    #region コンストラクタ
    /// <summary>依存サービスを受け取るコンストラクタ</summary>
    /// <param name="viewAccessor">メインビューアクセサ</param>
    public AvaloniaClipboardAccessor(IMainViewAccessor viewAccessor)
    {
        this.viewAccessor = viewAccessor;
    }
    #endregion

    // 公開プロパティ
    #region クリップボードアクセス
    /// <inheritdoc />
    public Task<string?> GetTextAsync() => getClipboard().GetTextAsync();

    /// <inheritdoc />
    public Task SetTextAsync(string? text) => getClipboard().SetTextAsync(text);

    /// <inheritdoc />
    public Task ClearAsync() => getClipboard().ClearAsync();
    #endregion

    // 非公開フィールド
    #region 依存サービス
    /// <summary>メインビューアクセサ</summary>
    private IMainViewAccessor viewAccessor;

    /// <summary>AvaloniaクリップボードI/F</summary>
    private IClipboard? backend;
    #endregion

    #region 依存サービス
    /// <summary>クリップボードI/Fを取得する</summary>
    /// <remarks>
    /// 実行するタイミングによっては準備ができていない可能性があるので、毎回このメソッドで取得を試みる。
    /// </remarks>
    /// <returns>クリップボードI/F</returns>
    private IClipboard getClipboard()
    {
        // まだ取得できていなければ取得を試みる。
        if (this.backend == null)
        {
            if (viewAccessor.GetMainView() is { } view)
            {
                if (TopLevel.GetTopLevel(view) is { } topView)
                {
                    this.backend = topView.Clipboard;
                }
            }
        }

        return this.backend ?? throw new NotSupportedException();
    }
    #endregion
}
