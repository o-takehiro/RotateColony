using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーにカメラを追従：仮実装
/// </summary>
public class FollowCamera : MonoBehaviour {
    [SerializeField]
    private Transform target;   // 追従対象

    //private Vector3 offset = new Vector3(-3, 0, 3);   // カメラ位置のオフセット
    private float followSpeed = 2.5f;     // 追従時の滑らかさ


    private bool isStartMoving = false;
    private float moveDuration = 3f;
    private float elapsedTime = 0f;

    // 円軌道演出用
    private Vector3 startOffset;
    private Vector3 endOffset;
    private float startAngle;
    private float endAngle;

    // 注視する場所 + ●●
    private readonly float UP_EYE = 2.5f;

    /// <summary>
    /// 追従対象を設定
    /// </summary>
    /// <param name="_target"></param>
    public void SetTarget(Transform _target) {
        target = _target;
    }

    /// <summary>
    /// ゲーム開始時のカメラ移動
    /// </summary>
    public void StartCircularMove(Vector3 fromOffset, Vector3 toOffset, float fromAngleDeg, float toAngleDeg, float duration) {
        startOffset = fromOffset;
        endOffset = toOffset;
        startAngle = fromAngleDeg;
        endAngle = toAngleDeg;
        moveDuration = duration;
        elapsedTime = 0f;
        isStartMoving = true;
    }

    /// <summary>
    /// 追従
    /// </summary>
    private void LateUpdate() {
        if (target == null) return;

        // 補間開始
        if (isStartMoving) {
            // 経過時間を足す
            elapsedTime += Time.deltaTime;

            // 値を0~1の間にする
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            // 高さと距離の補間
            Vector3 currentOffset = Vector3.Lerp(startOffset, endOffset, t);

            // Y軸を補間してラジアン角に変換
            float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;

            // 円周上の位置を計算
            float x = Mathf.Cos(angle) * currentOffset.magnitude;
            float z = Mathf.Sin(angle) * currentOffset.magnitude;
            Vector3 offsetPosition = new Vector3(x, currentOffset.y, z);

            // カメラ位置設定
            transform.position = target.position + offsetPosition;

            // 常にプレイヤーを見る
            //transform.LookAt(target);
            transform.LookAt(target.position + Vector3.up * UP_EYE);
            // 補間完了したらフラグオフ
            if (t >= 1f) isStartMoving = false;


        }
        else {
            // 補間後、通所の追跡カメラになる。
            Vector3 desiredPosition = target.position + endOffset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            //transform.LookAt(target);
            // プレイヤーの少し上を向くようにする（例：1.5m上を注視）
            Vector3 lookTarget = target.position + Vector3.up * UP_EYE;
            // 正面方向から見て上に上げる
            transform.rotation = Quaternion.LookRotation(lookTarget - transform.position);



        }
    }
}


