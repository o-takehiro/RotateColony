using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// フェード管理
/// </summary>
public class FadeManager : SystemObject {

    // フェード用黒画像
    [SerializeField]
    private Image _fadeImage = null;

    public static FadeManager instance { get; private set; } = null;

    // デフォルトのフェード時間
    private const float _DEFAULT_FADE_DURAITION = 0.1f;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        instance = this;
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// フェードアウト、暗くする
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask FadeOut(float duration = _DEFAULT_FADE_DURAITION) {
        await FadeTargetAlpha(1.0f, duration);
    }

    /// <summary>
    /// フェードイン、明るくする
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask FadeIn(float duration = _DEFAULT_FADE_DURAITION) {
        await FadeTargetAlpha(0.0f, duration);
    }

    /// <summary>
    /// フェード画像を指定の不透明度に変化させる
    /// </summary>
    /// <param name="targetAlpha"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private async UniTask FadeTargetAlpha(float targetAlpha, float duration) {
        _fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0.0f;   // 経過時間
        float startAlpha = _fadeImage.color.a;
        Color targetColor = _fadeImage.color;
        while (elapsedTime < duration) {
            // フレーム経過時間
            elapsedTime += Time.deltaTime;
            // 補間した不透明度をフェード画像に設定
            float t = elapsedTime / duration;
            targetColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            _fadeImage.color = targetColor;
            // 1フレーム待ち1
            await UniTask.DelayFrame(3);
        }
        targetColor.a = targetAlpha;
        _fadeImage.color = targetColor;
        _fadeImage.gameObject.SetActive(false);
    }


}
