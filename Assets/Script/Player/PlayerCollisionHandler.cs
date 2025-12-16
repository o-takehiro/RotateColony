
/*
 *  @file   PlayerCollisionHandler.cs
 *  @author oorui
 */

using UnityEngine;
using static GameConst;
/// <summary>
/// プレイヤーが当たったときの処理
/// </summary>
public class PlayerCollisionHandler : MonoBehaviour {

    private PlayerMove playerMove;      // プレイヤーの移動クラスを取得
    private const int _PLAYER_SE_ID = 5;// 使用するSEのID

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake() {
        // プレイヤーの移動を取得
        playerMove = GetComponent<PlayerMove>();
    }

    /// <summary>
    /// プレイヤーの動きを止める
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {

        // 障害物に当たった時
        if (other.CompareTag(_OBSTACLE_TAG) || other.CompareTag(_BOOSTBREAK_TAG) && !playerMove.boostFlag) {
            // エフェクト再生
            BrakePlayer();
        }
    }

    /// <summary>
    /// プレイヤーがぶつかった際に行う
    /// </summary>
    private void BrakePlayer() {
        // 現在の座標を取得
        Vector3 pos = playerMove.transform.position;
        // エフェクト再生位置をずらす
        pos += playerMove.transform.right * -2f;
        // カメラを揺らす
        CameraManager camera = Camera.main?.GetComponent<CameraManager>();
        if (camera == null) return;
        camera.ShakeCamera();
        // プレイヤーの動きを停止
        playerMove.StopMoving();
        // エフェクト再生
        PlayHitEffect(pos);
        // SEを再生
        PlayerPlaySE();
    }

    /// <summary>
    /// 障害物に当たったときのエフェクト再生
    /// </summary>
    private void PlayHitEffect(Vector3 pos) {
        // エフェクト再生
        EffectManager.Instance.Play(EffectID._BRAKE, pos);
    }

    /// <summary>
    /// フラグが立っていればSEを再生する
    /// </summary>
    /// <param name="flag"></param>
    private async void PlayerPlaySE() {
        // SE再生
        await SoundManager.instance.PlaySE(_PLAYER_SE_ID);

    }
}
