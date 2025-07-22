using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// リザルト画面でタップ入力を検知し、状態を進める
/// </summary>
public class TapToNextResult : MonoBehaviour {
    // Unity InputSystemで使用するアクション（タップ）
    [SerializeField]
    private InputActionReference _tap;

    // 親にある MenuResult を取得して制御する
    private MenuResult _menu;

    /// <summary>
    /// 初期化
    /// </summary>
    private void Start() {
        // 親から MenuResult を取得
        _menu = GetComponentInParent<MenuResult>();

        if (_tap != null) {
            // タップ開始時に OnTap を呼び出すよう登録
            _tap.action.performed += OnTap;

            // タップ入力を有効化
            _tap.action.Enable();
        }
    }

    /// <summary>
    /// MenuResultのStateを遷移させる
    /// </summary>
    private void OnTap(InputAction.CallbackContext context) {
        _menu?.NextState(); // MenuResult に状態遷移を要求
    }

    /// <summary>
    /// タップの状態を削除
    /// </summary>
    private void OnDestroy() {
        if (_tap != null) {
            _tap.action.performed -= OnTap;
            _tap.action.Disable();
        }
    }
}