using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoostButton : MonoBehaviour {
    // 入力を受け取る対象のアクション
    [SerializeField]
    private InputActionReference _boost;
    [SerializeField]
    private PlayerMove _playerMove;

    private void Start() {
        // 動的にPlayerMoveを探す
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            _playerMove = playerObj.GetComponent<PlayerMove>();
        }

        if (_boost != null) {
            _boost.action.performed += OnHold;
            _boost.action.canceled += OnHold;
            _boost.action.Enable();
        }

    }

    private void OnEnable() {
        if (_boost != null) return;

        // 長押し判定になったらコールバックが呼ばれる
        _boost.action.performed += OnHold;
        _boost.action.canceled += OnHold;
        // 入力を受け取るための必ず有効化
        _boost.action.Enable();

    }

    /// <summary>
    /// 押されたとき処理
    /// </summary>
    /// <param name="context"></param>
    private void OnHold(InputAction.CallbackContext context) {
        switch (context.phase) {
            case InputActionPhase.Performed:
                // ボタンが押された時
                // PlayerMoveの加速フラグをOnにする
                _playerMove.boostFlag = true;
                break;
            case InputActionPhase.Canceled:
                // ボタンが離されたとき
                // PlayerMoveの加速フラグをOffにする
                _playerMove.boostFlag = false;
                break;
        }
    }






}
