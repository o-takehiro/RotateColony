using Cysharp.Threading.Tasks;

/// <summary>
/// タイトルパート
/// </summary>
public class PartTitle : PartBase {
    public override async UniTask Initialize() {
        await base.Initialize();
        // メニューの初期化
        await MenuManager.instance.Get<MenuTitle>("Prefab/Menu/CanvasTitle").Initialize();
    }

    public override async UniTask Execute() {
        // タイトルメニュー表示
        await MenuManager.instance.Get<MenuTitle>().Open();


        // メインパートへ遷移
        UniTask task = PartManager.Instance.TransitionPart(eGamePart.MainGame);
        await UniTask.CompletedTask;
    }


}
