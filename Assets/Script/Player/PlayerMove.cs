using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class PlayerMove : PlayerBase {
    private float _playerSpeed = 7f;                 // プレイヤーの通常の速度
    private const float _BOOST_MAX_SPEED = 15f;      // プレイヤーの最大加速時の速度
    private const float _BOOST_ACCELERATIUN = 15f;   // プレイヤーの加速率

    private float _currentSpeed;                     // プレイヤーの現在のスピード

    // 加速検知用
    public bool boostFlag = false;
    // 衝突検知用
    private bool isStopped = false;
    // 移動開始判断用
    public bool isMoving = false;

    // 時間計測用
    public float gameClearTime = 0f;
    private bool _isGameClear = false;

    private void Start() {
        // 現在のspeedに通常の移動速度を入れる
        _currentSpeed = _playerSpeed;
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update() {
        // 衝突してないときかつ、移動開始が許可されたとき
        if (!isStopped && isMoving) {
            HandleSpeed();
            Move();
            gameClearTime += Time.deltaTime;
            if (gameClearTime < 0f) {
                _isGameClear = true;
            }
        }
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    public override void Move() {

        Vector3 forward = transform.forward;
        transform.Translate(forward * _currentSpeed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// ブーストによる加速/減速
    /// </summary>
    private void HandleSpeed() {
        if (boostFlag) {
            // ブースト中は加速
            _currentSpeed += _BOOST_ACCELERATIUN * Time.deltaTime;
            _currentSpeed = Mathf.Min(_currentSpeed, _BOOST_MAX_SPEED); // 上限を超えない
        }
        else {
            // ブーストOFFなら通常速度に戻す
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _playerSpeed, _BOOST_ACCELERATIUN * Time.deltaTime);
        }
    }


    /// <summary>
    /// 衝突可否判定
    /// </summary>
    public void StopMoving() {
        isStopped = true;
    }

    /// <summary>
    /// 衝突可否判定取得
    /// </summary>
    /// <returns></returns>
    public bool GetIsStopped() {
        return isStopped;
    }

    /// <summary>
    /// フラグリセット
    /// </summary>
    public void StopedReset() {
        isStopped = false;
    }

    /// <summary>
    /// 移動開始可否判定
    /// </summary>
    /// <param name="_moveFlag"></param>
    public void SetStartMoving(bool _moveFlag) {
        isMoving = _moveFlag;
    }


    /// <summary>
    /// 移動可否判定の取得
    /// </summary>
    /// <returns></returns>
    public bool GetIsMoving() {
        return isMoving;
    }

    /// <summary>
    /// クリアフラグを渡す用
    /// </summary>
    /// <returns></returns>
    public bool GetIsGameClear() {
        return _isGameClear;
    }

}
