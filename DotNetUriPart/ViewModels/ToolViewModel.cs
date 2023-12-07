using System;
using System.Reactive.Linq;
using DotNetUriPart.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace DotNetUriPart.ViewModels;

/// <summary>
/// ToolView のビューモデル
/// </summary>
public partial class ToolViewModel : ViewModelBase
{
    #region コンストラクタ
    /// <summary>依存サービスを受け取るコンストラクタ</summary>
    /// <param name="processor">URI分解処理</param>
    /// <param name="clipboard">クリップボードI/F</param>
    /// <param name="appConstants">アプリ定数</param>
    public ToolViewModel(IUriProcessor processor, IClipboardAccessor clipboard, IAppConstantsProvider appConstants)
    {
        // 入力テキスト
        this.InputText = new ReactivePropertySlim<string?>(appConstants.SampleUri).AddTo(this.Resources); ;

        // 入力のURI変換結果
        this.InputState = this.InputText
            .Select(text =>
            {
                if (string.IsNullOrEmpty(text)) return new InputStateInfo(default, default);
                if (!Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out var uri)) return new InputStateInfo(default, "Failed to create Uri");
                return new InputStateInfo(uri, default);
            })
            .ToReadOnlyReactivePropertySlim()
            .AddTo(this.Resources);

        // URI部分文字列情報リスト
        this.UriParts = this.InputState
            .Throttle(TimeSpan.FromSeconds(1))
            .ObserveOnUIDispatcher()
            .Select(state => processor.SplitParts(state?.Uri))
            .ToReadOnlyReactivePropertySlim()
            .AddTo(this.Resources);

        // URI部分取得コードのコピーコマンド
        this.CopyCodeCommand = new AsyncReactiveCommand<UriPart?>()
            .WithSubscribe(i => clipboard.SetTextAsync(i?.Code), o => o.AddTo(this.Resources))
            .AddTo(this.Resources);

        // URI部分文字列コピーコマンド
        this.CopyPartCommand = new AsyncReactiveCommand<UriPart?>()
            .WithSubscribe(i => clipboard.SetTextAsync(i?.Value), o => o.AddTo(this.Resources))
            .AddTo(this.Resources);
    }
    #endregion

    // 公開プロパティ
    #region データ型
    /// <summary>入力URIの変換結果情報型</summary>
    /// <param name="Uri">変換結果のURI(変換可能時)</param>
    /// <param name="Error">変換エラーメッセージ(エラー時)</param>
    /// <remarks>
    /// Uri クラスの等価判定は Fragment の差異を無視したりするため、等価判定は record 生成物を上書きする。
    /// </remarks>
    public record InputStateInfo(Uri? Uri, string? Error)
    {
        /// <inheritdoc />
        bool IEquatable<InputStateInfo>.Equals(InputStateInfo? other)
            => this.Uri?.OriginalString == other?.Uri?.OriginalString && this.Error == other?.Error;

        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(this.Uri?.OriginalString, this.Error);
    }
    #endregion

    // 公開プロパティ
    #region Viewインタフェース
    /// <summary>入力テキスト</summary>
    public ReactivePropertySlim<string?> InputText { get; }

    /// <summary>入力のURI変換結果</summary>
    public ReadOnlyReactivePropertySlim<InputStateInfo?> InputState { get; }

    /// <summary>URI部分文字列情報リスト</summary>
    public ReadOnlyReactivePropertySlim<UriPart[]?> UriParts { get; }

    /// <summary>URI部分取得コードのコピーコマンド</summary>
    public AsyncReactiveCommand<UriPart?> CopyCodeCommand { get; }

    /// <summary>URI部分文字列コピーコマンド</summary>
    public AsyncReactiveCommand<UriPart?> CopyPartCommand { get; }
    #endregion
}
