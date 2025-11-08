
/*
 *  @file   TapToStart
 *  @author oorui
 */

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// MenuからMainGameに行くためのフラグ変更
/// </summary>
public class TapToStart : MonoBehaviour {
    // 入力を受け取るInputAction
    [SerializeField]
    private InputActionReference _tap;
    private MenuTitle _title;           // タイトルメニュー


    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start() {
        // 親オブジェクトのスクリプトを取得
        _title = GetComponentInParent<MenuTitle>();
        if (_tap != null) {
            _tap.action.performed += OnTap;

            // 有効化
            _tap.action.Enable();
        }

    }

    /// <summary>
    /// タップしたときの処理
    /// </summary>
    /// <param name="context"></param>
    private void OnTap(InputAction.CallbackContext context) {
        // シーンを閉じる用のフラグをtrueに
        _title.isCloseScene = true;
    }



}
