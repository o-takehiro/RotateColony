using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class PlayerManager : SystemObject {

    public static PlayerManager instance { get; private set; } = null;

    // プレイヤーの親オブジェクト
    [SerializeField]
    private GameObject playerPrefab;
    private GameObject playerInstance;

    /// <summary>
    /// シングルトンの初期化
    /// </summary>
    private void Awake() {
        if(instance != null && instance != this) {
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
    public void UsePlayer(Vector3 _position,Quaternion _rotation) {
        if (playerInstance == null && playerPrefab != null) {
            // 生成
            playerInstance = Instantiate(playerPrefab, _position, _rotation);

            FollowCamera camera = Camera.main?.GetComponent<FollowCamera>();
            if(camera != null) {
                // カメラのターゲットに生成したプレイヤーをセット
                camera.SetTarget(playerInstance.transform);
            }

        }
    }

    /// <summary>
    /// プレイヤーを削除
    /// </summary>
    public void DestroyPlayer() {
        if (playerInstance != null) {
            Destroy(playerInstance);
            playerInstance = null;
        }
    }

    /// <summary>
    /// プレイヤーの取得
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayerObject() => playerInstance;

}
