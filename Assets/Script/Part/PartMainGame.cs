using Cysharp.Threading.Tasks;
using UnityEngine;

public class PartMainGame : PartBase {
    private bool goalReached = false;

    // ゴール到達時に呼ばれるメソッド
    private void OnGoalReachedHandler() {
        goalReached = true;
    }

    public override async UniTask Initialize() {
        await base.Initialize();
    }

    public override async UniTask SetUp() {
        await base.SetUp();

        // プレイヤー生成
        PlayerManager.instance.UsePlayer(new Vector3(0f, -2f, 0f), Quaternion.Euler(0, 0, 0));
        await FadeManager.instance.FadeIn();

        GameObject playerObj = PlayerManager.instance.GetPlayerObject();

        // アニメーション再生
        var animController = playerObj.GetComponent<PlayerAnimationController>();
        if (animController != null) {
            animController.PlayTransformAnimation();
        }

        Transform playerF = playerObj.transform;
        if (playerObj != null) {
            StageManager.instance.SetPlayer(playerF);
        }

        // ゴールイベント登録
        StageManager.instance.OnGoalReached += OnGoalReachedHandler;

        await UniTask.Delay(6000);  // クールタイム

        PlayerMove moveScript = playerObj.GetComponent<PlayerMove>();
        if (moveScript != null) {
            moveScript.StopedReset();
            moveScript.SetStartMoving(true);
            ButtonManager.instance.UseAllButtons();
        }

        await UniTask.CompletedTask;
    }

    public override async UniTask Execute() {
        GameObject playerObj = PlayerManager.instance.GetPlayerObject();
        PlayerMove moveScript = playerObj.GetComponent<PlayerMove>();
        if (moveScript == null || playerObj == null) return;

        while (true) {
            await UniTask.Yield();

            // クリア時の遷移処理
            if (goalReached) {
                GameResultData.ResultType = GameResultType.Clear;
                GameResultData.StagePassedCount = StageManager.instance.PassedStageCount;

                await UniTask.Delay(2000);
                await FadeManager.instance.FadeOut();
                await PartManager.Instance.TransitionPart(eGamePart.Ending);
                break;

            }
            // ゲームオーバー時の遷移処理
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

    public override async UniTask Teardown() {
        await base.Teardown();

        // イベントの解除
        if (StageManager.instance != null) {
            StageManager.instance.OnGoalReached -= OnGoalReachedHandler;
        }

        PlayerManager.instance.DestroyPlayer();
        ButtonManager.instance.DestroyAllButtons();
        StageManager.instance.ClearAllSegments();

        await UniTask.CompletedTask;
    }
}