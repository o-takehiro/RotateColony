using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各ステージにアタッチし、回転処理を行う
/// </summary>
public class StageSegment : MonoBehaviour {
    private bool shouldRotate = false;


    /// <summary>
    /// 回転対象にする
    /// </summary>
    /// <param name="_enable"></param>
    public void EnableRotation(bool _enable) {
        shouldRotate = _enable;
    }

    private void Update() {

        // 回転対象であれば
        if(shouldRotate) {
            transform.Rotate(0f, 30f * Time.deltaTime, 0f);
        }

    }

}
