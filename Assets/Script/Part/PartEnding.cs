using Cysharp.Threading.Tasks;


public class PartEnding : PartBase {
    public override async UniTask Execute() {
        UniTask task = PartManager.Instance.TransitionPart(eGamePart.Title);
        await UniTask.CompletedTask;
    }
}
