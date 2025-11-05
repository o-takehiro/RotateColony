/*
 *  @file   PartBase.cs
 *  @author oorui
 */

using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// ゲームパートの基底
/// </summary>
public abstract class PartBase : MonoBehaviour {

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Initialize() {
        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// パート実行前に準備
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask SetUp() {
        gameObject.SetActive(true);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// パートの実行
    /// </summary>
    /// <returns></returns>
    public abstract UniTask Execute();

    /// <summary>
    /// パート終了時の片付け
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Teardown() {
        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }


}
