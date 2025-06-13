using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーにカメラを追従：仮実装
/// </summary>
public class FollowCamera : MonoBehaviour {
    [SerializeField]
    private Transform target;   // 追従対象
    [SerializeField]
    private Vector3 offset = new Vector3(-5, 1.6f, 0.0f);   // カメラ位置のオフセット
    [SerializeField]
    private float followSpeed = 2.5f;     // 追従時の滑らかさ


    /// <summary>
    /// 追従対象を設定
    /// </summary>
    /// <param name="_target"></param>
    public void SetTarget(Transform _target) {
        target = _target;
    }

    /// <summary>
    /// 追従
    /// </summary>
    private void LateUpdate() {
        if (target == null) return;

        // 目標位置の計算
        Vector3 desiredPosition = target.position + offset;
        // 滑らかに補間
        transform.position = Vector3.Lerp(transform.position,desiredPosition, followSpeed * Time.deltaTime);

    }

}
