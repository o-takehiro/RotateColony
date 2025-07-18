using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ノーマルモードのステージ生成戦略（ゴールまでの有限ステージ）
/// </summary>
public class NormalStageMode : IStageGenerationStrategy {
    private readonly List<GameObject> stagePrefabs;
    private readonly GameObject goalPrefab;

    private bool goalSpawned = false;

    public NormalStageMode(List<GameObject> stagePrefabs, GameObject goalPrefab) {
        this.stagePrefabs = stagePrefabs;
        this.goalPrefab = goalPrefab;
    }

    public GameObject GetNextStagePrefab(int generatedCount, int maxSegments) {
        if (!goalSpawned && generatedCount >= maxSegments - 1) {
            goalSpawned = true;
            return goalPrefab;
        }

        if (goalSpawned && generatedCount >= maxSegments) {
            return null; // もう生成しない
        }

        int index = Random.Range(0, stagePrefabs.Count);
        return stagePrefabs[index];
    }

    public bool ShouldSpawnGoal(int generatedCount, int maxSegments) {
        return !goalSpawned && generatedCount >= maxSegments - 1;
    }
}