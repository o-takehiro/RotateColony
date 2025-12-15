/*
 *  @file   MenuResult.cs
 *  @author oorui
 */

using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using static GameResultData;

/// <summary>
/// リザルトメニュー
/// </summary>
public class MenuResult : MenuBase {
    [SerializeField] private TextMeshProUGUI penetrationText = null;   // 突破数テキスト
    [SerializeField] private TextMeshProUGUI timeText = null;          // 時間テキスト
    [SerializeField] private TextMeshProUGUI modeText = null;          // モードテキスト
    [SerializeField] private TextMeshProUGUI rankText = null;          // ランクテキスト

    private const int _RESULT_SE_ID = 3;                               // SEのID
    private const int _RESULT_SE_ID02 = 4;                             // SEのID

    private const int _DELAY_TIME01 = 500;                             // 待機時間
    private const int _DELAY_TIME02 = 300;                             // 待機時間

    private const string _GAMEMODE_NORMAL = "Normal";                  // ゲームモードノーマル
    private const string _GAMEMODE_ENDLESS = "Endless";                // ゲームモードエンドレス


    /// <summary>
    /// リザルト時の状態
    /// </summary>
    private enum ResultState {
        None,
        Showing,
        Finished
    }

    private ResultState _state = ResultState.None;
    private bool _isTapLocked = false;
    private GameResultType _currentResult;
    RankCalculatorBase calculator = null;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
        ResetState();
    }

    /// <summary>
    /// メニューを開く
    /// </summary>
    /// <param name="resultType"></param>
    /// <returns></returns>
    public override async UniTask Open(GameResultType resultType) {
        await base.Open();

        // ゲーム終了時の状態
        _currentResult = resultType;
        _state = ResultState.Showing;
        _isTapLocked = true;

        // ランク計算用のクラス選択
        if (GameResultData.SelectedMode == GameModeState.Normal) {
            calculator = new StageClearRankCalculator();
        }
        else {
            calculator = new EndlessModeRankCalculator();
        }

        int passed = StagePassedCount;
        float time = ClearTime;

        // フェードイン
        await FadeManager.instance.FadeIn();

        // スコア表示演出
        await PlayScoreSequence(passed, time);

        // 演出完了後はタップ可能に
        _isTapLocked = false;
        _state = ResultState.Finished;

        // ループ待機
        while (_state == ResultState.Finished) {
            // 次のフレームまで待機
            await UniTask.Yield();
        }
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close() {
        await base.Close();
        ResetState();
        // テキストをすべてクリアにする
        AllTextClear();
    }

    /// <summary>
    /// タップで終了
    /// </summary>
    public async void NextState() {
        if (_isTapLocked) return;
        if (_state != ResultState.Finished) return;

        _isTapLocked = true;
        // フェードアウト
        await FadeManager.instance.FadeOut();
        await Close();
    }

    /// <summary>
    /// スコア演出
    /// </summary>
    private async UniTask PlayScoreSequence(int passed, float time) {
        UniTask task;
        // 最初は全て非表示にしておく
        AllSetActive(false);

        // 値を設定
        penetrationText.text = $"{passed}";
        timeText.text = FormatTime(time);
        // 選択されたモード別でテキストを表示
        modeText.text = GetModeString(GameResultData.SelectedMode);

        // 順番に表示演出
        await AnimateText(penetrationText);
        // SE再生
        task = SoundManager.instance.PlaySE(_RESULT_SE_ID);
        await UniTask.Delay(_DELAY_TIME02);
        await AnimateText(timeText);
        // SE再生
        task = SoundManager.instance.PlaySE(_RESULT_SE_ID);
        await UniTask.Delay(_DELAY_TIME02);
        await AnimateText(modeText);
        // SE再生
        task = SoundManager.instance.PlaySE(_RESULT_SE_ID);

        // 少し待ってランク表示
        await UniTask.Delay(_DELAY_TIME01);
        ShowRankText(passed, time);
        await AnimateText(rankText);
        // SE再生
        task = SoundManager.instance.PlaySE(_RESULT_SE_ID02);
    }

    /// <summary>
    /// テキストを拡大→元サイズに戻す演出
    /// </summary>
    private async UniTask AnimateText(TextMeshProUGUI text) {
        if (text == null) return;

        float duration = 0.2f;       // 縮小時間
        float scaleUp = 2f;          // 初期拡大倍率
        Vector3 baseScale = text.rectTransform.localScale;



        // 少し待ってから表示（フェードインのように）
        await UniTask.Yield();
        text.gameObject.SetActive(true);

        // 一瞬で大きく
        text.rectTransform.localScale = baseScale * scaleUp;

        // 元のサイズに戻す
        float t = 0f;
        while (t < duration) {
            t += Time.deltaTime;
            float rate = t / duration;
            text.rectTransform.localScale = Vector3.Lerp(baseScale * scaleUp, baseScale, rate);
            await UniTask.Yield();
        }

        text.rectTransform.localScale = baseScale;
    }

    /// <summary>
    /// 突破数と時間からランク文字列を出す
    /// </summary>
    /// <param name="passed"></param>
    /// <param name="time"></param>
    private void ShowRankText(int passed, float time) {
        string rank = calculator.CalculateRank(passed, time);
        rankText.text = rank;
    }

    /// <summary>
    /// 秒数変換
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private string FormatTime(float seconds) {
        // 分を求める
        int min = Mathf.FloorToInt(seconds / 60f);
        // 秒を求める
        int sec = Mathf.FloorToInt(seconds % 60f);
        // 2桁の表示にする
        return $"{min:D2}:{sec:D2}";
    }

    /// <summary>
    /// 選択されたモードによって表示するテキストを変える
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    private string GetModeString(GameModeState mode) {
        switch (mode) {
            // ノーマルモード
            case GameModeState.Normal: return _GAMEMODE_NORMAL;
            // エンドレスモード
            case GameModeState.Endless: return _GAMEMODE_ENDLESS;
            default: return null;
        }
    }

    /// <summary>
    /// すべて同時に表示非表示を切り替える
    /// </summary>
    /// <param name="setValue"></param>
    private void AllSetActive(bool setValue) {
        penetrationText.gameObject.SetActive(setValue);
        timeText.gameObject.SetActive(setValue);
        modeText.gameObject.SetActive(setValue);
        rankText.gameObject.SetActive(setValue);
    }

    /// <summary>
    /// すべてのテキストをクリアにする
    /// </summary>
    private void AllTextClear() {
        if (penetrationText != null) penetrationText.text = "";
        if (timeText != null) timeText.text = "";
        if (modeText != null) modeText.text = "";
        if (rankText != null) rankText.text = "";
    }

    /// <summary>
    /// 状態とフラグの初期化
    /// </summary>
    private void ResetState() {
        _state = ResultState.None;
        _isTapLocked = false;
        _currentResult = GameResultType.None;
    }
}
