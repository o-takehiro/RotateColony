using Cysharp.Threading.Tasks;
using UnityEngine;

public class PartEnding : PartBase {

    public override async UniTask Initialize() {
        await base.Initialize();
        // ƒƒjƒ…[‚Ì‰Šú‰»
        await MenuManager.instance.Get<MenuResult>("Prefab/Menu/CanvasResult").Initialize();
    }

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

    public override async UniTask Execute() {
        await FadeManager.instance.FadeOut();
        await PartManager.Instance.TransitionPart(eGamePart.Title);
    }

    public override async UniTask Teardown() {
        await base.Teardown();
    }
}