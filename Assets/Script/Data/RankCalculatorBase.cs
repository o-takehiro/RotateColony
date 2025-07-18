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
    /// ランクの文字列を表示する
    /// </summary>
    protected string ScoreToRank(int totalScore) {
        if (totalScore >= 90) return "S";
        if (totalScore >= 70) return "A";
        if (totalScore >= 50) return "B";
        if (totalScore >= 30) return "C";
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
        int stageScore = CalculateStageScore(stageCount);
        int timeScore = CalculateTimeScore(timeInSeconds);
        int totalScore = stageScore + timeScore;

        return ScoreToRank(totalScore);
    }

    /// <summary>
    /// 時間スコア
    /// 長ければ長いほどスコアを高くする
    /// </summary>
    private int CalculateTimeScore(float time) {
        if (time >= 300f) return 50;
        if (time >= 240f) return 40;
        if (time >= 180f) return 30;
        if (time >= 120f) return 20;
        if (time >= 60f) return 10;
        return 0;
    }
}