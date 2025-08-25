using Cysharp.Threading.Tasks;       // 非同期処理に使うUniTaskを利用するためのライブラリ
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

using static SoundManager;           // SoundManagerのメソッドや変数を直接呼び出すための設定

/// <summary>
/// PartTitleからMenuTitle、MenuModeを経由してPartTitleに戻る流れを管理するクラス
/// </summary>
public class MenuMode : MenuBase {
    // ノーマルモードが選ばれたかどうかを管理するフラグ
    public bool _isNormal = false;
    // エンドレスモードが選ばれたかどうかを管理するフラグ
    public bool _isEndless = false;

    // 操作説明の画像やパネルを参照するための変数
    [SerializeField] private GameObject _howToPlayPanel;
    // 画像やパネルの大きさを制御するためにRectTransformを保持する変数
    private RectTransform _howToPlayRect;
    // 操作説明が表示されているかどうかを覚えておくための変数
    private bool _isHowToPlayVisible = false;
    // 表示や非表示のアニメーションを制御するためのコルーチンを保持する変数
    private Coroutine _animCoroutine;

    // 操作説明を表示する時の最終的な大きさを指定できる変数
    [SerializeField] private Vector3 _targetScale = Vector3.one;

    // メニューで使う効果音のID
    private const int _MENU_SE_ID = 0;

    // 初期化処理を行うメソッド
    public override async UniTask Initialize() {
        // 親クラスの初期化処理を実行する
        await base.Initialize();

        // 操作説明パネルが存在している場合、そのRectTransformを取得して非表示にする
        if (_howToPlayPanel != null) {
            _howToPlayRect = _howToPlayPanel.GetComponent<RectTransform>();
            _howToPlayPanel.SetActive(false);
        }
    }

    // メニューを開くときに呼ばれる処理
    public override async UniTask Open() {
        // 親クラスのOpen処理を実行する
        await base.Open();
        // 画面をフェードインする
        await FadeManager.instance.FadeIn();

        // モード選択を待つ処理。ノーマルかエンドレスが選ばれるまでループする
        while (true) {
            // エンドレスモードが選ばれた場合
            if (_isEndless) {
                // 効果音を再生する
                await instance.PlaySE(_MENU_SE_ID);
                // ステージマネージャにエンドレスモードを設定する
                StageManager.instance.SetupStrategy(GameModeState.Endless);
                break;
            }
            // ノーマルモードが選ばれた場合
            else if (_isNormal) {
                // 効果音を再生する
                await instance.PlaySE(_MENU_SE_ID);
                // ステージマネージャにノーマルモードを設定する
                StageManager.instance.SetupStrategy(GameModeState.Normal);
                break;
            }

            // 何も選ばれていない場合は一フレーム待機してループを続ける
            await UniTask.Delay(1);
        }

        // モードが決まったらフェードアウトを実行する
        await FadeManager.instance.FadeOut();
        // メニューを閉じる処理を呼ぶ
        await Close();
    }

    // メニューを閉じるときの処理
    public override async UniTask Close() {
        // 親クラスのClose処理を実行する
        await base.Close();

        // モード選択の状態をリセットする
        _isNormal = false;
        _isEndless = false;

        // 操作説明パネルが存在していれば非表示にする
        if (_howToPlayPanel != null) {
            _howToPlayPanel.SetActive(false);
        }

        // 非同期処理を完了させる
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 操作説明の表示と非表示を切り替える処理
    /// </summary>
    public void ToggleHowToPlay() {
        // 操作説明パネルが存在しない場合は何もしない
        if (_howToPlayPanel == null) return;

        // すでにアニメーションが再生中なら止める
        if (_animCoroutine != null) {
            StopCoroutine(_animCoroutine);
        }

        // 表示状態を反転させる
        _isHowToPlayVisible = !_isHowToPlayVisible;

        // 表示する場合
        if (_isHowToPlayVisible) {
            // パネルをアクティブにして表示する
            _howToPlayPanel.SetActive(true);
            // スケールをゼロから指定した大きさまで拡大するアニメーションを開始する
            _animCoroutine = StartCoroutine(AnimateScale(Vector3.zero, _targetScale, 0.3f));
        }
        // 非表示にする場合
        else {
            // 現在のスケールからゼロまで縮小して、完了したら非表示にする
            _animCoroutine = StartCoroutine(AnimateScale(_howToPlayRect.localScale, Vector3.zero, 0.2f, () => {
                _howToPlayPanel.SetActive(false);
            }));
        }
    }

    /// <summary>
    /// 指定された大きさから別の大きさへ、一定時間をかけて補間しながらアニメーションさせる処理
    /// </summary>
    private IEnumerator AnimateScale(Vector3 from, Vector3 to, float duration, System.Action onComplete = null) {
        // 経過時間を管理する変数
        float time = 0f;
        // スタート時のスケールを設定する
        _howToPlayRect.localScale = from;

        // 経過時間が指定の時間に達するまでループする
        while (time < duration) {
            // 経過時間を増加させる
            time += Time.deltaTime;
            // 0から1の範囲に正規化する
            float t = Mathf.Clamp01(time / duration);
            // スケールを線形補間して滑らかに変化させる
            _howToPlayRect.localScale = Vector3.Lerp(from, to, t);
            // 一フレーム待機する
            yield return null;
        }

        // 最終的に目的のスケールに確定させる
        _howToPlayRect.localScale = to;
        // アニメーション完了時に追加で処理が必要なら呼び出す
        onComplete?.Invoke();
        // アニメーション中のコルーチンをリセットする
        _animCoroutine = null;
    }
}