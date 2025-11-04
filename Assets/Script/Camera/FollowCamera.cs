/*
 *  @file   FollowCamera.cs
 *  @author oorui
 */

using UnityEngine;

/// <summary>
/// プレイヤーを追従するカメラ制御クラス
/// </summary>
public class FollowCamera : MonoBehaviour {
    /// <summary>
    /// カメラの状態
    /// </summary>
    private enum CameraState {
        Idle,           // 待機
        CircularMove,   // 周回演出
        Follow,         // 通常カメラ
        Clear           // 固定カメラ
    }

    [Header("Follow Settings")]
    [SerializeField] private Transform target;                         // 追従対象
    [SerializeField, Range(0f, 10f)] private float followSpeed = 2.5f; // 追従時の滑らかさ

    // カメラ注視位置の高さ
    private const float UP_EYE = 2.5f;

    // 現在の状態
    private CameraState currentState = CameraState.Idle;

    // 円軌道移動関連
    private Vector3 startOffset;
    private Vector3 endOffset;
    private float startAngle;
    private float endAngle;
    private float moveDuration = 3f;
    private float elapsedTime = 0f;

    // 固定視点関連
    private Vector3 clearPosition;

    /// <summary>
    /// 追従対象を設定
    /// </summary>
    /// <param name="_target">追従対象のTransform</param>
    public void SetTarget(Transform _target) {
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

        switch (currentState) {
            case CameraState.CircularMove:
                UpdateCircularMove();
                break;

            case CameraState.Follow:
                UpdateFollow();
                break;

            case CameraState.Clear:
                UpdateClearView();
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
    /// ゲームクリア時などにカメラを固定
    /// </summary>
    public void ClearCameraAngle() {
        clearPosition = transform.position;
        currentState = CameraState.Clear;
    }

    /// <summary>
    /// 固定視点状態を解除（通常追従へ戻す）
    /// </summary>
    public void ExitCamera() {
        currentState = CameraState.Follow;
    }
}