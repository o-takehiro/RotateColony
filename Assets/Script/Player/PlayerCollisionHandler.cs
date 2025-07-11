using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour {
    // プレイヤーの移動クラスを取得
    private PlayerMove playerMove;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake() {
        playerMove = GetComponent<PlayerMove>();
    }

    /// <summary>
    /// プレイヤーが障害物タグに接地したとき、プレイヤーの動きを止める
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Obstacle")) playerMove.StopMoving();

        if (other.CompareTag("BoostBreak") && !playerMove.boostFlag) playerMove.StopMoving();

    }
}
