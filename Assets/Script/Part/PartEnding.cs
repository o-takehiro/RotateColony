using Cysharp.Threading.Tasks;
using UnityEngine;

public class PartEnding : PartBase {

    public override async UniTask Initialize() {
        await base.Initialize();
    }

    public override async UniTask SetUp() {
        await base.SetUp();

        // ゲーム結果に応じたログ出力
        switch (GameResultData.ResultType) {
            case GameResultType.Clear:
                Debug.Log("ゲームクリア");


                break;
            case GameResultType.GameOver:
                Debug.Log("ゲームオーバー");


                break;
        }

        Debug.Log("通過したステージ数: " + GameResultData.StagePassedCount);
    }

    public override async UniTask Execute() {
        // 演出などを挟む余地あり
        await UniTask.Delay(3000);
        await FadeManager.instance.FadeOut();
        await PartManager.Instance.TransitionPart(eGamePart.Title);
    }

    public override async UniTask Teardown() {
        await base.Teardown();
    }
}