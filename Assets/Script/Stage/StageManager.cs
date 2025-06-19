using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SystemObject {
    [SerializeField] private Transform player;                      // プレイヤーのTransform
    [SerializeField] private List<GameObject> stagePrefabs;         // ステージのプレハブ群
    
   private float segmentLength = 70f;            // 1ステージPrefabの長さ
   private int initialSegments = 5;               // 初期生成数
   private int maxSegments = 5;                   // 保持するステージPrefab最大数

    private List<GameObject> activeSegments = new List<GameObject>();
    private float spawnZ = 40;     // 初期生成位置

    public static StageManager instance { get; private set; } = null;

    public override async UniTask Initialize() {
        instance = this;
        for (int i = 0; i < initialSegments; i++) {
            SpawnSegment();
        }
        await UniTask.CompletedTask;
    }

    private void Update() {
        if (player == null) return;
        // プレイヤーが次のステージPrefabに近づいたら新しいステージを生成
        if (player.position.z > spawnZ - segmentLength * (initialSegments - 1)) {
            // ステージ生成
            SpawnSegment();
            // ステージ削除
            RemoveOldSegment();
        }

        UpdateActiveSegment();
    }


    /// <summary>
    /// 新しいステージPrefabを生成し、リストに追加
    /// </summary>
    private void SpawnSegment() {
        GameObject prefab = stagePrefabs[Random.Range(0, stagePrefabs.Count)];
        // Y軸をランダムで決定
        float randamX = Random.Range(-90f, 90f);
        // 設定した回転値と位置にprefabを生成
        GameObject segment = Instantiate(prefab, new Vector3(0, 0f, spawnZ), Quaternion.Euler(randamX, 90, 90));
        activeSegments.Add(segment);
        spawnZ += segmentLength;
    }

    /// <summary>
    /// 古いステージPrefabを削除し、リストから除外
    /// </summary>
    private void RemoveOldSegment() {
        if (activeSegments.Count > maxSegments) {
            GameObject oldSegment = activeSegments[0];
            activeSegments.RemoveAt(0);
            Destroy(oldSegment);
        }
    }

    /// <summary>
    /// プレイヤーに最も近いステージPrefabだけを回転対象にする
    /// </summary>
    private void UpdateActiveSegment() {
        // 一番近いステージPrefabを選定
        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject segment in activeSegments) {
            float distance = Mathf.Abs(player.position.z - segment.transform.position.z);
            if (distance < minDistance) {
                minDistance = distance;
                nearest = segment;
            }
        }

        // 各ステージPrefabの回転フラグを更新
        foreach (GameObject segment in activeSegments) {
            StageSegment script = segment.GetComponent<StageSegment>();
            if (script != null) {
                script.EnableRotation(segment == nearest);
            }
        }
    }

    /// <summary>
    /// プレイヤーを取得
    /// </summary>
    /// <param name="_player"></param>
    public void SetPlayer(Transform _player) {
        player = _player;
    }


    /// <summary>
    /// すべてのステージPrefabを削除し、リストと生成位置をリセットする
    /// </summary>
    public void ClearAllSegments() {
        foreach (GameObject segment in activeSegments) {
            if (segment != null) {
                Destroy(segment);
            }
        }
        activeSegments.Clear();
        spawnZ = 50f; // 初期位置にリセット
    }

}
