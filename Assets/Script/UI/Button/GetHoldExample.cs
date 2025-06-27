using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// ボタンの長押し入力を受付
/// </summary>
public class GetHoldExample : MonoBehaviour {
    // 入力を受け取る対象のアクション
    [SerializeField]
    private InputActionReference _hold;
    [SerializeField]
    private PlayerShot _playerShoot;
    // private Image _image = null;

    private void Start() {
        // 動的にPlayerShotを探す（例：タグが"Player"のオブジェクトにアタッチされているなら）
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        // _image = gameObject.GetComponent<Image>();
        if (playerObj != null) {
            _playerShoot = playerObj.GetComponent<PlayerShot>();
        }

        if (_hold != null) {
            _hold.action.performed += OnHold;
            _hold.action.canceled += OnHold;
            _hold.action.Enable();
        }
    }

    private void OnEnable() {
        if (_hold == null) return;


        // 長押し判定になったらコールバックが呼ばれる
        _hold.action.performed += OnHold;
        _hold.action.canceled += OnHold;
        // 入力を受け取るために必ず有効化
        _hold.action.Enable();

    }

    /// <summary>
    /// 長押しされたときに呼ばれるメソッド
    /// </summary>
    /// <param name="context"></param>
    private void OnHold(InputAction.CallbackContext context) {
        // プレイヤーに射撃可能かどうか渡す

        switch (context.phase) {
            case InputActionPhase.Performed:
                // ボタンが押された時の処理
                // PlayerShotの射撃可能フラグをtrueにする
                Debug.Log("isShot_true");
                // _image.color = Color.gray;
                _playerShoot.isShot = true;

                break;

            case InputActionPhase.Canceled:
                // ボタンが離された時の処理
                // PlayerShotの射撃可能フラグをfalseにする
                Debug.Log("isShot_false");
                // _image.color = Color.white;
                _playerShoot.isShot = false;

                break;
        }


    }

    private void OnDisable() {
        if (_hold == null) return;

        _hold.action.performed -= OnHold;
        _hold.action.canceled -= OnHold;
        _hold.action.Disable();
    }

}
