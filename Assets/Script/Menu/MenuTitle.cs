using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTitle : MenuBase {

    public override async UniTask Open() {
        await base.Open();
        await FadeManager.instance.FadeIn();
        // ZƒL[‚ª‰Ÿ‚³‚ê‚é‚Ü‚Å‘Ò‚Â
        while (true) {
            if (Input.GetKeyDown(KeyCode.Z)) break;

            await UniTask.Delay(1);
        }
        await FadeManager.instance.FadeOut();
        await Close();
    }

}
