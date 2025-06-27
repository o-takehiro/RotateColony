using Cysharp.Threading.Tasks;
using UnityEngine;

public class PartEnding : PartBase {

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
    }

    public override async UniTask SetUp() {
        await base.SetUp();

        // フェードイン
        await FadeManager.instance.FadeIn();
        // 通過したステージ数を見る
        int stageCount = StageManager.instance.PassedStageCount;
        // プレイヤー取得
        GameObject playerObj = PlayerManager.instance.GetPlayerObject();
        PlayerMove moveScript = playerObj.GetComponent<PlayerMove>();

        if (moveScript.GetIsGameClear()) {
            // クリア



        }
        else {
            // ゲームオーバー



        }




    }

    public override async UniTask Execute() {
        UniTask task = PartManager.Instance.TransitionPart(eGamePart.Title);
        await UniTask.CompletedTask;
    }
}
