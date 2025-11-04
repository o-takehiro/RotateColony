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

    private const int _RESULT_SE_ID = 0;                               // SEのID

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

    public override async UniTask Initialize() {
        await base.Initialize();
        _state = ResultState.None;
        _isTapLocked = false;
        _currentResult = GameResultType.None;
    }

    public override async UniTask Open(GameResultType resultType) {
        await base.Open();

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

        await FadeManager.instance.FadeIn();

        // スコア表示演出
        await PlayScoreSequence(passed, time);

        // 演出完了後はタップ可能に
        _isTapLocked = false;
        _state = ResultState.Finished;

        // ループ待機（タップで閉じる）
        while (_state == ResultState.Finished) {
            await UniTask.Yield();
        }
    }

    public override async UniTask Close() {
        await base.Close();
        _state = ResultState.None;
        _isTapLocked = false;
        _currentResult = GameResultType.None;

        if (penetrationText != null) penetrationText.text = "";
        if (timeText != null) timeText.text = "";
        if (modeText != null) modeText.text = "";
        if (rankText != null) rankText.text = "";
    }

    /// <summary>
    /// タップで終了
    /// </summary>
    public async void NextState() {
        if (_isTapLocked) return;
        if (_state != ResultState.Finished) return;

        _isTapLocked = true;
        await FadeManager.instance.FadeOut();
        await Close();
    }

    /// <summary>
    /// スコア演出
    /// </summary>
    private async UniTask PlayScoreSequence(int passed, float time) {
        // 最初は全て非表示にしておく
        penetrationText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        modeText.gameObject.SetActive(false);
        rankText.gameObject.SetActive(false);

        // 値を設定
        penetrationText.text = $"{passed}";
        timeText.text = FormatTime(time);
        modeText.text = GetModeString(GameResultData.SelectedMode);

        // 順番に表示演出
        await AnimateText(penetrationText);
        // SE再生
        await SoundManager.instance.PlaySE(3);
        await UniTask.Delay(300);
        await AnimateText(timeText);
        // SE再生
        await SoundManager.instance.PlaySE(3);
        await UniTask.Delay(300);
        await AnimateText(modeText);
        // SE再生
        await SoundManager.instance.PlaySE(3);

        // 少し待ってランク表示
        await UniTask.Delay(500);
        ShowRankText(passed, time);
        await AnimateText(rankText);
        // SE再生
        await SoundManager.instance.PlaySE(4);
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

    private void ShowRankText(int passed, float time) {
        string rank = calculator.CalculateRank(passed, time);
        rankText.text = rank;
    }

    private string FormatTime(float seconds) {
        int min = Mathf.FloorToInt(seconds / 60f);
        int sec = Mathf.FloorToInt(seconds % 60f);
        return $"{min:D2}:{sec:D2}";
    }

    private string GetModeString(GameModeState mode) {
        switch (mode) {
            case GameModeState.Normal: return "Normal";
            case GameModeState.Endless: return "Endless";
            default: return "不明";
        }
    }
}
