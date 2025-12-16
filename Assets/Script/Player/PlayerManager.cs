/*
 *  @file   PlayerManager.cs
 *  @author oorui
 */

using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// プレイヤーの管理クラス
/// </summary>
public class PlayerManager : SystemObject {

    public static PlayerManager instance { get; private set; } = null;  // 自身の参照

    // プレイヤーの親オブジェクト
    [SerializeField]
    private GameObject playerPrefab;
    private GameObject playerInstance;

    private const float _DURATION = 3f;         // 移動に掛ける時間
    private const float _TO_ANGLE_DEG = 248f;   // カメラ軌道の回転
    private const float _FROM_ANGLE_DEG = 0f;   // カメラ軌道スタート時の角度


    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        instance = this;
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// プレイヤー生成
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_rotation"></param>
    public void UsePlayer(Vector3 _position, Quaternion _rotation) {
        if (playerInstance == null && playerPrefab != null) {
            // プレイヤーを生成
            playerInstance = Instantiate(playerPrefab, _position, _rotation);
            // カメラの取得
            CameraManager camera = Camera.main?.GetComponent<CameraManager>();
            // カメラをセット
            if (camera != null) {
                // カメラのターゲットにプレイヤーをセット
                camera.SetTarget(playerInstance.transform);

                // カメラ演出と追従
                camera.StartCircularMove(
                fromOffset: new Vector3(0f, 4f, -4f),   // 円の半径と高さ初期値（例）
                toOffset: new Vector3(0f, 4f, -6f),     // 高さを少し上げる
                fromAngleDeg: _FROM_ANGLE_DEG,          // スタート角度
                toAngleDeg: _TO_ANGLE_DEG,              // 360度一周させたい場合
                duration: _DURATION                     // かかる時間
                );
            }


        }


    }

    /// <summary>
    /// プレイヤーを削除
    /// </summary>
    public void DestroyPlayer() {
        if (playerInstance != null) {
            // 削除
            Destroy(playerInstance);
            playerInstance = null;
        }
    }

    /// <summary>
    /// プレイヤーの取得
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayerObject() => playerInstance;


    /// <summary>
    /// プレイヤーの座標を取得
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPlayerPos() {
        return playerPrefab.transform.position;
    }

}
