using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBase : MonoBehaviour {
    [SerializeField]
    private GameObject _menuRoot = null;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Initialize() {
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 開く
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Open() {
        // メニューを表示
        _menuRoot?.SetActive(true);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 開く
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Open(GameResultType type) {
        // メニューを表示
        _menuRoot?.SetActive(true);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Close() {
        // メニューを非表示
        _menuRoot?.SetActive(false);
        await UniTask.CompletedTask;
    }

}
