using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// エフェクトをゲーム全体で共通管理するマネージャ
/// プール方式で再利用することで処理を軽くする
/// </summary>
public class EffectManager : SystemObject {
    // 唯一のインスタンスを保持する
    public static EffectManager Instance { get; private set; }

    [System.Serializable]
    public class EffectEntry {
        public string name;                // 識別用の名前
        public GameObject prefab;          // 実際のエフェクトのひな型
        public int initialPoolSize = 5;    // あらかじめ確保しておく数
        public float autoReturnTime = -1f; // プールに戻すタイミング
    }

    [SerializeField]
    private EffectEntry[] effectEntries; // インスペクタで設定するリスト

    [SerializeField]
    private Transform poolRoot; // プールをまとめる親のオブジェクト

    // 名前とひな型の対応表
    private readonly Dictionary<string, GameObject> effectPrefabs = new Dictionary<string, GameObject>();

    // 名前とプールの対応表
    private readonly Dictionary<string, Queue<GameObject>> effectPools = new Dictionary<string, Queue<GameObject>>();

    // 現在動いているエフェクトを記録するリスト
    private readonly List<(string effectName, GameObject obj)> activeEffects = new List<(string, GameObject)>();

    private void Awake() {
        // インスタンスが重複していたら削除する
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // ゲーム全体で保持する

        // 親が設定されていなければ自動で作成する
        if (poolRoot == null) {
            var poolRootObj = new GameObject("EffectPoolRoot");
            poolRootObj.transform.SetParent(transform);
            poolRoot = poolRootObj.transform;
        }
    }

    public override async UniTask Initialize() {
        // プールの初期化
        InitializePools();
        await UniTask.CompletedTask;
    }

    // エフェクトリストをもとにプールを作成する
    private void InitializePools() {
        foreach (var entry in effectEntries) {
            if (string.IsNullOrEmpty(entry.name) || entry.prefab == null) {
                continue;
            }

            effectPrefabs[entry.name] = entry.prefab;

            // すでにプールがあるなら作らない
            if (effectPools.ContainsKey(entry.name)) continue;

            var queue = new Queue<GameObject>();
            var effectParent = new GameObject($"Pool_{entry.name}");
            effectParent.transform.SetParent(poolRoot);

            // あらかじめ指定された数だけ生成して無効化しておく
            for (int i = 0; i < entry.initialPoolSize; i++) {
                var obj = Instantiate(entry.prefab, effectParent.transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            effectPools[entry.name] = queue;
        }
    }

    // エフェクトを指定位置で再生する
    public GameObject Play(string effectName, Vector3 position, bool autoReturn = true) {
        if (!effectPools.ContainsKey(effectName)) {
            return null;
        }

        GameObject obj;
        // プールから取り出す
        if (effectPools[effectName].Count > 0) {
            obj = effectPools[effectName].Dequeue();
        }
        else {
            // 新しく生成する
            obj = Instantiate(effectPrefabs[effectName], poolRoot);
        }

        obj.transform.position = position;
        obj.SetActive(true);
        activeEffects.Add((effectName, obj));

        // 一定時間後にエフェクト停止
        if (autoReturn) {
            // EffectEntry から時間を取る
            var entry = System.Array.Find(effectEntries, e => e.name == effectName);
            float delay = entry != null && entry.autoReturnTime > 0 ? entry.autoReturnTime : 2f; // 2秒をデフォルト
            _ = ReturnToPoolAfterTime(effectName, obj, delay);
        }

        return obj;
    }

    // 一定時間後に自動でプールへ戻す
    private async UniTaskVoid ReturnToPoolAfterTime(string effectName, GameObject obj, float delay) {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay));
        if (obj == null || obj.Equals(null)) return;
        Stop(effectName, obj);
    }

    // 指定のエフェクトを停止してプールへ戻す
    public void Stop(string effectName, GameObject obj) {
        if (obj == null) return;
        obj.SetActive(false);
        activeEffects.RemoveAll(e => e.obj == obj);

        if (effectPools.ContainsKey(effectName)) {
            effectPools[effectName].Enqueue(obj);
        }
        else {
            Destroy(obj);
        }
    }

    // 再生中のすべてのエフェクトを停止してプールへ戻す
    public void StopAll() {
        foreach (var (effectName, obj) in activeEffects) {
            if (obj == null || obj.Equals(null)) continue;
            obj.SetActive(false);

            if (effectPools.ContainsKey(effectName)) {
                effectPools[effectName].Enqueue(obj);
            }
            else {
                Destroy(obj);
            }
        }
        activeEffects.Clear();
    }
}
