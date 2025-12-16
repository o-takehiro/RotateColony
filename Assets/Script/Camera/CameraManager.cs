/*
 *  @file   FollowCamera.cs
 *  @author oorui
 */

using UnityEngine;

/// <summary>
/// プレイヤーを追従するカメラ制御クラス
/// </summary>
public class CameraManager : MonoBehaviour {
    /// <summary>
    /// カメラの状態
    /// </summary>
    private enum CameraState {
        Idle,           // 待機
        CircularMove,   // 周回演出
        Follow,         // 通常カメラ
        Clear,          // 固定カメラ
        Shake           // カメラシェイク
    }
    // 現在の状態
    private CameraState currentState = CameraState.Idle;

    [SerializeField] private Transform target;          // 追従対象
    [SerializeField] private float followSpeed = 2.5f;  // 追従時の滑らかさ

    private const float UP_EYE = 2.5f;  // カメラ注視位置の高さ


    // 円軌道移動関連
    private Vector3 startOffset;        // スタートからの距離
    private Vector3 endOffset;          // 終了地点からの距離
    private float startAngle;           // スタート時の角度
    private float endAngle;             // 終了時の角度
    private float moveDuration = 3f;    // 円軌道のかかる時間
    private float elapsedTime = 0f;

    // 固定視点関連
    private Vector3 clearPosition;

    // カメラシェイク
    private float shakeDuration = 0f;           // 時間
    private float shakeMagnitude = 0.3f;        // 大きさ
    private float shakeSpeed = 2f;              // 速度
    private Vector3 shakeOffset = Vector3.zero; // オフセット
    private const float _AMOUNT_SWAY = 0.5f;    // 揺れ幅

    /// <summary>
    /// 追従対象を設定
    /// </summary>
    /// <param name="_target">追従対象のTransform</param>
    public void SetTarget(Transform _target) {
        // 対象がいなければ抜ける
        if (_target == null) return;
        // 対象を瀬戸
        target = _target;
    }

    /// <summary>
    /// ゲーム開始時の円軌道カメラ移動を開始
    /// </summary>
    /// <param name="fromOffset">開始位置オフセット</param>
    /// <param name="toOffset">終了位置オフセット</param>
    /// <param name="fromAngleDeg">開始角度（度）</param>
    /// <param name="toAngleDeg">終了角度（度）</param>
    /// <param name="duration">移動にかける時間</param>
    public void StartCircularMove(Vector3 fromOffset, Vector3 toOffset, float fromAngleDeg, float toAngleDeg, float duration) {
        startOffset = fromOffset;
        endOffset = toOffset;
        startAngle = fromAngleDeg;
        endAngle = toAngleDeg;
        moveDuration = duration;
        elapsedTime = 0f;
        currentState = CameraState.CircularMove;
    }

    /// <summary>
    /// 毎フレーム後のカメラ更新処理
    /// </summary>
    private void LateUpdate() {
        if (target == null) return;

        // 状態によってカメラの挙動を変化させる
        switch (currentState) {
            case CameraState.CircularMove:
                // カメラの円軌道
                UpdateCircularMove();
                break;

            case CameraState.Follow:
                // 通常のカメラ移動 
                UpdateFollow();
                break;

            case CameraState.Clear:
                // クリア時のカメラ移動
                UpdateClearView();
                break;
            case CameraState.Shake:
                // カメラの揺れ
                UpdateShake();
                break;
        }
    }

    /// <summary>
    /// 円軌道演出中の更新処理
    /// </summary>
    private void UpdateCircularMove() {
        // 経過時間を進める
        elapsedTime += Time.deltaTime;

        // 0~1の範囲に補間値をクランプ
        float t = Mathf.Clamp01(elapsedTime / moveDuration);

        // オフセットを補間
        Vector3 currentOffset = Vector3.Lerp(startOffset, endOffset, t);

        // 角度を補間してラジアンに変換
        float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;

        // 周回の位置を計算
        float x = Mathf.Cos(angle) * currentOffset.magnitude;
        float z = Mathf.Sin(angle) * currentOffset.magnitude;
        Vector3 offsetPosition = new Vector3(x, currentOffset.y, z);

        // カメラ位置を設定
        transform.position = target.position + offsetPosition;

        // プレイヤーの上方向を見る
        transform.LookAt(target.position + Vector3.up * UP_EYE);

        // 演出終了後、通常追従カメラに移行
        if (t >= 1f) currentState = CameraState.Follow;
    }

    /// <summary>
    /// 通常カメラの更新処理
    /// </summary>
    private void UpdateFollow() {
        // 目標位置
        Vector3 desiredPosition = target.position + endOffset;

        // 現在位置から滑らかに補間
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // ターゲットの少し上を見る
        Vector3 lookTarget = target.position + Vector3.up * UP_EYE;
        transform.rotation = Quaternion.LookRotation(lookTarget - transform.position);
    }

    /// <summary>
    /// 固定カメラの更新処理
    /// </summary>
    private void UpdateClearView() {
        transform.position = clearPosition;
        transform.LookAt(target.position + Vector3.up * UP_EYE);
    }

    /// <summary>
    /// カメラを設定位置で固定させる
    /// </summary>
    public void ClearCameraAngle() {
        // カメラ位置をクリア判定になった場所に設定
        clearPosition = transform.position;
        // カメラの状態をクリア状態にする
        currentState = CameraState.Clear;
    }

    /// <summary>
    /// 固定視点状態を解除
    /// </summary>
    public void ExitCamera() {
        // カメラの状態を通常カメラ状態に戻す
        currentState = CameraState.Follow;
    }

    /// <summary>
    /// カメラ揺れ更新
    /// </summary>
    private void UpdateShake() {
        // 通常の追従カメラ処理
        UpdateFollow();

        if (shakeDuration > 0f) {
            // 揺れの量を計算
            float x = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - _AMOUNT_SWAY) * shakeMagnitude;
            float y = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - _AMOUNT_SWAY) * shakeMagnitude;

            // オフセットに揺れの値を入れる
            shakeOffset = new Vector3(x, y, 0f);

            // 追従位置に対して揺れのオフセットを足す
            transform.position += shakeOffset;

            // 揺れ時間を減らす
            shakeDuration -= Time.deltaTime;
        }
        else {
            // オフセットをリセットし通常追従へ戻す
            shakeOffset = Vector3.zero;
            currentState = CameraState.Follow;
        }
    }

    /// <summary>
    /// カメラの状態を揺れに変更
    /// </summary>
    public void ShakeCamera() {
        // カメラの状態を揺れ状態に変更
        currentState = CameraState.Shake;
        // 揺れの時間を設定
        shakeDuration = 4f;
    }
}