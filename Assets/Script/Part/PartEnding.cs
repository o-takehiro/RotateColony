using Cysharp.Threading.Tasks;
using UnityEngine;

public class PartEnding : PartBase {

    /// <summary>
    /// ‰Šú‰»ˆ—
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
    }

    public override async UniTask SetUp() {
        await base.SetUp();

    }

    public override async UniTask Execute() {
        // ¡‚Í‰½‚à‚¹‚¸Å‘¬‚ÅTitle‚É‘JˆÚ‚·‚é
        UniTask task = PartManager.Instance.TransitionPart(eGamePart.Title);
        await UniTask.CompletedTask;
    }
}
