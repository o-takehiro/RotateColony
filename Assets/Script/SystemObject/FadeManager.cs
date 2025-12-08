/*
 *  @file   FadeManager
 *  @author oorui
 */
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// フェード管理
/// </summary>
public class FadeManager : SystemObject {
    // フェード用黒画像
    [SerializeField]
    private Image _fadeImage_Black = null;

    // フェード用白画像
    [SerializeField]
    private Image _fadeImage_White = null;

    // フェードに使用する色
    public enum FadeColor {
        Black,  // 黒
        White   // 白
    }

    // 自身への参照
    public static FadeManager instance { get; private set; } = null;

    // デフォルトのフェード時間
    private const float _DEFAULT_FADE_DURAITION = 1.0f;

    public override async UniTask Initialize() {
        instance = this;
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// フェードアウト開始
    /// </summary>
    public async UniTask FadeOut(FadeColor color = FadeColor.Black, float duration = _DEFAULT_FADE_DURAITION) {
        ResetOtherFadeImage(color);   // 使用しない画像を無効化
        await FadeTargetAlpha(GetFadeImage(color), 1.0f, duration);
    }

    /// <summary>
    /// フェードイン開始
    /// </summary>
    public async UniTask FadeIn(FadeColor color = FadeColor.Black, float duration = _DEFAULT_FADE_DURAITION) {
        ResetOtherFadeImage(color);   // 使用しない画像を無効化
        await FadeTargetAlpha(GetFadeImage(color), 0.0f, duration);
    }

    /// <summary>
    /// フェード画像取得
    /// </summary>
    private Image GetFadeImage(FadeColor color) {
        return color == FadeColor.Black ? _fadeImage_Black : _fadeImage_White;
    }

    /// <summary>
    /// 使用しないフェード画像の状態をリセット（非表示＆透明にする）
    /// </summary>
    private void ResetOtherFadeImage(FadeColor activeColor) {
        Image inactive = activeColor == FadeColor.Black ? _fadeImage_White : _fadeImage_Black;
        Color c = inactive.color;
        c.a = 0f;                    // 完全透明に変更
        inactive.color = c;
        inactive.gameObject.SetActive(false); // 非表示
    }

    /// <summary>
    /// フェード画像を指定の不透明度に変化させる
    /// </summary>
    private async UniTask FadeTargetAlpha(Image fadeImage, float targetAlpha, float duration) {
        fadeImage.gameObject.SetActive(true); // 使用する画像を表示する

        float elapsedTime = 0.0f; // 経過時間
        float startAlpha = fadeImage.color.a; // 現在の不透明度
        Color color = fadeImage.color; // 作業用

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // 時間経過割合

            color.a = Mathf.Lerp(startAlpha, targetAlpha, t); // 補間した不透明度
            fadeImage.color = color;

            await UniTask.Yield(); // 次のフレーム待機
        }

        // 最終値の適用
        color.a = targetAlpha;
        fadeImage.color = color;

        if (Mathf.Approximately(targetAlpha, 0f)) {
            fadeImage.gameObject.SetActive(false); // 完全透明なら非表示
        }
    }
}
