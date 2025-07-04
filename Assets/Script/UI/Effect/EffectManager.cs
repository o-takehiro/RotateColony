using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// プール型の汎用エフェクトマネージャ
/// </summary>
public class EffectManager : SystemObject {
    // シングルトン
    public static EffectManager Instance { get; private set; }

    [System.Serializable]
    public class EffectEntry {
        public string name;              // 識別用のエフェクト名
        public GameObject prefab;        // エフェクトのプレハブ
        public int initialPoolSize = 5;  // プールに確保する初期個数
    }

    [SerializeField]
    private EffectEntry[] effectEntries;

    // エフェクト名→Prefab
    private readonly Dictionary<string, GameObject> effectPrefabs = new Dictionary<string, GameObject>();

    // エフェクト名→プール
    private readonly Dictionary<string, Queue<GameObject>> effectPools = new Dictionary<string, Queue<GameObject>>();

    private void Awake() {
        // シングルトンの重複防止
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// SystemObjectの抽象メソッドをオーバーライド
    /// </summary>
    public override async UniTask Initialize() {
        InitializePools();
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 登録済みのエフェクトをもとにプールを構築
    /// </summary>
    private void InitializePools() {
        foreach (var entry in effectEntries) {
            if (string.IsNullOrEmpty(entry.name) || entry.prefab == null) {
                continue;
            }

            effectPrefabs[entry.name] = entry.prefab;

            var queue = new Queue<GameObject>();

            for (int i = 0; i < entry.initialPoolSize; i++) {
                var obj = Instantiate(entry.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            effectPools[entry.name] = queue;
        }
    }

    /// <summary>
    /// エフェクトを再生
    /// </summary>
    /// <param name="effectName">名前</param>
    /// <param name="position">座標</param>
    /// <param name="autoReturn">trueなら一定時間後に戻す</param>
    /// <returns></returns>
    public GameObject Play(string effectName, Vector3 position, bool autoReturn = true) {
        if (!effectPools.ContainsKey(effectName)) {
            return null;
        }

        GameObject obj;

        if (effectPools[effectName].Count > 0) {
            obj = effectPools[effectName].Dequeue();
        }
        else {
            obj = Instantiate(effectPrefabs[effectName]);
        }

        obj.transform.position = position;
        obj.SetActive(true);

        if (autoReturn) {
            // 2秒後に自動でプールに戻す
            _ = ReturnToPoolAfterTime(effectName, obj, 2f);
        }

        return obj;
    }

    /// <summary>
    /// 一定時間後に自動でプールに戻す
    /// </summary>
    private async UniTaskVoid ReturnToPoolAfterTime(string effectName, GameObject obj, float delay) {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay));

        if (obj == null || obj.Equals(null)) return;
        if (!effectPools.ContainsKey(effectName)) {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        effectPools[effectName].Enqueue(obj);
    }

    /// <summary>
    /// 外部から明示的に停止
    /// </summary>
    public void Stop(string effectName, GameObject obj) {
        obj.SetActive(false);

        if (effectPools.ContainsKey(effectName)) {
            effectPools[effectName].Enqueue(obj);
        }
        else {
            Destroy(obj);
        }
    }
}