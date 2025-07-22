using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class TapToMode : MonoBehaviour {
    // 入力を受け取るInputAction
    [SerializeField]
    private InputActionReference _normalButton;
    [SerializeField]
    private InputActionReference _EndllesButton;
    private MenuMode _mode;


    private void Start() {
        // 親オブジェクトのスクリプトを取得
        _mode = GetComponentInParent<MenuMode>();
        // ノーマルモード選択用ボタン
        if (_normalButton != null) {
            _normalButton.action.performed += OnTap;

            // 有効化
            _normalButton.action.Enable();
        }
        // エンドレスモード選択用ボタン
        if (_EndllesButton != null) {
            _EndllesButton.action.performed += OnTap2;

            // 有効化
            _EndllesButton.action.Enable();
        }
    }

    /// <summary>
    /// ノーマルボタンが選ばれたときの処理
    /// </summary>
    /// <param name="context"></param>
    private void OnTap(InputAction.CallbackContext context) {
        // ノーマルモードに設定する
        _mode._isNormal = true;
    }


    /// <summary>
    /// エンドレスボタンが押されたときの処理
    /// </summary>
    /// <param name="context"></param>
    private void OnTap2(InputAction.CallbackContext context) {
        // エンドレスモードに設定する
        _mode._isEndless = true;
    }

}
