using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの実行処理
/// </summary>
public static class PlayerUtility {

    /// <summary>
    /// 指定した位置と回転でプレイヤーを生成する
    /// </summary>
    /// <param name="_positon"></param>
    /// <param name="_rotation"></param>
    /// <returns></returns>
    public static GameObject CreatePlayer(Vector3 _positon, Quaternion _rotation) {
        GameObject playerPrefab = Resources.Load<GameObject>("Player/PlayerPrefab");

        if (playerPrefab == null) return null;

        // プレイヤーを指定位置、回転で生成し、インスタンスを返す
        return GameObject.Instantiate(playerPrefab, _positon, _rotation);

    }

    /// <summary>
    /// 指定されたプレイヤーのGameObjectを破棄する
    /// </summary>
    /// <param name="_player"></param>
    public static void DestroyPlayer(GameObject _player) {
        if (_player == null) return;

        // プレイヤー破棄
        GameObject.Destroy(_player);

    }




}
