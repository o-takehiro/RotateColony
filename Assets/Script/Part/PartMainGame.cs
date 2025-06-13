using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlayerManager;
/// <summary>
/// メインゲームパート
/// </summary>
public class PartMainGame : PartBase {

    private bool endFlag = false;
    private bool clerFlag = false;

    public override async UniTask Initialize() {
        await base.Initialize();
        //await instance.Initialize();
    }

    public override async UniTask SetUp() {
        await base.SetUp();
        await FadeManager.instance.FadeIn();
        // プレイヤーとステージ生成
        instance.UsePlayer(Vector3.zero, Quaternion.Euler(0,90,0));


        await UniTask.CompletedTask;
    }

    public override async UniTask Execute() {
        // ゲームの実行
        if (Input.GetKeyDown(KeyCode.Space)) {
            endFlag = true;
        }
        else if (Input.GetKeyDown(KeyCode.H)) {
            clerFlag = true;
        }
        UniTask task;
        if (endFlag) {
            task = PartManager.Instance.TransitionPart(eGamePart.Title);
        }
        else if (clerFlag) {
            task = PartManager.Instance.TransitionPart(eGamePart.Ending);
        }


    }

    public override async UniTask Teardown() {
        await base.Teardown();
        await UniTask.CompletedTask;
    }

}
