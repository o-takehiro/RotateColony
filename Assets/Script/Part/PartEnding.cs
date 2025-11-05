/*
 *  @file   PartEnding.cs
 *  @author oorui
 */

using Cysharp.Threading.Tasks;

public class PartEnding : PartBase {

    private const string _CANVAS_TITLE = "Prefab/Menu/CanvasTitle";     // Titleの場所
    private const string _CANVAS_RESULT = "Prefab/Menu/CanvasResult";   // Resultの場所

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
        // メニューの初期化
        await MenuManager.instance.Get<MenuResult>(_CANVAS_RESULT).Initialize();
    }
    /// <summary>
    /// 準備
    /// </summary>
    /// <returns></returns>
    public override async UniTask SetUp() {
        await base.SetUp();

        var menuResult = MenuManager.instance.Get<MenuResult>();

        // クリアかゲームオーバーかどうかで処理を変える
        switch (GameResultData.ResultType) {
            case GameResultType.Clear:
                // クリア画面を表示
                await menuResult.Open(GameResultType.Clear);
                break;
            case GameResultType.GameOver:
                // ゲームオーバー画面を表示
                await menuResult.Open(GameResultType.GameOver);
                break;
        }
    }
    /// <summary>
    /// 実行
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute() {
        // フェードを行う
        await FadeManager.instance.FadeOut();
        // タイトル画面を事前にロードしておく
        var menuTitle = MenuManager.instance.Get<MenuTitle>(_CANVAS_TITLE);
        await menuTitle.Initialize();
        // Titleシーンに遷移
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