/*
 *  @file   NormalStageMode.cs
 *  @author oorui
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ノーマルモードのステージ生成
/// </summary>
public class NormalStageMode : IStageGenerationStrategy {
    private readonly List<GameObject> stagePrefabs;     // 生成されたステージのプレファブリスト
    private readonly GameObject goalPrefab;             // 最後に生成されるゴールステージのプレファブ

    private bool goalSpawned = false;                   // ゴールが出現したかどうか
    
    /// <summary>
    /// ノーマルモード
    /// </summary>
    /// <param name="stagePrefabs"></param>
    /// <param name="goalPrefab"></param>
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