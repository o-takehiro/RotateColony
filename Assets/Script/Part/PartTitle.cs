using Cysharp.Threading.Tasks;
using System.Net.Sockets;

/// <summary>
/// タイトルパート
/// </summary>
public class PartTitle : PartBase {
    public override async UniTask Initialize() {
        await base.Initialize();
        // メニューの初期化
        await MenuManager.instance.Get<MenuTitle>("Prefab/Menu/CanvasTitle").Initialize();
        await MenuManager.instance.Get<MenuMode>("Prefab/Menu/CanvasModeMenu").Initialize();
    }

    public override async UniTask Execute() {
        // タイトルメニュー表示
        await MenuManager.instance.Get<MenuTitle>().Open();
        await UniTask.DelayFrame(1);

        // モード選択のMenuをタイトルメニューのあとに開く
        await MenuManager.instance.Get<MenuMode>().Open();

        // メインパートへ遷移
        UniTask task = PartManager.Instance.TransitionPart(eGamePart.MainGame);
        await UniTask.CompletedTask;
    }


}
