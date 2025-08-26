using UnityEngine;

/// <summary>
/// ランク計算の共通インターフェース
/// </summary>
public abstract class RankCalculatorBase {
    /// <summary>
    /// ランクを計算して返す
    /// </summary>
    public abstract string CalculateRank(int stageCount, float timeInSeconds);

    /// <summary>
    /// ステージスコア計算
    /// </summary>
    protected int CalculateStageScore(int stageCount) {
        return Mathf.Min(stageCount * 5, 50); // 10ステージ以上で50点満点
    }

    /// <summary>
    /// ステージスコア計算
    /// </summary>
    protected int EndlessStageScore(int stageCount) {
        return Mathf.Min(stageCount * 5, 150); // 30ステージ以上で50点満点
    }

    /// <summary>
    /// ノーマルモードのランク
    /// </summary>
    protected string ScoreToRank(int totalScore) {
        if (totalScore >= 100) return "S";
        if (totalScore >= 80) return "A";
        if (totalScore >= 50) return "B";
        if (totalScore >= 30) return "C";
        return "D";
    }

    /// <summary>
    /// エンドレスモードのランク
    /// </summary>
    protected string EndlessToScoreRank (int totalScore) {
        if (totalScore >= 200) return "S";
        if (totalScore >= 140) return "A";
        if (totalScore >= 80) return "B";
        if (totalScore >= 20) return "C";
        return "D";
    }
}

/// <summary>
/// ステージクリアモードのランク計算
/// クリア時間が短いほどランクを高く設定する
/// </summary>
public class StageClearRankCalculator : RankCalculatorBase {
    public override string CalculateRank(int stageCount, float timeInSeconds) {
        int stageScore = CalculateStageScore(stageCount);
        int timeScore = CalculateTimeScore(timeInSeconds);
        int totalScore = stageScore + timeScore;

        return ScoreToRank(totalScore);
    }

    /// <summary>
    /// 時間スコア
    /// 短ければ短いほどスコアを上げる
    /// </summary>
    private int CalculateTimeScore(float time) {
        if (time <= 60f) return 50;
        if (time <= 90f) return 40;
        if (time <= 120f) return 30;
        if (time <= 180f) return 20;
        if (time <= 240f) return 10;
        return 0;
    }
}

/// <summary>
/// エンドレスモードのランク計算
/// 生存時間が長いほどスコアを高くする
/// </summary>
public class EndlessModeRankCalculator : RankCalculatorBase {
    public override string CalculateRank(int stageCount, float timeInSeconds) {
        int stageScore = EndlessStageScore(stageCount);
        int timeScore = CalculateTimeScore(timeInSeconds);
        int totalScore = stageScore + timeScore;

        return EndlessToScoreRank(totalScore);
    }

    /// <summary>
    /// 時間スコア
    /// 長ければ長いほどスコアを高くする
    /// </summary>
    private int CalculateTimeScore(float time) {
        if (time >= 180f) return 50;
        if (time >= 120f) return 40;
        if (time >= 90f) return 30;
        if (time >= 40f) return 20;
        if (time >= 20f) return 10;
        return 0;
    }
}