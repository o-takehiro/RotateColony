using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// MenuからMainGameに行くためのモード選択入力制御クラス
/// </summary>
public class TapToMode : MonoBehaviour {
    [SerializeField] private InputActionReference _normalButton;
    [SerializeField] private InputActionReference _endlessButton;
    [SerializeField] private InputActionReference _howToPlayButton; // ← 追加

    private MenuMode _mode;

    private void Awake() {
        _mode = GetComponentInParent<MenuMode>();
    }

    private void OnEnable() {
        if (_normalButton != null) {
            _normalButton.action.performed += OnTap;
            _normalButton.action.Enable();
        }

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

    private void OnTap(InputAction.CallbackContext context) {
        _mode._isNormal = true;
    }

    private void OnTap2(InputAction.CallbackContext context) {
        _mode._isEndless = true;
    }

    private void OnTapHowToPlay(InputAction.CallbackContext context) {
        _mode.ToggleHowToPlay();
    }
}