using System;
using System.IO;
using System.Reactive;
using Avalonia;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using MicroCom.Runtime;
using Reactive.Bindings;

namespace DotNetUriPart.Services;

/// <summary>ライセンス情報</summary>
/// <param name="Kind">ライセンス種別</param>
/// <param name="Text">本文</param>
public record LicenseInfo(string Kind, string? Text);

/// <summary>ライブラリ情報</summary>
/// <param name="Name">名称</param>
/// <param name="Version">バージョン</param>
/// <param name="Site">サイトURL</param>
/// <param name="License">ライセンス情報</param>
public record LibraryInfo(string Name, string? Version, string? Site, LicenseInfo License);

/// <summary>
/// 使用ライブラリ情報プロバイダ
/// </summary>
public interface ILibraryInfoProvider
{
    /// <summary>ライブラリ情報を取得する</summary>
    /// <returns>ライブラリ情報</returns>
    LibraryInfo[] GetLibraryInfos();
}

/// <summary>
/// 使用ライブラリ情報プロバイダ
/// </summary>
public class LibraryInfoProvider : ILibraryInfoProvider
{
    /// <inheritdoc />
    public LibraryInfo[] GetLibraryInfos()
    {
        return infoCache ?? (infoCache = new LibraryInfo[]
        {
            new(".NET Runtime",           getVer<object>(),             @"https://github.com/dotnet/runtime",            getLicense("MIT License", ".NET Runtime.txt")),
            new("Avalonia",               getVer<Application>(),        @"https://github.com/AvaloniaUI/Avalonia",       getLicense("MIT License", "Avalonia.md")),
            new("CommunityToolkit.Mvvm",  getVer<ObservableObject>(),   @"https://github.com/CommunityToolkit/dotnet",   getLicense("MIT License", "CommunityToolkit.Mvvm.md")),
            new("ReactiveProperty",       getVer<ReactiveCommand>(),    @"https://github.com/runceel/ReactiveProperty",  getLicense("MIT License", "ReactiveProperty.txt")),
            new("System.Reactive",        getVer<ObserverBase<int>>(),  @"https://github.com/dotnet/reactive",           getLicense("MIT License", "System.Reactive.txt")),
            new("MicroCom",               getVer<MicroComProxyBase>(),  @"https://github.com/kekekeks/MicroCom",         getLicense("MIT License", "MicroCom.txt")),
            new("SkiaSharp",              "2.88.3",                     @"https://github.com/mono/SkiaSharp",            getLicense("MIT License", "SkiaSharp.txt")),
            new("HarfBuzzSharp",          "2.8.2.3",                    @"https://github.com/mono/SkiaSharp",            getLicense("MIT License", "SkiaSharp.txt")),   // SkiaSharpと同じ。
            new("Tmds.DBus.Protocol",     "0.15.0",                     @"https://github.com/tmds/Tmds.DBus",            getLicense("MIT License", "Tmds.DBus.Protocol.txt")),
            new("ANGLE",                  "2.1.0.2023020321",           @"https://github.com/AvaloniaUI/angle",          getLicense("ANGLE License", "ANGLE.txt")),
        });
    }

    /// <summary>ライブラリ情報のキャッシュ</summary>
    private LibraryInfo[]? infoCache;

    /// <summary>型情報からそれを含むアセンブリのバージョンを取得する</summary>
    /// <typeparam name="T">対象型</typeparam>
    /// <returns>バージョン</returns>
    private string? getVer<T>() => typeof(T).Assembly.GetName().Version?.ToString(fieldCount: 3);

    /// <summary>リソースからライセンス情報を取得する</summary>
    /// <param name="kind">ライセンス種別</param>
    /// <param name="res">リソースのライセンスファイル名</param>
    /// <returns>ライセンス情報</returns>
    private LicenseInfo getLicense(string kind, string res)
    {
        var text = default(string?);
        try
        {
            using var stream = AssetLoader.Open(new($"avares://DotNetUriPart/Assets/Licenses/{res}"));
            using var reader = new StreamReader(stream);
            text = reader.ReadToEnd();
        }
        catch { }

        return new(kind, text);
    }
}
