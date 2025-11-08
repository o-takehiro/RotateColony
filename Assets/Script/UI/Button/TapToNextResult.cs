
/*
 *  @file   TapToNextResult
 *  @author oorui
 */
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// リザルト画面でタップ入力を検知し、状態を進める
/// </summary>
public class TapToNextResult : MonoBehaviour {
    [SerializeField]
    private InputActionReference _tap;

    private MenuResult _menu;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake() {
        _menu = GetComponentInParent<MenuResult>();
    }

    /// <summary>
    /// ボタンが押された判定にする
    /// </summary>
    private void OnEnable() {
        if (_tap != null) {
            _tap.action.performed += OnTap;
            _tap.action.Enable();
        }
    }

    /// <summary>
    /// ボタンが離された判定にする
    /// </summary>
    private void OnDisable() {
        if (_tap != null) {
            _tap.action.performed -= OnTap;
            _tap.action.Disable();
        }
    }

    /// <summary>
    /// ボタンが押された時の処理を実行
    /// </summary>
    /// <param name="context"></param>
    private async void OnTap(InputAction.CallbackContext context) {
        // SEを再生
        await SoundManager.instance.PlaySE(0);
        // 次の状態へ移行
        _menu?.NextState();
    }
}