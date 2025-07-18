using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// MenuからMainGameに行くためのフラグ変更
/// </summary>
public class TapToStart : MonoBehaviour {
    // 入力を受け取るInputAction
    [SerializeField]
    private InputActionReference _tap;
    private MenuTitle _title;


    private void Start() {
        // 親オブジェクトのスクリプトを取得
        _title = GetComponentInParent<MenuTitle>();
        if (_tap != null) {
            _tap.action.started += OnTap;

            // 有効化
            _tap.action.Enable();
        }

    }

    /// <summary>
    /// タップしたときの処理
    /// </summary>
    /// <param name="context"></param>
    private void OnTap(InputAction.CallbackContext context) {
        _title.isCloseScene = true;
    }

    

}
