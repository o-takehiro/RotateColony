/*
 *  @file   PartMainGame.cs
 *  @author oorui
 */

using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// メインゲームシーン制御
/// </summary>
public class PartMainGame : PartBase {
    private bool goalReached = false;                 // ゴールに到達したかどうかを示すフラグ]

    private const int _MAIN_BGM_ID = 0;               // 使用するBGMのID
    private const int _MAIN_SE_ID_7 = 7;              // 使用するSEのID

    private const int _COUNTDOWN_TIME = 3;            // カウントダウンの秒数
    /// <summary>
    /// ゴールに到達した際に呼び出されるイベント
    /// </summary>
    private void OnGoalReachedHandler() {
        goalReached = true; // ゴール到達フラグを立てる
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override async UniTask Initialize() {
        await base.Initialize();
        goalReached = false; // フラグ初期化
    }

    /// <summary>
    /// セットアップ処理
    /// </summary>
    public override async UniTask SetUp() {
        // フラグ初期化
        goalReached = false;
        await base.SetUp();

        // BGM再生
        SoundManager.instance.PlayBGM(_MAIN_BGM_ID);

        // プレイヤーキャラクターを初期位置に生成
        PlayerManager.instance.UsePlayer(new Vector3(0f, -2f, 0f), Quaternion.Euler(0, 0, 0));

        // 画面フェードイン演出
        await FadeManager.instance.FadeIn();

        // プレイヤーのゲームオブジェクト取得
        GameObject playerObj = PlayerManager.instance.GetPlayerObject();

        // アニメーション再生
        var animController = playerObj.GetComponent<PlayerAnimationController>();
        if (animController != null) {
            animController.PlayTransformAnimation();
        }

        // プレイヤーTransformをステージマネージャーに登録
        Transform playerF = playerObj.transform;
        if (playerObj != null) {
            StageManager.instance.SetPlayer(playerF);
        }

        // ゴールイベント登録（ゴールに到達したときの処理を追加）
        StageManager.instance.OnGoalReached += OnGoalReachedHandler;

        // 適当に待つ
        await UniTask.Delay(1000);

        // 各ステージの要素のジャイロキャリブレーション実行
        foreach (var segment in StageManager.instance.AllSegments) {
            segment.CalibrateGyro();
        }

        // カウントダウンを実行
        CountdownManager countdownManager = FindObjectOfType<CountdownManager>();
        if (countdownManager != null) {
            // SE再生
            await SoundManager.instance.PlaySE(_MAIN_SE_ID_7);
            await countdownManager.StartCountdown(_COUNTDOWN_TIME);
        }
        else {
            await UniTask.Delay(6000);
        }

        // プレイヤーの移動スクリプト取得
        PlayerMove moveScript = playerObj.GetComponent<PlayerMove>();
        if (moveScript != null) {
            moveScript.StopedReset();                // 停止状態のリセット
            moveScript.SetStartMoving(true);         // 移動開始
            ButtonManager.instance.UseAllButtons();  // 全ボタン使用可能に
        }

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 実行処理（プレイ中のループ。ゴール or ゲームオーバーまで待機）
    /// </summary>
    public override async UniTask Execute() {
        GameObject playerObj = PlayerManager.instance.GetPlayerObject();
        PlayerMove moveScript = playerObj.GetComponent<PlayerMove>();
        if (moveScript == null || playerObj == null) return;

        while (true) {
            // 非同期のループを行う
            await UniTask.Yield();

            // ゴール到達処理
            if (goalReached) {
                if (playerObj != null) {
                    // クリア時間を設定
                    moveScript?.ClearPlayer();
                }

                // クリア情報を記録
                GameResultData.ResultType = GameResultType.Clear;
                GameResultData.StagePassedCount = StageManager.instance.PassedStageCount;
                GameResultData.SelectedMode = StageManager.instance.CurrentMode;

                // 適当に待つ
                await UniTask.Delay(2000);
                // フェードアウト
                await FadeManager.instance.FadeOut();
                // エンディングパートに遷移
                await PartManager.Instance.TransitionPart(eGamePart.Ending);
                break;
            }

            // ゲームオーバーだった場合
            if (moveScript.GetIsStopped()) {
                GameResultData.ResultType = GameResultType.GameOver;
                GameResultData.StagePassedCount = StageManager.instance.PassedStageCount;
                GameResultData.SelectedMode = StageManager.instance.CurrentMode;
                // 適当に待つ
                await UniTask.Delay(2000);
                // フェードアウト:しろ
                await FadeManager.instance.FadeOut(FadeManager.FadeColor.White);
                // エンディングパートに遷移
                await PartManager.Instance.TransitionPart(eGamePart.Ending);
                break;
            }
        }
    }

    /// <summary>
    /// パート終了時の後片付け
    /// </summary>
    public override async UniTask Teardown() {
        await base.Teardown();
        // BGMの再生停止
        SoundManager.instance.StopBGM();

        // ゴールイベントの登録解除
        if (StageManager.instance != null) {
            StageManager.instance.OnGoalReached -= OnGoalReachedHandler;
        }

        // 使用していたオブジェクトの破棄処理
        PlayerManager.instance.DestroyPlayer();       // プレイヤーオブジェクト削除
        ButtonManager.instance.DestroyAllButtons();   // ボタンの削除
        StageManager.instance.ClearAllSegments();     // ステージ要素クリア
        EffectManager.Instance.StopAll();             // 全エフェクト停止
        CameraManager camera = Camera.main?.GetComponent<CameraManager>();
        camera.ExitCamera();
        await UniTask.CompletedTask;
    }
}