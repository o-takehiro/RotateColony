using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 準備パート
/// </summary>
public class PartStandby : PartBase {
    public override async UniTask Execute() {
        // フェードアウト
        await FadeManager.instance.FadeOut();
        // タイトルパートへ遷移、終了待ちはしない
        UniTask task = PartManager.Instance.TransitionPart(eGamePart.Title);
        await UniTask.CompletedTask;
    }
}
