using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuTitle : MenuBase {
    // 画面切り替えの選択
    public bool isCloseScene = false;

    // <summary>
    // 初期化
    // </summary>
    // <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
        await UniTask.CompletedTask;
    }


    /// <summary>
    /// メニューを開いたときの処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open() {
        // ここで必ず初期化する（２周目以降も必須）
        isCloseScene = false;
        await base.Open();
        await FadeManager.instance.FadeIn();
        // Zキーが押されるまで待つ
        while (true) {
            // InputSystemに切り替え
            if (isCloseScene) break;

            await UniTask.Delay(1);
        }
        await FadeManager.instance.FadeOut();
        await Close();
    }

    public override async UniTask Close() {
        await base.Close();
        isCloseScene = false;
        await UniTask.CompletedTask;
    }

}
