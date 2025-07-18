/**
 * @file GameEnum.cs
 * @brief 列挙体定義
 */

// ゲームのパート
public enum eGamePart {
    Invalid = -1,
    Standby,
    Title,
    MainGame,
    Ending,
    Max,

}


public enum eGameEndReason {
    Invalid = -1,
    Dead,
    Clear,
}


public enum GameResultType {
    Clear,
    GameOver
}


// タップ状態を段階管理するための列挙体
public enum ResultState {
    None,
    ShowImage,  // 画像のみ表示中
    ShowRank,   // ランクなどの情報表示中
    Finish      // 終了フェーズ
}

/// <summary>
/// ゲーム開始時に選ぶモード
/// </summary>
public enum GameModeState {
    Invalid,
    Endless,    // エンドレスモード
    Normal      // ノーマルモード
}