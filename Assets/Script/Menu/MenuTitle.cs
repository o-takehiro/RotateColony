using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTitle : MenuBase {

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
        
        await UniTask.CompletedTask;
    }


    /// <summary>
    /// メニューを開いたときの処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open() {
        await base.Open();
        await FadeManager.instance.FadeIn();
        // Zキーが押されるまで待つ(フラグ切り替え変更予定)
        while (true) {
            // InputSystemに切り替え
            if (Input.GetKeyDown(KeyCode.Z)) break;

            await UniTask.Delay(1);
        }
        await FadeManager.instance.FadeOut();
        await Close();
    }


    /// <summary>
    /// メニューを閉じる
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close() {
        
        await Close();
    }

}
