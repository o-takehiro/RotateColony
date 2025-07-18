using UnityEngine;
/// <summary>
/// モードごとのインターフェース
/// </summary>
public interface IStageGenerationStrategy {
    GameObject GetNextStagePrefab(int generatedCount, int maxSegments);
    bool ShouldSpawnGoal(int generatedCount, int maxSegments);
}
