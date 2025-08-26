using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour {
    // プレイヤーの移動クラスを取得
    private PlayerMove playerMove;
    // プレイヤー停止フラグ
    bool shouldStop = false;
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake() {
        playerMove = GetComponent<PlayerMove>();
        shouldStop = false;
    }

    /// <summary>
    /// プレイヤーが障害物タグに接地したとき、プレイヤーの動きを止める
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {

        // 障害物に当たった時
        if (other.CompareTag("Obstacle")) {
            shouldStop = true;
        }

        // 非加速時、破壊可能壁に当たったとき
        if (other.CompareTag("BoostBreak") && !playerMove.boostFlag) {
            shouldStop = true;
        }

        // エフェクト再生関数を呼ぶ
        if (shouldStop) {
            Vector3 pos =  playerMove.transform.position;
            pos += playerMove.transform.right * -2f;
            playerMove.StopMoving();
            PlayHitEffect(pos);
        }
    }


    /// <summary>
    /// 障害物に当たったときのエフェクト再生
    /// </summary>
    private void PlayHitEffect(Vector3 pos) {
        // エフェクト再生
        EffectManager.Instance.Play("fire", pos);
    }
}
