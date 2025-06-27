using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

using static PlayerManager;
using static ButtonManager;
/// <summary>
/// メインゲームパート
/// </summary>
public class PartMainGame : PartBase {

    public override async UniTask Initialize() {
        await base.Initialize();

    }

    public override async UniTask SetUp() {
        await base.SetUp();

        // プレイヤー生成
        PlayerManager.instance.UsePlayer(new Vector3(0f, -2f, 0f), Quaternion.Euler(0, 0, 0));
        // フェードイン
        await FadeManager.instance.FadeIn();

        // ステージ生成
        // プレイヤー取得
        GameObject playerObj = PlayerManager.instance.GetPlayerObject();

        // アニメーションを再生
        var animController = playerObj.GetComponent<PlayerAnimationController>();
        if (animController != null) {
            animController.PlayTransformAnimation();
        }

        // GameObject型からTransform型に変換        
        Transform playerF = playerObj.transform;
        if (playerObj != null) {
            // Stageにプレイヤーの情報を渡す
            StageManager.instance.SetPlayer(playerF);
        }

        UniTask task = StageManager.instance.Initialize();

        // 3秒クールタイムを待つ
        await UniTask.Delay(6000);  // ミリ秒で指定
        // ボタン生成

        // プレイヤーのPlayerMoveスクリプトを取得し、移動開始を許可
        PlayerMove moveScript = playerObj.GetComponent<PlayerMove>();
        if (moveScript != null) {
            moveScript.StopedReset();
            moveScript.SetStartMoving(true);
            ButtonManager.instance.UseAllButtons();
        }

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// エンディングパートへの遷移
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute() {
        GameObject playerObj = PlayerManager.instance.GetPlayerObject();
        PlayerMove moveScript = playerObj.GetComponent<PlayerMove>();
        if (moveScript == null || playerObj == null) return;

        // フラグ監視ループ
        while (true) {
            // 毎フレームチェック
            await UniTask.Yield(); // 次のフレームを待つ
            
            if (moveScript.GetIsStopped()) {
                // エンディングパートへ遷移
                await UniTask.Delay(3000);
                await FadeManager.instance.FadeOut();
                await PartManager.Instance.TransitionPart(eGamePart.Ending);
                break;
            }
        }
    }

    /// <summary>
    /// パート遷移時の片付け
    /// </summary>
    /// <returns></returns>
    public override async UniTask Teardown() {
        await base.Teardown();
        // プレイヤーを削除
        PlayerManager.instance.DestroyPlayer();
        ButtonManager.instance.DestroyAllButtons();
        // ステージのPrefabを全部削除
        StageManager.instance.ClearAllSegments();

        await UniTask.CompletedTask;
    }

}
