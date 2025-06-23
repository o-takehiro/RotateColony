using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonManager : SystemObject {

    public static ButtonManager instance { get; private set; } = null;

    // ボタンの親オブジェクト
    [SerializeField]
    private GameObject buttonPrefab;
    private GameObject buttonInstance;


    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake() {
        if (instance != null && buttonInstance != this) {
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
    public void UseButton() {
        if (buttonInstance == null && buttonPrefab != null) {
            // 生成
            buttonInstance = Instantiate(buttonPrefab);

        }
    }


    /// <summary>
    /// ボタンを削除
    /// </summary>
    public void DestroyButton() {
        if (buttonInstance != null) {
            Destroy(buttonInstance);
            buttonInstance = null;
        }
    }

}
