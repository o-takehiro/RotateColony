/*
 *  @file   PartTitle.cs
 *  @author oorui
 */

using Cysharp.Threading.Tasks;

/// <summary>
/// タイトルパート
/// </summary>
public class PartTitle : PartBase {

    private const int _TITLE_BGM_ID = 1;    // 使用するBGMのID
    public WaveTextManager waveText;        // アニメーションテキスト

    private const string _CANVAS_TITLE = "Prefab/Menu/CanvasTitle";
    private const string _CANVAS_MODEMENU = "Prefab/Menu/CanvasModeMenu";

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
        // メニューの初期化
        await MenuManager.instance.Get<MenuTitle>(_CANVAS_TITLE).Initialize();
        await MenuManager.instance.Get<MenuMode>(_CANVAS_MODEMENU).Initialize();
        if (waveText != null) {
            waveText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 実行処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute() {
        // BGM再生
        SoundManager.instance.PlayBGM(_TITLE_BGM_ID);
        // フェードイン
        await FadeManager.instance.FadeIn();
        // タイトルメニュー表示
        await MenuManager.instance.Get<MenuTitle>().Open();
        await UniTask.DelayFrame(1);

        // モード選択画面を表示
        await MenuManager.instance.Get<MenuMode>().Open();
        if (waveText != null) waveText.gameObject.SetActive(false);
        

        // 画面遷移時にフェードアウト
        await FadeManager.instance.FadeOut();
        // メインパートへ遷移
        UniTask task = PartManager.Instance.TransitionPart(eGamePart.MainGame);



        await UniTask.CompletedTask;
    }


}
