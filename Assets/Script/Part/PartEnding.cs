using Cysharp.Threading.Tasks;
using UnityEngine;

public class PartEnding : PartBase {

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
        // メニューの初期化
        await MenuManager.instance.Get<MenuResult>("Prefab/Menu/CanvasResult").Initialize();
    }
    /// <summary>
    /// 準備
    /// </summary>
    /// <returns></returns>
    public override async UniTask SetUp() {
        await base.SetUp();

        var menuResult = MenuManager.instance.Get<MenuResult>();

        switch (GameResultData.ResultType) {
            case GameResultType.Clear:
                await menuResult.Open(GameResultType.Clear);
                break;
            case GameResultType.GameOver:
                await menuResult.Open(GameResultType.GameOver);
                break;
        }
    }
    /// <summary>
    /// 実行
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute() {
        await FadeManager.instance.FadeOut();
        // タイトル画面を事前にロードしておく
        var menuTitle = MenuManager.instance.Get<MenuTitle>("Prefab/Menu/CanvasTitle");
        await menuTitle.Initialize();
        await PartManager.Instance.TransitionPart(eGamePart.Title);
    }

    /// <summary>
    /// 片付け
    /// </summary>
    /// <returns></returns>
    public override async UniTask Teardown() {
        await base.Teardown();
    }
}