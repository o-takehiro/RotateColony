
/*
 *  @file   TapToMode
 *  @author oorui
 */
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// MenuからMainGameに行くためのモード選択入力制御クラス
/// </summary>
public class TapToMode : MonoBehaviour {
    [SerializeField] private InputActionReference _normalButton;        // ノーマルモード選択ボタン
    [SerializeField] private InputActionReference _endlessButton;       // エンドレスモード選択ボタン
    [SerializeField] private InputActionReference _howToPlayButton;     // 操作説明展開用ボタン

    private MenuMode _mode;

    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake() {
        // 親オブジェクトからメニューを取得
        _mode = GetComponentInParent<MenuMode>();
    }

    /// <summary>
    /// 各ボタンのイベントの有効
    /// </summary>
    private void OnEnable() {
        // ノーマルモード時
        if (_normalButton != null) {
            _normalButton.action.performed += OnTap;
            _normalButton.action.Enable();
        }
        // エンドレスモード時
        if (_endlessButton != null) {
            _endlessButton.action.performed += OnTap2;
            _endlessButton.action.Enable();
        }

        // 操作説明画像表示
        if (_howToPlayButton != null) {
            _howToPlayButton.action.performed += OnTapHowToPlay;
            _howToPlayButton.action.Enable();
        }
    }
    /// <summary>
    /// 各ボタンのイベントの無効化
    /// </summary>
    private void OnDisable() {
        if (_normalButton != null) {
            _normalButton.action.performed -= OnTap;
            _normalButton.action.Disable();
        }

        if (_endlessButton != null) {
            _endlessButton.action.performed -= OnTap2;
            _endlessButton.action.Disable();
        }
        // 操作説明画像を閉じる
        if (_howToPlayButton != null) {
            _howToPlayButton.action.performed -= OnTapHowToPlay;
            _howToPlayButton.action.Disable();
        }
    }

    /// <summary>
    /// ノーマルモード選択ボタンが押されたときに呼ばれる
    /// </summary>
    private void OnTap(InputAction.CallbackContext context) {
        _mode._isNormal = true;
    }

    /// <summary>
    /// エンドレスモード選択ボタンが押されたときに呼ばれる
    /// </summary>
    private void OnTap2(InputAction.CallbackContext context) {
        _mode._isEndless = true;
    }

    /// <summary>
    /// 操作説明（HowToPlay）ボタンが押されたときに呼ばれる
    /// </summary>
    private void OnTapHowToPlay(InputAction.CallbackContext context) {
        _mode.ToggleHowToPlay(); // 操作説明画面の開閉を切り替える
    }
}