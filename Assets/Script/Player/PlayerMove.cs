using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class PlayerMove : PlayerBase {
    private float playerSpeed = 3f;

    // 衝突検知用
    private bool isStopped = false;

    // 移動開始判断用
    private bool isMoving = false;


    /// <summary>
    /// 更新
    /// </summary>
    private void Update() {
        // 衝突してないときかつ、移動開始が許可されたとき
        if (!isStopped && isMoving) {
            Move();
        }
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    public override void Move() {

        Vector3 forward = transform.forward;
        transform.Translate(forward * playerSpeed * Time.deltaTime, Space.World);
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
    ///  フラグリセット
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



}
