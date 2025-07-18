using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// リザルトニューで使用するボタン
/// </summary>
public class MenuResult : MenuBase {

    // 現在の状態を保持
    private ResultState _state = ResultState.None;

    // タップ連打防止のフラグ（一定時間は無視）
    private bool _isTapLocked = false;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override async UniTask Initialize() {
        await base.Initialize();
        _state = ResultState.ShowImage;
        //await UniTask.CompletedTask;
    }

    /// <summary>
    /// メニューを開いたときの処理
    /// </summary>
    public override async UniTask Open(GameResultType type) {
        await base.Open();

        // フェードインで画面を表示
        await FadeManager.instance.FadeIn();

        // ステートが Finish になるまでループ
        while (true) {
            switch (_state) {
                case ResultState.ShowImage:
                    // リザルトテキスト表示
                    if (type == GameResultType.Clear) {
                        Debug.Log("クリアだてばよ");
                    }
                    else {
                        Debug.Log("ゲームオーバーだてばよ");

                    }
                    break;

                case ResultState.ShowRank:
                    // ランク・スコアなどのUI表示処理を呼び出し
                    ShowRankText();
                    break;

                case ResultState.Finish:
                    // 終了処理：フェードアウト後にメニューを閉じる
                    await FadeManager.instance.FadeOut();
                    await Close();
                    return;
            }

            // 0.1秒待機（ループ速度制御）
            await UniTask.Delay(100);
        }


    }

    /// <summary>
    /// メニュークローズ処理（状態リセット）
    /// </summary>
    public override async UniTask Close() {
        await base.Close();
        _state = ResultState.None;
        await UniTask.CompletedTask;
    }


    /// <summary>
    /// 外部から呼ばれるステート遷移関数（タップ時に呼ばれる）
    /// </summary>
    public async void NextState() {
        if (_isTapLocked) return; // 連続タップを制限する
        _isTapLocked = true;
        // タップに応じたリザルトの遷移
        switch (_state) {
            case ResultState.ShowImage:
                _state = ResultState.ShowRank;
                break;

            case ResultState.ShowRank:
                _state = ResultState.Finish;
                break;
        }

        // タップ可能になるまで待つ
        await UniTask.Delay(500);
        _isTapLocked = false;
    }

    /// <summary>
    /// ランク・スコアなどのUI表示処理（表示アニメーションなどもここに）
    /// </summary>
    private void ShowRankText() {
        Debug.Log("ランク表示処理表示");
    }
}
