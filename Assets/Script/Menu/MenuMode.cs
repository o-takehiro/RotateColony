using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PartTitle→MenuTitle→MenuMode→PartTitleに戻る
/// </summary>
public class MenuMode : MenuBase {

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override async UniTask Initialize() {
        await base.Initialize();
        //await UniTask.CompletedTask;
    }

    public override async UniTask Open() {
        await base.Open();
        // モードの選択
        // エンドレス or ノーマル


        // フェードアウト
        await FadeManager.instance.FadeOut();
        await Close();
    }


    /// <summary>
    /// メニュークローズ処理（状態リセット）
    /// </summary>
    public override async UniTask Close() {
        await base.Close();
        await UniTask.CompletedTask;
    }

}
