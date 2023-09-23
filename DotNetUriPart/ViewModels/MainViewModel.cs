using System;
using Microsoft.Extensions.DependencyInjection;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace DotNetUriPart.ViewModels;

/// <summary>
/// MainView のビューモデル
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    // 構築
    #region コンストラクタ
    /// <summary>依存サービスを受け取るコンストラクタ</summary>
    /// <param name="provider">サービスプロバイダ</param>
    public MainViewModel(IServiceProvider provider)
    {
        // ツール表示パネル用のVM
        var toolVm = provider.GetRequiredService<ToolViewModel>().AddTo(this.Resources);

        // ライセンス表示パネル用のVM
        var licenseVm = provider.GetRequiredService<AboutViewModel>().AddTo(this.Resources);

        // アクティブパネルのVM
        var activeContent = new ReactivePropertySlim<ViewModelBase>(toolVm).AddTo(this.Resources);

        // ツール表示パネル選択コマンド
        this.SelectToolCommmand = new ReactiveCommandSlim()
            .WithSubscribe(() => activeContent.Value = toolVm, o => o.AddTo(this.Resources))
            .AddTo(this.Resources);

        // ライセンス表示パネル選択コマンド
        this.SelectLicenseCommmand = new ReactiveCommandSlim()
            .WithSubscribe(() => activeContent.Value = licenseVm, o => o.AddTo(this.Resources))
            .AddTo(this.Resources);

        // アクティブパネルのVM
        this.ActiveContent = activeContent.ToReadOnlyReactivePropertySlim().AddTo(this.Resources);
    }
    #endregion

    // 公開プロパティ
    #region Viewインタフェース
    /// <summary>ツール表示パネル選択コマンド</summary>
    public ReactiveCommandSlim SelectToolCommmand { get; }

    /// <summary>ライセンス表示パネル選択コマンド</summary>
    public ReactiveCommandSlim SelectLicenseCommmand { get; }

    /// <summary>アクティブパネルのVM</summary>
    public ReadOnlyReactivePropertySlim<ViewModelBase?> ActiveContent { get; }
    #endregion
}
