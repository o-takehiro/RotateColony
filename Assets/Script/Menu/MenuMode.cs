using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

using static SoundManager;
/// <summary>
/// PartTitle→MenuTitle→MenuMode→PartTitleに戻る
/// </summary>
public class MenuMode : MenuBase {
    public bool _isNormal = false;
    public bool _isEndless = false;
    // 使用するSEのID
    private const int _MENU_SE_ID = 0;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override async UniTask Initialize() {
        await base.Initialize();
        //await UniTask.CompletedTask;
    }

    public override async UniTask Open() {
        await base.Open();
        await FadeManager.instance.FadeIn();

        // モードの選択
        // エンドレス or ノーマル
        while (true) {

            if (_isEndless) {
                // SE再生
                await instance.PlaySE(_MENU_SE_ID);
                StageManager.instance.SetupStrategy(GameModeState.Endless);
                break;
            }
            else if (_isNormal) {
                // SE再生
                await instance.PlaySE(_MENU_SE_ID);
                StageManager.instance.SetupStrategy(GameModeState.Normal);
                break;
            }
            await UniTask.Delay(1);
        }

        // フェードアウト
        await FadeManager.instance.FadeOut();
        await Close();
    }


    public override async UniTask Close() {
        await base.Close();

        // 状態をリセットしておく
        _isNormal = false;
        _isEndless = false;

        await UniTask.CompletedTask;
    }

}
