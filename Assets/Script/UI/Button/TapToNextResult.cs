using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// リザルト画面でタップ入力を検知し、状態を進める
/// </summary>
public class TapToNextResult : MonoBehaviour {
    [SerializeField]
    private InputActionReference _tap;

    private MenuResult _menu;

    private void Awake() {
        _menu = GetComponentInParent<MenuResult>();
    }

    private void OnEnable() {
        if (_tap != null) {
            _tap.action.performed += OnTap;
            _tap.action.Enable();
        }
    }

    private void OnDisable() {
        if (_tap != null) {
            _tap.action.performed -= OnTap;
            _tap.action.Disable();
        }
    }

    private void OnTap(InputAction.CallbackContext context) {
        _menu?.NextState();
    }
}