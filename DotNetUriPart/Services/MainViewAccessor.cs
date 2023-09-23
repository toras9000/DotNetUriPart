using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace DotNetUriPart.Services;

/// <summary>
/// メインビューへのアクセスサービス
/// </summary>
/// <remarks>
/// Avalonia v0.11 ではクリップボードへのアクセスがビューを介する必要があるよう(Android の仕様に引っ張られている模様)なので、主にその補助用。
/// </remarks>
public interface IMainViewAccessor
{
    /// <summary>メインウィンドウを取得する</summary>
    /// <returns>メインウィンドウインスタンス。非デスクトップアプリの場合は null </returns>
    Window? GetMainWindow();

    /// <summary>メインビューを取得する。</summary>
    /// <returns></returns>
    Control? GetMainView();
}

/// <summary>
/// メインビューへのアクセスサービス Avalonia実装
/// </summary>
public class AvaloniaMainViewAccessor : IMainViewAccessor
{
    /// <inheritdoc />
    public Window? GetMainWindow()
    {
        return (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
    }

    /// <inheritdoc />
    public Control? GetMainView()
    {
        if (Application.Current is var app && app is not null)
        {
            if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
            {
                return desktopApp.MainWindow;
            }
            else if (app.ApplicationLifetime is ISingleViewApplicationLifetime singleViewApp)
            {
                return singleViewApp.MainView;
            }
        }

        return null;
    }
}
