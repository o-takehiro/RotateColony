/*
 *  @file   MenuMode.cs
 *  @author oorui
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

using static SoundManager;

/// <summary>
/// メモード選択画面
/// </summary>
public class MenuMode : MenuBase {
    public bool _isNormal = false;                              // ノーマルモード選択フラグ
    public bool _isEndless = false;                             // エンドレスモード選択フラグ

    [SerializeField] private GameObject _howToPlayPanel;        // 操作説明UI
    private RectTransform _howToPlayRect;                       // スケール制御用
    private bool _isHowToPlayVisible = false;                   // 表示状態
    private Coroutine _animCoroutine;                           // アニメ管理


    [SerializeField] private Vector3 _targetScale = Vector3.one; // 表示サイズ
    private const int _MENU_SE_ID = 0; // 効果音ID

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
        if (_howToPlayPanel != null) {
            _howToPlayRect = _howToPlayPanel.GetComponent<RectTransform>();
            _howToPlayPanel.SetActive(false);
        }
    }
    /// <summary>
    /// 準備
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open() {
        await base.Open();
        await FadeManager.instance.FadeIn();

        // モードが選ばれるまで待機
        while (true) {
            // エンドレスモードに遷移
            if (_isEndless) {
                // SE再生
                await instance.PlaySE(_MENU_SE_ID);
                // エンドレスモードに遷移
                StageManager.instance.SetupStrategy(GameModeState.Endless);
                break;
            }
            // ノーマルモードに遷移
            else if (_isNormal) {
                // SE再生
                await instance.PlaySE(_MENU_SE_ID);
                // ノーマルモードに遷移
                StageManager.instance.SetupStrategy(GameModeState.Normal);
                break;
            }
            // 待つ
            await UniTask.Delay(1);
        }

        // フェードアウトを行う
        await FadeManager.instance.FadeOut();
        // メニューを閉じる
        await Close();
    }
    /// <summary>
    /// メニューを閉じる
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close() {
        await base.Close();
        _isNormal = false;
        _isEndless = false;
        if (_howToPlayPanel != null) _howToPlayPanel.SetActive(false);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 操作説明の表示切替
    /// </summary>
    public void ToggleHowToPlay() {
        if (_howToPlayPanel == null) return;
        if (_animCoroutine != null) StopCoroutine(_animCoroutine);

        _isHowToPlayVisible = !_isHowToPlayVisible;

        // 説明描画
        if (_isHowToPlayVisible) {
            _howToPlayPanel.SetActive(true);
            // アニメーションで画像を表示する
            _animCoroutine = StartCoroutine(AnimateScale(Vector3.zero, _targetScale, 0.3f));
        }
        else {
            _animCoroutine = StartCoroutine(AnimateScale(_howToPlayRect.localScale, Vector3.zero, 0.2f, () => {
                _howToPlayPanel.SetActive(false);
            }));
        }
    }

    /// <summary>
    /// スケールを補間してアニメーション
    /// </summary>
    private IEnumerator AnimateScale(Vector3 from, Vector3 to, float duration, System.Action onComplete = null) {
        float time = 0f;
        // アニメーション開始時のスケール
        _howToPlayRect.localScale = from;

        while (time < duration) {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            // 線形補間で目標位置まで補間
            _howToPlayRect.localScale = Vector3.Lerp(from, to, t);
            // 次のフレーム
            yield return null;
        }
        // スケールを固定k
        _howToPlayRect.localScale = to;

        onComplete?.Invoke();
        _animCoroutine = null;
    }
}
