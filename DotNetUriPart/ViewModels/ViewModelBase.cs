using System;
using System.Reactive.Disposables;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DotNetUriPart.ViewModels;

/// <summary>
/// アプリケーションのViewModelベースクラス
/// </summary>
public class ViewModelBase : ObservableObject, IDisposable
{
    // 構築
    #region コンストラクタ
    /// <summary>デフォルトコンストラクタ</summary>
    public ViewModelBase()
    {
        this.Resources = new CompositeDisposable();
    }
    #endregion

    // 公開プロパティ
    #region リソース管理
    /// <summary>破棄予定リソース</summary>
    public CompositeDisposable Resources { get; }
    #endregion

    // 公開メソッド
    #region 破棄
    /// <summary>リソース破棄</summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    // 保護プロパティ
    #region 状態
    /// <summary>破棄済みフラグ</summary>
    protected bool IsDisposed { get; private set; }
    #endregion

    // 保護メソッド
    #region 破棄
    /// <summary>リソース破棄</summary>
    /// <param name="disposing">マネージ破棄過程であるか否か</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.IsDisposed)
        {
            // マネージ破棄過程であれば、マネージオブジェクトを破棄する。
            if (disposing)
            {
                // マネージオブジェクト破棄
                this.Resources.Dispose();
            }

            // 破棄済みをマーク
            this.IsDisposed = true;
        }
    }
    #endregion
}
