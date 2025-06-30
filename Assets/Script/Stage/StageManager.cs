using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SystemObject {
    [SerializeField] private Transform player;
    [SerializeField] private List<GameObject> stagePrefabs;
    [SerializeField] private GameObject goalPrefab;

    private const float _SEGMENT_LENGTH = 70f;
    private const int _INITIAL_SEGMETNS = 5;
    private const int _MAX_SEGMENTS = 20;

    private List<GameObject> activeSegments = new List<GameObject>();
    private float spawnZ = 40f;
    private int totalGeneratedCount = 0;
    private int passedStageCount = 0;
    public int PassedStageCount => passedStageCount;

    public static StageManager instance { get; private set; } = null;

    // ゴール到達イベント
    public event System.Action OnGoalReached;

    public override async UniTask Initialize() {
        instance = this;
        passedStageCount = 0;
        totalGeneratedCount = 0;
        spawnZ = 40f;

        for (int i = 0; i < _INITIAL_SEGMETNS; i++) {
            SpawnSegment();
        }

        await UniTask.CompletedTask;
    }

    private void Update() {
        if (player == null) return;

        CheckPassedSegments();
        TrySpawnNewSegment();
        UpdateActiveSegment();
    }

    /// <summary>
    /// 通過検知
    /// </summary>
    private void CheckPassedSegments() {
        foreach (var segment in activeSegments) {
            var script = segment.GetComponent<StageSegment>();
            if (script == null || script.hasPassed) continue;
            if (player.position.z <= segment.transform.position.z + _SEGMENT_LENGTH / 2f) continue;

            script.hasPassed = true;
            passedStageCount++;
            Debug.Log("通過したステージ数: " + passedStageCount);

            // ゴール判定
            if (segment.CompareTag("Goal")) {
                Debug.Log("ゴールに到達");
                OnGoalReached?.Invoke();
            }
        }
    }

    private void TrySpawnNewSegment() {
        if (player.position.z > spawnZ - _SEGMENT_LENGTH * (_INITIAL_SEGMETNS - 1)) {
            SpawnSegment();
            RemoveOldSegment();
        }
    }

    private void SpawnSegment() {
        if (totalGeneratedCount >= _MAX_SEGMENTS) return;

        GameObject prefab = (totalGeneratedCount == _MAX_SEGMENTS - 1)
            ? goalPrefab
            : stagePrefabs[Random.Range(0, stagePrefabs.Count)];

        float randomX = Random.Range(-90f, 90f);
        GameObject segment = Instantiate(
            prefab,
            new Vector3(0, 5f, spawnZ),
            Quaternion.Euler(randomX, 90f, 90f)
        );

        activeSegments.Add(segment);
        spawnZ += _SEGMENT_LENGTH;
        totalGeneratedCount++;
    }

    private void RemoveOldSegment() {
        if (activeSegments.Count > _INITIAL_SEGMETNS) {
            GameObject oldSegment = activeSegments[0];
            activeSegments.RemoveAt(0);
            Destroy(oldSegment);
        }
    }

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
            StageSegment script = segment.GetComponent<StageSegment>();
            if (script != null) {
                script.EnableRotation(segment == nearest);
            }
        }
    }

    public void SetPlayer(Transform _player) {
        player = _player;
    }

    public void ClearAllSegments() {
        foreach (GameObject segment in activeSegments) {
            if (segment != null) {
                Destroy(segment);
            }
        }

        activeSegments.Clear();
        spawnZ = 50f;
        passedStageCount = 0;
        totalGeneratedCount = 0;
    }

    public List<StageSegment> AllSegments {
        get {
            List<StageSegment> segments = new List<StageSegment>();
            foreach (var obj in activeSegments) {
                var script = obj.GetComponent<StageSegment>();
                if (script != null) {
                    segments.Add(script);
                }
            }
            return segments;
        }
    }

}