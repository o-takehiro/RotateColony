using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ノーマルモードのステージ生成戦略
/// </summary>
public class NormalStageMode : IStageGenerationStrategy {
    private readonly List<GameObject> stagePrefabs;
    private readonly GameObject goalPrefab;

    public NormalStageMode(List<GameObject> stagePrefabs, GameObject goalPrefab) {
        this.stagePrefabs = stagePrefabs;
        this.goalPrefab = goalPrefab;
    }

    public GameObject GetNextStagePrefab(int generatedCount, int maxSegments) {
        if (generatedCount == maxSegments - 1) {
            return goalPrefab;
        }
        return stagePrefabs[Random.Range(0, stagePrefabs.Count)];
    }
    
    /// <summary>
    /// ノーマルモード
    /// ゴールを生成する
    /// </summary>
    /// <param name="generatedCount"></param>
    /// <param name="maxSegments"></param>
    /// <returns></returns>
    public bool ShouldSpawnGoal(int generatedCount, int maxSegments) {
        return generatedCount == maxSegments - 1;
    }
}