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
    private bool _isStopped = false;
    // 移動開始判断用
    public bool isMoving = false;

    //private bool _isGameClear = false;

    // エフェクトをGameObjectに格納するよう
    private GameObject _boostEffect = null;
    // 時間計測開始用フラグ
    private bool _hasTimerStarted = false;

    [SerializeField]
    private Transform _playerBoostPosRight;     // 加速時エフェクトの右側
    [SerializeField]
    private Transform _playerBoostPosLeft;      // 加速時エフェクトの左側
    private GameObject _boosterEffectRight = null;    // エフェクト格納用
    private GameObject _boosterEffectLeft = null;     // エフェクト格納用

    private void Start() {
        // 現在のspeedに通常の移動速度を入れる
        _currentSpeed = _playerSpeed;


    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update() {
        // 衝突してないときかつ、移動開始が許可されたとき
        if (!_isStopped && isMoving) {
            HandleSpeed();
            Move();
            // 時間計測を開始する
            if (!_hasTimerStarted) {
                TimeManager.Instance.StartTimer();
                _hasTimerStarted = true;
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
            if (_boostEffect == null) {
                // boostがOnになった時
                // エフェクトを格納
                _boostEffect = EffectManager.Instance.Play("boost", transform.position, false);
                _boosterEffectRight = EffectManager.Instance.Play("booster", _playerBoostPosRight.transform.position, false);
                _boosterEffectLeft = EffectManager.Instance.Play("booster", _playerBoostPosLeft.transform.position, false);
            }
            else {
                // エフェクトをプレイヤーに追従させる
                _boostEffect.transform.position = transform.position;
                _boosterEffectRight.transform.position = _playerBoostPosRight.transform.position;
                _boosterEffectLeft.transform.position = _playerBoostPosLeft.transform.position;
            }
        }
        else {
            // ブーストOFFなら通常速度に戻す
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _playerSpeed, _BOOST_ACCELERATIUN * Time.deltaTime);

            if (_boostEffect != null) {
                // boostがOFFになった瞬間に止める
                EffectManager.Instance.Stop("boost", _boostEffect);
                _boostEffect = null;
            }
        }

    }


    /// <summary>
    /// 衝突可否判定
    /// </summary>
    public void StopMoving() {

        TimeManager.Instance.StopTimer();
        GameResultData.ClearTime = TimeManager.Instance.GetTime();
        _isStopped = true;
    }

    /// <summary>
    /// 衝突可否判定取得
    /// </summary>
    /// <returns></returns>
    public bool GetIsStopped() {
        return _isStopped;
    }

    /// <summary>
    /// フラグリセット
    /// </summary>
    public void StopedReset() {
        _isStopped = false;
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
    /// ゲームをクリアした際に呼ばれる
    /// </summary>
    public void ClearPlayer() {
        // 時間を止めてセットする
        TimeManager.Instance.StopTimer();
        GameResultData.ClearTime = TimeManager.Instance.GetTime();
    }

}
