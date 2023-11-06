using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using DotNetUriPart.Services;
using DotNetUriPart.ViewModels;
using DotNetUriPart.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetUriPart;

/// <summary>
/// Avalonia ベースのアプリケーションクラス
/// </summary>
public partial class App : Application
{
    // 構築
    #region コンストラクタ
    /// <summary>デフォルトコンストラクタ</summary>
    public App()
    {
    }
    #endregion

    // 公開メソッド
    #region フレームワーク
    /// <inheritdoc />
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc />
    public override void RegisterServices()
    {
        base.RegisterServices();

        // 構成ファイル
        var config = new ConfigurationBuilder()
            .SetBasePath(System.IO.Path.GetDirectoryName(Environment.ProcessPath)!)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        // 型の登録
        var services = new ServiceCollection();

        services.AddLogging(builder => builder
            .AddDebug()
            .AddConfiguration(config.GetSection("Logging"))
        );

        services.AddTransient<IAppConstantsProvider, AppConstantsProvider>();
        services.AddTransient<ILibraryInfoProvider, LibraryInfoProvider>();
        services.AddTransient<IUriProcessor, UriProcessor>();
        services.AddTransient<IMainViewAccessor, AvaloniaMainViewAccessor>();
        services.AddTransient<IClipboardAccessor, AvaloniaClipboardAccessor>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<ToolViewModel>();
        services.AddTransient<AboutViewModel>();

        // サービスプロバイダを構築してCommunityToolkitのデフォルトプロバイダに設定しておく
        Ioc.Default.ConfigureServices(services.BuildServiceProvider());

        // 独自のビューロケータをアプリケーションに追加しておく
        var locator = new ViewLocator(Ioc.Default);
        this.DataTemplates.Add(locator);
    }

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        // CommunityToolkit のバリデーションを利用する場合、バリデータの重複を防ぐためにAvaloniaのバリデータを取り除く必要がある、ということのよう。
        BindingPlugins.DataValidators.RemoveAt(0);

        // アプリケーションタイプごとのメインビュー初期化
        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
        {
            desktopApp.MainWindow = new MainWindow() { DataContext = Ioc.Default.GetService<MainViewModel>(), };
        }
        else if (this.ApplicationLifetime is ISingleViewApplicationLifetime singleViewApp)
        {
            singleViewApp.MainView = new MainView() { DataContext = Ioc.Default.GetService<MainViewModel>(), };
        }

        base.OnFrameworkInitializationCompleted();
    }
    #endregion

}
