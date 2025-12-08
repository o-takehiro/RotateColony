/*
 *  @file   EffectManager.cs
 *  @brief  エフェクト全体の管理
 *  @author oorui
 */

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エフェクトIDをまとめて管理
/// </summary>
public static class EffectID {
    public const int None = 0;

    public const int _BOOST = 1;
    public const int _EX = 2;
    public const int _EX2 = 3;
    public const int _BRAKE = 4;
    public const int _BOOST2 = 5;
    public const int _SMOKE = 6;

}

public class EffectManager : SystemObject {

    // 自身への参照可能なインスタンス
    public static EffectManager Instance { get; private set; }


    [System.Serializable]
    public class EffectEntry {
        public int id;                     // 識別用のID
        public GameObject prefab;          // 実際のエフェクトのひな型
        public int initialPoolSize = 5;    // あらかじめ確保しておく数
        public float autoReturnTime = -1f; // プールに戻すタイミング
    }
    // プール元の生成位置のstring型
    private string _POOLROOT_NAME = "EffectPoolRoot";

    [SerializeField]
    private EffectEntry[] effectEntries; // インスペクタで設定するリスト

    [SerializeField]
    private Transform poolRoot; // プールをまとめる親のオブジェクト

    // エフェクトのPrefabを名前で管理
    private readonly Dictionary<int, GameObject> effectPrefab = new();

    // 使い回し用の待機エフェクト
    private readonly Dictionary<int, Queue<GameObject>> effectPool = new();

    // 現在使用中のエフェクト
    private readonly List<(int id, GameObject instance)> activeEffectList = new();

    private void Awake() {
        // インスタンスが重複していたら削除する
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 親が設定されていなければ自動で作成する
        if (poolRoot == null) {
            var poolRootObj = new GameObject(_POOLROOT_NAME);
            poolRootObj.transform.SetParent(transform);
            poolRoot = poolRootObj.transform;
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override async UniTask Initialize() {
        Instance = this;
        // poolRoot が確実に存在するようにする
        if (poolRoot == null) {
            var poolRootObj = new GameObject(_POOLROOT_NAME);
            poolRootObj.transform.SetParent(transform);
            poolRoot = poolRootObj.transform;
        }

        // エフェクトのプールを作成
        InitializePools();


        await UniTask.CompletedTask;
    }

    // エフェクトリストをもとにプールを作成する
    private void InitializePools() {
        // 登録されたエフェクトリストを見る
        foreach (var entry in effectEntries) {
            // Prefab、IDが泣ければ生成しない
            if (entry.id <= 0 || entry.prefab == null) continue;

            // IDごとにエフェクトを保存
            effectPrefab[entry.id] = entry.prefab;

            // すでにプールがあるなら作らない
            if (effectPool.ContainsKey(entry.id)) continue;

            // プールを表示する親オブジェクトの作成
            var queue = new Queue<GameObject>();
            // Hieralkey上に表示
            var effectParent = new GameObject($"Pool_{entry.id}");
            effectParent.transform.SetParent(poolRoot);

            // 初期生成分生成し、非表示
            for (int i = 0; i < entry.initialPoolSize; i++) {
                var obj = Instantiate(entry.prefab, effectParent.transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            // 待機エフェクトリストに保存
            effectPool[entry.id] = queue;
        }
    }

    // エフェクトを指定位置で再生する
    public GameObject Play(int effectId, Vector3 position, bool autoReturn = true) {
        if (!effectPool.ContainsKey(effectId)) return null;

        GameObject obj;
        // プールから取り出す
        if (effectPool[effectId].Count > 0) {
            obj = effectPool[effectId].Dequeue();
        }
        else {
            // 最も古く再生されたエフェクトを探して再利用する

            // 使用中リストから同じIDのエフェクトを探す
            var index = activeEffectList.FindIndex(e => e.id == effectId);

            if (index != -1) {
                // 見つかった場合そのエフェクトを停止＆再利用する
                obj = activeEffectList[index].instance;

                // 停止処理
                Stop(effectId, obj);

                // プールに戻ったので Dequeue し直して使用
                obj = effectPool[effectId].Dequeue();
            }
            else {
                // 同じIDの使用中エフェクトが存在しない
                return null;
            }
        }

        obj.transform.position = position;
        obj.SetActive(true);
        activeEffectList.Add((effectId, obj));

        // 一定時間後にエフェクト停止
        if (autoReturn) {
            // EffectEntry から時間を取る
            var entry = System.Array.Find(effectEntries, e => e.id == effectId);
            float delay = entry != null && entry.autoReturnTime > 0 ? entry.autoReturnTime : 2f;
            _ = ReturnToPoolAfterTime(effectId, obj, delay);
        }

        return obj;
    }

    // 一定時間後に自動でプールへ戻す
    private async UniTaskVoid ReturnToPoolAfterTime(int effectId, GameObject obj, float delay) {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay));
        if (obj == null || obj.Equals(null)) return;
        Stop(effectId, obj);
    }

    // 指定のエフェクトを停止してプールへ戻す
    public void Stop(int effectId, GameObject obj) {
        if (obj == null) return;

        // 表示 OFF
        obj.SetActive(false);

        // 現在使用中リストから削除
        activeEffectList.RemoveAll(e => e.instance == obj);

        // プールへ戻す
        if (effectPool.ContainsKey(effectId)) {
            effectPool[effectId].Enqueue(obj);
        }
        else {
            Destroy(obj);
        }
    }

    // 再生中のすべてのエフェクトを停止してプールへ戻す
    public void StopAll() {
        foreach (var (effectName, obj) in activeEffectList) {
            if (obj == null || obj.Equals(null)) continue;
            obj.SetActive(false);

            if (effectPool.ContainsKey(effectName)) {
                effectPool[effectName].Enqueue(obj);
            }
            else {
                Destroy(obj);
            }
        }
        activeEffectList.Clear();
    }

}
