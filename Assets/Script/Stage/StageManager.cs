using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージの生成と管理を担当するクラス
/// </summary>
public class StageManager : SystemObject {
    [SerializeField] private Transform player;
    [SerializeField] private List<GameObject> stagePrefabs;
    [SerializeField] private GameObject goalPrefab;

    private const float SEGMENT_LENGTH = 70f;
    private const int INITIAL_SEGMENTS = 5;
    private const int MAX_SEGMENTS = 10;

    private readonly List<GameObject> activeSegments = new List<GameObject>();
    private float spawnZ = 40f;
    private int totalGeneratedCount = 0;
    private int passedStageCount = 0;

    public int PassedStageCount => passedStageCount;
    public static StageManager instance { get; private set; } = null;

    public event System.Action OnGoalReached;

    private IStageGenerationStrategy stageGenerationStrategy;
    public GameModeState CurrentMode { get; private set; } = GameModeState.Normal;

    /// <summary>
    /// 初期化（モードに応じたステージ生成戦略を設定）
    /// </summary>
    public override async UniTask Initialize() {
        instance = this;

        // デフォルトではノーマルモード
        SetupStrategy(GameModeState.Normal);

        passedStageCount = 0;
        totalGeneratedCount = 0;
        spawnZ = 40f;

        // 初期ステージ生成
        for (int i = 0; i < INITIAL_SEGMENTS; i++) {
            SpawnSegment();
        }

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// モードに応じて、ステージの生成方法を変更する
    /// </summary>
    public void SetupStrategy(GameModeState mode) {
        CurrentMode = mode;
        switch (mode) {
            case GameModeState.Normal:
                stageGenerationStrategy = new NormalStageMode(stagePrefabs, goalPrefab);
                break;
            case GameModeState.Endless:
            default:
                stageGenerationStrategy = new EndlessStageMode(stagePrefabs);
                break;
        }
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update() {
        if (player == null) return;

        CheckPassedSegments();
        TrySpawnNewSegment();
        UpdateActiveSegment();
    }

    /// <summary>
    /// プレイヤーが通過したステージを検知
    /// </summary>
    private void CheckPassedSegments() {
        foreach (var segment in activeSegments) {
            var script = segment.GetComponent<StageSegment>();
            if (script == null || script.hasPassed) continue;
            if (player.position.z <= segment.transform.position.z + SEGMENT_LENGTH / 2f) continue;

            script.hasPassed = true;
            passedStageCount++;

            if (segment.CompareTag("Goal")) {
                OnGoalReached?.Invoke();
            }
        }
    }

    /// <summary>
    /// プレイヤーの進行に応じて新しいステージを生成
    /// </summary>
    private void TrySpawnNewSegment() {
        if (player.position.z > spawnZ - SEGMENT_LENGTH * (INITIAL_SEGMENTS - 1)) {
            SpawnSegment();
            RemoveOldSegment();
        }
    }

    /// <summary>
    /// ステージの生成処理
    /// </summary>
    private void SpawnSegment() {
        if (stageGenerationStrategy == null) return;

        GameObject prefab = stageGenerationStrategy.GetNextStagePrefab(totalGeneratedCount, MAX_SEGMENTS);
        if (prefab == null) return; // ゴール生成後はnullになる

        float randomX = Random.Range(-90f, 90f);
        GameObject segment = Instantiate(
            prefab,
            new Vector3(0, 5f, spawnZ),
            Quaternion.Euler(randomX, 90f, 90f)
        );

        activeSegments.Add(segment);
        spawnZ += SEGMENT_LENGTH;
        totalGeneratedCount++;
    }

    /// <summary>
    /// 古いセグメントを破棄
    /// </summary>
    private void RemoveOldSegment() {
        if (activeSegments.Count > INITIAL_SEGMENTS) {
            GameObject old = activeSegments[0];
            activeSegments.RemoveAt(0);
            Destroy(old);
        }
    }

    /// <summary>
    /// プレイヤーに一番近いステージの処理を更新
    /// </summary>
    private void UpdateActiveSegment() {
        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject segment in activeSegments) {
            float distance = Mathf.Abs(player.position.z - segment.transform.position.z);
            if (distance < minDistance) {
                minDistance = distance;
                nearest = segment;
            }
        }

        foreach (GameObject segment in activeSegments) {
            var script = segment.GetComponent<StageSegment>();
            if (script != null) {
                script.EnableRotation(segment == nearest);
            }
        }
    }

    public void SetPlayer(Transform _player) => player = _player;

    public void ClearAllSegments() {
        foreach (GameObject segment in activeSegments) {
            if (segment != null) Destroy(segment);
        }

        activeSegments.Clear();
        spawnZ = 40f;
        totalGeneratedCount = 0;
        passedStageCount = 0;
    }

    public List<StageSegment> AllSegments {
        get {
            List<StageSegment> list = new();
            foreach (var obj in activeSegments) {
                var script = obj.GetComponent<StageSegment>();
                if (script != null) list.Add(script);
            }
            return list;
        }
    }
}