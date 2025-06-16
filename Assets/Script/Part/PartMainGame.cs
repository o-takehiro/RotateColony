using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlayerManager;
/// <summary>
/// メインゲームパート
/// </summary>
public class PartMainGame : PartBase {

    public override async UniTask Initialize() {
        await base.Initialize();

    }

    public override async UniTask SetUp() {
        await base.SetUp();
        // フェードイン

        // プレイヤーとステージ生成
        instance.UsePlayer(Vector3.zero, Quaternion.Euler(0, 0, 0));
        await FadeManager.instance.FadeIn();

        // ステージ生成
        // プレイヤー取得
        GameObject playerObj = instance.GetPlayerObject();
        // GameObject型からTransform型に変換        
        Transform playerF = playerObj.transform;
        if (playerObj != null) {
            // Stageにプレイヤーの情報を渡す
            StageManager.instance.SetPlayer(playerF);
        }

        UniTask task = StageManager.instance.Initialize();

        // 3秒クールタイムを待つ
        await UniTask.Delay(6000);  // ミリ秒で指定

        // プレイヤーのPlayerMoveスクリプトを取得し、移動開始を許可
        PlayerMove moveScript = playerObj.GetComponent<PlayerMove>();
        if (moveScript != null) {
            moveScript.StopedReset();
            moveScript.SetStartMoving(true);
        }

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// エンディングパートへの遷移
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute() {
        GameObject playerObj = instance.GetPlayerObject();
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
        instance.DestroyPlayer();
        // ステージのPrefabを全部削除
        StageManager.instance.ClearAllSegments();

        await UniTask.CompletedTask;
    }

}
