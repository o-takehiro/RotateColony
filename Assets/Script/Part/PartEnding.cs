using Cysharp.Threading.Tasks;
using UnityEngine;

public class PartEnding : PartBase {

    /// <summary>
    /// パートの初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
    }

    /// <summary>
    /// パートの準備
    /// </summary>
    /// <returns></returns>
    public override async UniTask SetUp() {
        await base.SetUp();

        // ゲーム結果に応じたログ出力
        switch (GameResultData.ResultType) {
            case GameResultType.Clear:
                Debug.Log("ゲームクリア！");
                // ゲームクリア時の処理をのちに追加
                break;
            case GameResultType.GameOver:
                Debug.Log("ゲームオーバー...");
                // ゲームオーバー時の処理をのちに追加
                break;
        }
        // 通過したステージ数を表示：のちにUIとして実装
        Debug.Log("通過したステージ数: " + GameResultData.StagePassedCount);
    }

    /// <summary>
    /// 遷移処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute() {
        // 演出などを挟む
        await UniTask.Delay(3000);
        await FadeManager.instance.FadeOut();
        await PartManager.Instance.TransitionPart(eGamePart.Title);
    }


    /// <summary>
    /// パートの型付け
    /// </summary>
    /// <returns></returns>
    public override async UniTask Teardown() {
        await base.Teardown();
    }
}