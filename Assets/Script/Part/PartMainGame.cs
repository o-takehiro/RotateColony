using Cysharp.Threading.Tasks;
using UnityEngine;

public class PartMainGame : PartBase {
    private bool goalReached = false;

    // ゴール到達時に呼ばれるメソッド
    private void OnGoalReachedHandler() {
        Debug.Log("OnGoalReachedHandler() 呼び出された");
        goalReached = true;
        
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
        goalReached = false;          // フラグを必ず初期化
    }

    /// <summary>
    /// パートの準備
    /// </summary>
    /// <returns></returns>
    public override async UniTask SetUp() {
        goalReached = false;
        await base.SetUp();

        // プレイヤー生成
        PlayerManager.instance.UsePlayer(new Vector3(0f, -2f, 0f), Quaternion.Euler(0, 0, 0));
        await FadeManager.instance.FadeIn();

        GameObject playerObj = PlayerManager.instance.GetPlayerObject();

        var animController = playerObj.GetComponent<PlayerAnimationController>();
        if (animController != null) {
            animController.PlayTransformAnimation();
        }

        Transform playerF = playerObj.transform;
        if (playerObj != null) {
            StageManager.instance.SetPlayer(playerF);
        }

        // ゴールイベント
        StageManager.instance.OnGoalReached += OnGoalReachedHandler;
        
        // 安定するまで1秒待つ
        await UniTask.Delay(1000);

        // センサーの調整
        foreach (var segment in StageManager.instance.AllSegments) {
            segment.CalibrateGyro();
        }

        await UniTask.Delay(6000);  // クールタイム

        PlayerMove moveScript = playerObj.GetComponent<PlayerMove>();
        if (moveScript != null) {
            moveScript.StopedReset();
            moveScript.SetStartMoving(true);
            ButtonManager.instance.UseAllButtons();
        }

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 実行処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute() {
        GameObject playerObj = PlayerManager.instance.GetPlayerObject();
        PlayerMove moveScript = playerObj.GetComponent<PlayerMove>();
        if (moveScript == null || playerObj == null) return;

        while (true) {
            await UniTask.Yield();

            if (goalReached) {
                GameResultData.ResultType = GameResultType.Clear;
                GameResultData.StagePassedCount = StageManager.instance.PassedStageCount;

                await UniTask.Delay(2000);
                await FadeManager.instance.FadeOut();
                await PartManager.Instance.TransitionPart(eGamePart.Ending);
                break;
            }

            if (moveScript.GetIsStopped()) {
                GameResultData.ResultType = GameResultType.GameOver;
                GameResultData.StagePassedCount = StageManager.instance.PassedStageCount;

                await UniTask.Delay(3000);
                await FadeManager.instance.FadeOut();
                await PartManager.Instance.TransitionPart(eGamePart.Ending);
                break;
            }
        }
    }

    /// <summary>
    /// パートの片付け
    /// </summary>
    /// <returns></returns>
    public override async UniTask Teardown() {
        await base.Teardown();

        // イベントの解除
        if (StageManager.instance != null) {
            StageManager.instance.OnGoalReached -= OnGoalReachedHandler;
        }

        // それぞれのオブジェクトを破棄する
        PlayerManager.instance.DestroyPlayer();
        ButtonManager.instance.DestroyAllButtons();
        StageManager.instance.ClearAllSegments();

        await UniTask.CompletedTask;
    }
}