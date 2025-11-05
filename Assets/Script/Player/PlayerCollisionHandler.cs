
/*
 *  @file   PlayerCollisionHandler.cs
 *  @author oorui
 */

using UnityEngine;

/// <summary>
/// プレイヤーが当たったときの処理
/// </summary>
public class PlayerCollisionHandler : MonoBehaviour {

    private PlayerMove playerMove;      // プレイヤーの移動クラスを取得
    bool shouldStop = false;            // プレイヤーの停止フラグ
    private const int _PLAYER_SE_ID = 5;// 使用するSEのID


    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake() {
        // プレイヤーの移動を取得
        playerMove = GetComponent<PlayerMove>();
        // フラグの初期化
        shouldStop = false;
    }

    /// <summary>
    /// プレイヤーの動きを止める
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
            Vector3 pos = playerMove.transform.position;
            // エフェクト再生位置をずらす
            pos += playerMove.transform.right * -2f;
            // プレイヤーの動きを停止させる
            playerMove.StopMoving();
            // エフェクトを指定した位置で再生
            PlayHitEffect(pos);
            // 衝突時のSEを再生
            PlayerPlaySE(shouldStop);
        }
    }


    /// <summary>
    /// 障害物に当たったときのエフェクト再生
    /// </summary>
    private void PlayHitEffect(Vector3 pos) {
        // エフェクト再生
        EffectManager.Instance.Play("fire", pos);
    }

    /// <summary>
    /// フラグが立っていればSEを再生する
    /// </summary>
    /// <param name="flag"></param>
    private async void PlayerPlaySE(bool flag) {
        if (flag) {
            await SoundManager.instance.PlaySE(_PLAYER_SE_ID);
        }
    }
}
