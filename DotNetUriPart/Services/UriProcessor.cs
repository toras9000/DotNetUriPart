using System;

namespace DotNetUriPart.Services;

/// <summary>URI部分情報</summary>
/// <param name="Member">部分情報取得メンバ</param>
/// <param name="Value">部分情報</param>
/// <param name="Valid">部分情報取得可能か否か</param>
/// <param name="Code">コピーするコード</param>
public record UriPart(string Member, string Value, bool Valid, string Code);

/// <summary>
/// URIの分解処理
/// </summary>
public interface IUriProcessor
{
    /// <summary>URIを部分情報に分解する</summary>
    /// <param name="uri">対象URI</param>
    /// <returns>分解された各部分情報のリスト</returns>
    UriPart[]? SplitParts(Uri? uri);
}

/// <summary>
/// URIの分解処理 実装
/// </summary>
public class UriProcessor : IUriProcessor
{
    /// <inheritdoc />
    public UriPart[]? SplitParts(Uri? uri)
    {
        if (uri == null) return null;

        // 部分情報の切り出しを試みる
        static UriPart getUriPart(Uri uri, string caption, Func<Uri, string> getter, string? code = null)
        {
            var codeText = $"uri.{code ?? caption}";
            try
            {
                return new(caption, getter(uri), true, codeText);
            }
            catch (Exception ex)
            {
                return new(caption, $"Error:{ex.GetType().Name}", false, codeText);
            }
        }

        return new UriPart[]
        {
            getUriPart(uri, nameof(Uri.OriginalString), uri => uri.OriginalString),
            getUriPart(uri, nameof(Uri.IsAbsoluteUri),  uri => $"{uri.IsAbsoluteUri}"),
            getUriPart(uri, nameof(Uri.IsLoopback),     uri => $"{uri.IsLoopback}"),
            getUriPart(uri, nameof(Uri.IsFile),         uri => $"{uri.IsFile}"),
            getUriPart(uri, nameof(Uri.IsUnc),          uri => $"{uri.IsUnc}"),
            getUriPart(uri, nameof(Uri.UserEscaped),    uri => $"{uri.UserEscaped}"),
            getUriPart(uri, nameof(Uri.AbsoluteUri),    uri => uri.AbsoluteUri),
            getUriPart(uri, nameof(Uri.Scheme),         uri => uri.Scheme),
            getUriPart(uri, nameof(Uri.UserInfo),       uri => uri.UserInfo),
            getUriPart(uri, nameof(Uri.Authority),      uri => uri.Authority),
            getUriPart(uri, nameof(Uri.Host),           uri => uri.Host),
            getUriPart(uri, nameof(Uri.DnsSafeHost),    uri => uri.DnsSafeHost),
            getUriPart(uri, nameof(Uri.IdnHost),        uri => uri.IdnHost),
            getUriPart(uri, nameof(Uri.HostNameType),   uri => $"{uri.HostNameType}"),
            getUriPart(uri, nameof(Uri.Port),           uri => $"{uri.Port}"),
            getUriPart(uri, nameof(Uri.IsDefaultPort),  uri => $"{uri.IsDefaultPort}"),
            getUriPart(uri, nameof(Uri.AbsolutePath),   uri => uri.AbsolutePath),
            getUriPart(uri, nameof(Uri.LocalPath),      uri => uri.LocalPath),
            getUriPart(uri, nameof(Uri.PathAndQuery),   uri => uri.PathAndQuery),
            getUriPart(uri, nameof(Uri.Query),          uri => uri.Query),
            getUriPart(uri, nameof(Uri.Fragment),       uri => uri.Fragment),
            getUriPart(uri, "GetLeftPart(Scheme)",      uri => uri.GetLeftPart(UriPartial.Scheme),    "GetLeftPart(UriPartial.Scheme)"),
            getUriPart(uri, "GetLeftPart(Authority)",   uri => uri.GetLeftPart(UriPartial.Authority), "GetLeftPart(UriPartial.Authority)"),
            getUriPart(uri, "GetLeftPart(Path)",        uri => uri.GetLeftPart(UriPartial.Path),      "GetLeftPart(UriPartial.Path)"),
            getUriPart(uri, "GetLeftPart(Query)",       uri => uri.GetLeftPart(UriPartial.Query),     "GetLeftPart(UriPartial.Query)"),
        };
    }

}
