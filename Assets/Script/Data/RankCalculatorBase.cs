/*
 *  @file   RankCalculatorBase.cs
 *  @author oorui
 */

using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// リザルト時のスコア計算
/// </summary>
public abstract class RankCalculatorBase {

    private const int _STAGEPOINT = 5;              // ステージ通過時のポイント
    private const int _STAGEPOINTMAX_NORMAL = 50;   // ノーマルステージのステージ通過最大ポイント   
    private const int _STAGEPOINTMAX_ENDLESS = 150; // エンドレスステージのステージ通過最大ポイント   

    /// <summary>
    /// ランクを計算して返す
    /// </summary>
    public abstract string CalculateRank(int stageCount, float timeInSeconds);

    /// <summary>
    /// ステージスコア計算
    /// </summary>
    protected int CalculateStageScore(int stageCount) {
        return Mathf.Min(stageCount * _STAGEPOINT, _STAGEPOINTMAX_NORMAL); // 10ステージ以上で50点満点
    }

    /// <summary>
    /// ステージスコア計算
    /// </summary>
    protected int EndlessStageScore(int stageCount) {
        return Mathf.Min(stageCount * _STAGEPOINT, _STAGEPOINTMAX_ENDLESS); // 30ステージ以上で50点満点
    }

    /// <summary>
    /// ノーマルモードのランク
    /// </summary>
    protected string ScoreToRank(int totalScore) {
        // マスターデータから取得
        var scoreData = MasterDataManager.ScoreData[0];

        // スコアを上から順に見る
        foreach (var data in scoreData) {
            // 点数に合ったランクを返す
            if (totalScore >= data.totalScore) {
                return data.text;
            }
        }

        return "D";
    }

    /// <summary>
    /// エンドレスモードのランク
    /// </summary>
    protected string EndlessToScoreRank(int totalScore) {
        // マスターデーターから取得
        var scoreData = MasterDataManager.ScoreData[1];

        // スコアを上から順に見る
        foreach (var data in scoreData) {
            // 点数に合ったランクを返す
            if (totalScore >= data.totalScore) {
                return data.text;
            }
        }
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
        // マスターデーターから取得
        var timeScore = MasterDataManager.TimeScore[0];

        // データを上から順に見る
        foreach (var data in timeScore) {
            // クリアタイムに応じてスコアを返す
            if (time <= data.time) {
                // 結果に応じたスコアを返す
                return data.score;
            }
        }
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
        // マスターデーターから取得
        var timeScore = MasterDataManager.TimeScore[1];

        foreach (var data in timeScore) {
            // 経過時間に応じてスコアを返す
            if (time >= data.time) {
                // 結果に応じたスコアを返す
                return data.score;
            }
        }

        return 0;
    }
}