/*
 * @file    MenuTitle.cs
 * @author  oorui
 */

using Cysharp.Threading.Tasks;

/// <summary>
/// タイトルシーン
/// </summary>
public class MenuTitle : MenuBase {
    public bool isCloseScene = false;   // 画面切り替えの状態
    private const int _TITLE_SE_ID = 1; // 使用するSEのID
    public WaveTextManager waveText;    // アニメーションテキスト

    // <summary>
    // 初期化
    // </summary>
    // <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
        gameObject.SetActive(true);
        await UniTask.CompletedTask;
    }


    /// <summary>
    /// メニューを開いたときの処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open() {
        isCloseScene = false;
        await base.Open();
        await FadeManager.instance.FadeIn();
        // 押されるのを待つ
        while (true) {
            // InputSystemに切り替え
            if (isCloseScene) {
                // SE再生
                UniTask task = SoundManager.instance.PlaySE(_TITLE_SE_ID);
                break;
            }
            await UniTask.DelayFrame(1);
        }
        await FadeManager.instance.FadeOut();
        await Close();
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close() {
        await base.Close();
        isCloseScene = false;
        await UniTask.CompletedTask;
    }

}
