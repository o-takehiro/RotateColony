using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// MenuからMainGameに行くためのモード選択入力制御クラス
/// </summary>
public class TapToMode : MonoBehaviour {
    [SerializeField] private InputActionReference _normalButton;
    [SerializeField] private InputActionReference _EndllesButton;

    private MenuMode _mode;

    private void Awake() {
        _mode = GetComponentInParent<MenuMode>();
    }

    private void OnEnable() {
        if (_normalButton != null) {
            _normalButton.action.performed += OnTap;
            _normalButton.action.Enable();
        }

        if (_EndllesButton != null) {
            _EndllesButton.action.performed += OnTap2;
            _EndllesButton.action.Enable();
        }
    }

    private void OnDisable() {
        if (_normalButton != null) {
            _normalButton.action.performed -= OnTap;
            _normalButton.action.Disable();
        }

        if (_EndllesButton != null) {
            _EndllesButton.action.performed -= OnTap2;
            _EndllesButton.action.Disable();
        }
    }

    private void OnTap(InputAction.CallbackContext context) {
        _mode._isNormal = true;
    }

    private void OnTap2(InputAction.CallbackContext context) {
        _mode._isEndless = true;
    }
}