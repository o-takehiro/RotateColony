using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// プール型の汎用エフェクトマネージャ
/// ・エフェクトの再利用によるパフォーマンス向上
/// ・エフェクトの再生・停止・一括停止を制御
/// </summary>
public class EffectManager : SystemObject {
    // シングルトンインスタンス
    public static EffectManager Instance { get; private set; }

    [System.Serializable]
    public class EffectEntry {
        public string name;              // 識別用のエフェクト名
        public GameObject prefab;        // エフェクトのプレハブ
        public int initialPoolSize = 5;  // プールに確保する初期個数
    }

    [SerializeField]
    private EffectEntry[] effectEntries; // インスペクタで設定するエフェクトリスト

    // エフェクト名 → プレハブ
    private readonly Dictionary<string, GameObject> effectPrefabs = new Dictionary<string, GameObject>();

    // エフェクト名 → 非アクティブなエフェクトのプール
    private readonly Dictionary<string, Queue<GameObject>> effectPools = new Dictionary<string, Queue<GameObject>>();

    // 現在アクティブな（再生中の）エフェクト一覧
    private readonly List<(string effectName, GameObject obj)> activeEffects = new List<(string, GameObject)>();

    private void Awake() {
        // シングルトン重複防止
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 初期化処理（SystemObjectの抽象メソッドの実装）
    /// </summary>
    public override async UniTask Initialize() {
        InitializePools();
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 登録されたエフェクトをもとにオブジェクトプールを構築
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
    /// <param name="effectName">識別名</param>
    /// <param name="position">再生位置</param>
    /// <param name="autoReturn">自動停止するか</param>
    public GameObject Play(string effectName, Vector3 position, bool autoReturn = true) {
        if (!effectPools.ContainsKey(effectName)) {
            return null;
        }

        GameObject obj;

        // プールから取得または新規生成
        if (effectPools[effectName].Count > 0) {
            obj = effectPools[effectName].Dequeue();
        }
        else {
            obj = Instantiate(effectPrefabs[effectName]);
        }

        obj.transform.position = position;
        obj.SetActive(true);

        // 再生中リストに登録
        activeEffects.Add((effectName, obj));

        // 一定時間後に自動で停止
        if (autoReturn) {
            _ = ReturnToPoolAfterTime(effectName, obj, 2f);
        }

        return obj;
    }

    /// <summary>
    /// 指定時間経過後にエフェクトを停止してプールに戻す
    /// </summary>
    private async UniTaskVoid ReturnToPoolAfterTime(string effectName, GameObject obj, float delay) {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay));

        if (obj == null || obj.Equals(null)) return;

        Stop(effectName, obj);
    }

    /// <summary>
    /// 指定したエフェクトを明示的に停止
    /// </summary>
    /// <param name="effectName">識別名</param>
    /// <param name="obj">停止対象のオブジェクト</param>
    public void Stop(string effectName, GameObject obj) {
        obj.SetActive(false);

        // 再生中リストから削除
        activeEffects.RemoveAll(e => e.obj == obj);

        // プールに戻す or 破棄
        if (effectPools.ContainsKey(effectName)) {
            effectPools[effectName].Enqueue(obj);
        }
        else {
            Destroy(obj);
        }
    }

    /// <summary>
    /// 再生中のすべてのエフェクトを停止し、プールに戻す
    /// </summary>
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

        // 再生中リストをクリア
        activeEffects.Clear();
    }
}