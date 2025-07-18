using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// エンドレスモードのステージ生成戦略
/// </summary>
public class EndlessStageMode : IStageGenerationStrategy {
    private readonly List<GameObject> stagePrefabs;

    public EndlessStageMode(List<GameObject> stagePrefabs) {
        this.stagePrefabs = stagePrefabs;
    }

    public GameObject GetNextStagePrefab(int generatedCount, int maxSegments) {
        return stagePrefabs[Random.Range(0, stagePrefabs.Count)];
    }

    /// <summary>
    /// エンドレスモード
    /// ゴールは出現しない
    /// </summary>
    /// <param name="generatedCount"></param>
    /// <param name="maxSegments"></param>
    /// <returns></returns>
    public bool ShouldSpawnGoal(int generatedCount, int maxSegments) {
        return false;
    }
}