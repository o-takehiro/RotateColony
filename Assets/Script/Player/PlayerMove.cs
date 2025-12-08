/*
 *  @file   PlayerMove.cs
 *  @author oorui
 */

using UnityEngine;

/// <summary>
/// プレイヤーの移動関連クラス
/// </summary>
public class PlayerMove : PlayerBase {

    private float _playerSpeed = 7f;                 // プレイヤーの通常の速度
    private const float _BOOST_MAX_SPEED = 15f;      // プレイヤーの最大加速時の速度
    private const float _BOOST_ACCELERATIUN = 15f;   // プレイヤーの加速率
    private float _currentSpeed;                     // プレイヤーの現在のスピード

    public bool boostFlag = false;                   // 加速検知用
    private bool _isStopped = false;                 // 衝突検知用
    public bool isMoving = false;                    // 移動開始判断用
    private bool _hasTimerStarted = false;           // 時間計測開始用フラグ

    private GameObject _boostEffect = null;          // エフェクトをGameObjectに格納する用

    [SerializeField]
    private Transform _playerBoostPosRight;          // 加速時エフェクトの右側
    [SerializeField]
    private Transform _playerBoostPosLeft;           // 加速時エフェクトの左側
    private GameObject _boosterEffectRight = null;   // エフェクト格納用
    private GameObject _boosterEffectLeft = null;    // エフェクト格納用

    private const int _PLAY_SE_ID = 4;               // 使用するSEのID

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
            // ブーストによる、加速を行う
            HandleSpeed();
            Move();
            // 時間計測を開始する
            if (!_hasTimerStarted) {
                // ゲーム開始から終わりまでの時間計測を開始
                TimeManager.Instance.StartTimer();
                _hasTimerStarted = true;
            }
        }

    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    public override void Move() {
        // 正面方向に進める
        Vector3 forward = transform.forward;
        transform.Translate(forward * _currentSpeed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// ブーストによる加速/減速
    /// </summary>
    private void HandleSpeed() {
        if (boostFlag) {
            // SE再生
            PlayerPlaySE();
            // ブースト中は加速
            _currentSpeed += _BOOST_ACCELERATIUN * Time.deltaTime;
            _currentSpeed = Mathf.Min(_currentSpeed, _BOOST_MAX_SPEED); // 上限を超えない
            if (_boostEffect == null) {
                // boostがOnになった時
                // エフェクトを格納
                _boostEffect = EffectManager.Instance.Play(EffectID._BOOST, transform.position, false);
                _boosterEffectRight = EffectManager.Instance.Play(EffectID._BOOST2, _playerBoostPosRight.transform.position, false);
                _boosterEffectLeft = EffectManager.Instance.Play(EffectID._BOOST2, _playerBoostPosLeft.transform.position, false);
            }
            else {
                // エフェクトをプレイヤーに追従させる
                {
                    _boostEffect.transform.position = transform.position;
                    _boosterEffectRight.transform.position = _playerBoostPosRight.transform.position;
                    _boosterEffectLeft.transform.position = _playerBoostPosLeft.transform.position;
                }
            }
        }
        else {
            // ブーストOFFなら通常速度に戻す
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _playerSpeed, _BOOST_ACCELERATIUN * Time.deltaTime);

            if (_boostEffect != null) {
                // boostがOFFになった瞬間に止める
                EffectManager.Instance.Stop(EffectID._BOOST, _boostEffect);
                _boostEffect = null;
            }
        }

    }


    /// <summary>
    /// 衝突可否判定
    /// </summary>
    public void StopMoving() {
        // 時間を止める
        TimeManager.Instance.StopTimer();
        // リザルトデータに時間を送る
        GameResultData.ClearTime = TimeManager.Instance.GetTime();
        // ゲームオーバー用のフラグを立てる
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
        CameraManager camera = Camera.main?.GetComponent<CameraManager>();
        camera.ClearCameraAngle();
    }

    /// <summary>
    /// 音を再生
    /// </summary>
    private async void PlayerPlaySE() {
        await SoundManager.instance.PlaySE(_PLAY_SE_ID);
    }

}
