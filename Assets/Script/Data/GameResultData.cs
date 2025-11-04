/*
 *  @file   GameResultData.cs
 *  @autor oorui
 */

/// <summary>
/// リザルト時の詳細
/// </summary>
public static class GameResultData {

    /// <summary>
    /// ゲームのクリア状況
    /// </summary>
    public static GameResultType ResultType { get; set; }

    /// <summary>
    /// 通過したオブジェクトの数
    /// </summary>
    public static int StagePassedCount { get; set; }

    /// <summary>
    /// ゲームのクリアタイム
    /// </summary>
    public static float ClearTime { get; set; }

    /// <summary>
    /// 選択されたゲームモード
    /// </summary>
    public static GameModeState SelectedMode { get; set; }

}