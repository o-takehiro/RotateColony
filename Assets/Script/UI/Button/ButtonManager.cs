using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonManager : SystemObject {

    public static ButtonManager instance { get; private set; } = null;


    // ボタンのプレハブリスト
    [SerializeField]
    private List<GameObject> buttonPrefabs = new List<GameObject>();

    // 生成したボタンのインスタンス管理
    private List<GameObject> buttonInstances = new List<GameObject>();
    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }


    public override async UniTask Initialize() {
        instance = this;
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// ボタンの生成
    /// </summary>
    /// <param name="index">buttonPrefabs のインデックス</param>
    public void UseButton(int index) {
        if (index < 0 || index >= buttonPrefabs.Count) return;

        var prefab = buttonPrefabs[index];
        if (prefab != null) {
            var instance = Instantiate(prefab);
            buttonInstances.Add(instance);
        }
    }


    /// <summary>
    /// すべてのボタンを削除
    /// </summary>
    public void DestroyAllButtons() {
        foreach (var button in buttonInstances) {
            if (button != null) {
                Destroy(button);
            }
        }
        buttonInstances.Clear();
    }


    /// <summary>
    /// 登録済みのすべてのボタンを一度に生成する
    /// </summary>
    public void UseAllButtons() {
        foreach (var prefab in buttonPrefabs) {
            if (prefab != null) {
                var instance = Instantiate(prefab);
                buttonInstances.Add(instance);
            }
        }
    }


}
