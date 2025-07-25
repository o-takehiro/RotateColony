using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

using static GameResultData;

public class MenuResult : MenuBase {
    [SerializeField] private TextMeshProUGUI penetrationText = null;   // 突破数テキスト
    [SerializeField] private TextMeshProUGUI timeText = null;          // 時間テキスト
    // [SerializeField] private TextMeshProUGUI getText = null;           // 取得アイテムテキスト
    [SerializeField] private TextMeshProUGUI rankText = null;          // ランクテキスト
    // 使用するSEのID
    private const int _RESULT_SE_ID = 0;

    // 内部状態管理
    private enum ResultState {
        None,
        ShowImage,
        ShowRank,
        Finish
    }

    private ResultState _state = ResultState.None;
    private bool _isTapLocked = false;

    private GameResultType _currentResult;

    RankCalculatorBase calculator = null;

    /// <summary>
    /// 初期化処理（毎回呼ぶ想定）
    /// </summary>
    public override async UniTask Initialize() {
        await base.Initialize();
        _state = ResultState.None;
        _isTapLocked = false;
        _currentResult = GameResultType.None;
    }

    /// <summary>
    /// メニューを開いたときの処理（結果タイプを受け取る）
    /// </summary>
    public override async UniTask Open(GameResultType resultType) {
        await base.Open();

        _currentResult = resultType;
        _state = ResultState.ShowImage;
        _isTapLocked = false;

        calculator = new StageClearRankCalculator();

        int passed = StagePassedCount;
        float time = ClearTime;

        await FadeManager.instance.FadeIn();

        while (true) {
            switch (_state) {
                case ResultState.ShowImage:
                    penetrationText.text = $"{passed}";
                    timeText.text = FormatTime(time);
                    if (_currentResult == GameResultType.Clear) {
                        Debug.Log("ゲームクリア表示");
                    }
                    else {
                        Debug.Log("ゲームオーバー表示");
                    }
                    break;

                case ResultState.ShowRank:
                    // ランクを表示
                    ShowRankText(passed, time);
                    break;

                case ResultState.Finish:
                    await FadeManager.instance.FadeOut();
                    await Close();
                    return;
            }

            await UniTask.Delay(100);
        }
    }

    /// <summary>
    /// メニュー閉じるときのリセット処理
    /// </summary>
    public override async UniTask Close() {
        await base.Close();
        _state = ResultState.None;
        _isTapLocked = false;
        _currentResult = GameResultType.None;
    }

    /// <summary>
    /// 外部から呼ばれるタップ入力により状態を進める関数
    /// </summary>
    public async void NextState() {
        if (_isTapLocked) return;

        _isTapLocked = true;

        switch (_state) {
            case ResultState.ShowImage:
                _state = ResultState.ShowRank;
                break;

            case ResultState.ShowRank:
                _state = ResultState.Finish;
                break;
        }

        await UniTask.Delay(500);
        _isTapLocked = false;
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
}