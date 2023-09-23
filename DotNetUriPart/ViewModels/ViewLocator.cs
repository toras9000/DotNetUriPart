using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace DotNetUriPart.ViewModels;

/// <summary>
/// コンテキストデータに対応するビューインスタンスを決定するビューロケータ
/// </summary>
/// <remarks>
/// このロケータでは以下のようにビューインスタンスを解決する。
/// <list type="bullet">
///  <item>事前登録されたデータ型とビュー型対応付けに沿ってビューを生成
///   <list type="bullet">
///    <item>登録あれば優先的に解決する。</item>
///    <item>データ型に制約はない。</item>
///   </list>
///  </item>
///  <item>名称ベースでデータ型からビュー型の対応付け(ViewModel→View)を試みてビューを生成
///   <list type="bullet">
///    <item>データ型が ViewModelBase を継承している場合のみ。</item>
///   </list>
///  </item>
/// </list>
/// ビューのインスタンス生成では初めに規定のサービスプロバイダからインスタンスの取得を試み、
/// 取得できない場合はデフォルトのアクティベータによるインスタンス生成を行う。
/// それでもビューのインスタンスを決定されない場合、データの文字列表現を表示するTextBlockを生成する。
/// </remarks>
public class ViewLocator : IDataTemplate
{
    // 構築
    #region コンストラクタ
    /// <summary>依存サービスを受け取るコンストラクタ</summary>
    /// <param name="provider">サービスプロバイダ</param>
    public ViewLocator(IServiceProvider provider)
    {
        this.provider = provider;
        this.binder = new();
    }
    #endregion

    // 公開メソッド
    #region 型の登録
    /// <summary>データ型と対応するビューのファクトリを登録する</summary>
    /// <typeparam name="TViewModel">データ型</typeparam>
    /// <param name="factory">ビューのファクトリ</param>
    /// <returns>自身のロケータインスタンス</returns>
    public ViewLocator Register<TViewModel>(Func<Control> factory)
    {
        this.binder[typeof(TViewModel)] = factory;
        return this;
    }

    /// <summary>データ型と対応するビュー型を登録する</summary>
    /// <typeparam name="TViewModel">データ型</typeparam>
    /// <typeparam name="TView">ビュー型</typeparam>
    /// <returns>自身のロケータインスタンス</returns>
    public ViewLocator Register<TViewModel, TView>() where TView : Control
        => Register<TViewModel>(() => createViewInstance(typeof(TView))!);
    #endregion

    #region ビューの取得
    /// <inheritdoc />
    public bool Match(object? data)
    {
        if (data == null) return false;

        // 登録済みデータ型であるか
        if (this.binder.ContainsKey(data.GetType())) return true;

        // VMのベースクラスから派生しているか
        return data is ViewModelBase;
    }

    /// <inheritdoc />
    public Control Build(object? param)
    {
        // ビューのインスタンスを構築する
        var view = buildViewInstance(param);
        if (view == null)
        {
            // 対応するビューを生成できなかったらテキスト表示ビューを作っておく
            view = new TextBlock { Text = param?.ToString(), };
        }

        return view;
    }
    #endregion

    // 非公開フィールド
    #region 生成材料
    /// <summary>サービスプロバイダ</summary>
    private readonly IServiceProvider provider;

    /// <summary>データ型とビューファクトリの対応ディクショナリ</summary>
    private readonly Dictionary<Type, Func<Control>> binder;
    #endregion

    // 非公開メソッド
    #region インスタンス生成
    /// <summary>ビューの型からインスタンスを生成する</summary>
    /// <param name="viewType">ビューの型</param>
    /// <returns>生成したビューインスタンス</returns>
    private Control? createViewInstance(Type viewType)
    {
        var instance = this.provider.GetService(viewType) ?? Activator.CreateInstance(viewType);

        return instance as Control;
    }

    /// <summary>コンテキストデータに対するビューインスタンスを生成する</summary>
    /// <param name="param">コンテキストデータ</param>
    /// <returns>生成したビューインスタンス</returns>
    private Control? buildViewInstance(object? param)
    {
        if (param == null) return null;

        // データの型の取得
        var paramType = param.GetType();
        if (paramType == null) return null;

        // データ型に対するビューの登録があるか
        if (this.binder.TryGetValue(paramType, out var factory))
        {
            // ビューファクトリで生成
            return factory();
        }

        // 型名ベースで対応するビュー型の推定をする。
        var viewTypeName = paramType.FullName?.Replace("ViewModel", "View");
        if (viewTypeName == null) return null;

        // 推定した型がビュー型であるかを判定
        var viewType = Type.GetType(viewTypeName);
        if (viewType == null) return null;
        if (!viewType.IsAssignableTo(typeof(Control))) return null;

        // ビュー生成
        return createViewInstance(viewType);
    }
    #endregion
}
