using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各ステージにアタッチし、回転処理を行う
/// </summary>
public class StageSegment : MonoBehaviour {
    private bool shouldRotate = false;

    private bool stopMove = false;
    /// <summary>
    /// 回転対象にする
    /// </summary>
    /// <param name="_enable"></param>
    public void EnableRotation(bool _enable) {
        shouldRotate = _enable;
    }

    private void Update() {
        if (!shouldRotate) return;

        float rotationSpeed = 60f; // 回転速度（度/秒）
        float rotationInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) {
            rotationInput = 1f;  // 左矢印キーで反時計回りに回転
        }
        else if (Input.GetKey(KeyCode.RightArrow)) {
            rotationInput = -1f;   // 右矢印キーで時計回りに回転
        }

        transform.Rotate(0f, rotationSpeed * rotationInput * Time.deltaTime, 0f);
    }



    /// <summary>
    /// 衝突可否判定
    /// </summary>
    public void StopStageMoving() {
        stopMove = true;
    }



}
