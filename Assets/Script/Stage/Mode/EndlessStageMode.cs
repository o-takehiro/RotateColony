/*
 *  @file   EndlessStageMode.cs
 *  @author oorui
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// エンドレスモードのステージ生成
/// </summary>
public class EndlessStageMode : IStageGenerationStrategy {
    private readonly List<GameObject> stagePrefabs;

    /// <summary>
    /// エンドレスモード
    /// </summary>
    /// <param name="stagePrefabs"></param>
    public EndlessStageMode(List<GameObject> stagePrefabs) {
        this.stagePrefabs = stagePrefabs;
    }

    /// <summary>
    /// 次のステージのプレファブを取得
    /// </summary>
    /// <param name="generatedCount"></param>
    /// <param name="maxSegments"></param>
    /// <returns></returns>
    public GameObject GetNextStagePrefab(int generatedCount, int maxSegments) {
        return stagePrefabs[Random.Range(0, stagePrefabs.Count)];
    }

    /// <summary>
    /// ゴールは出現しない
    /// </summary>
    /// <param name="generatedCount"></param>
    /// <param name="maxSegments"></param>
    /// <returns></returns>
    public bool ShouldSpawnGoal(int generatedCount, int maxSegments) {
        return false;
    }
}