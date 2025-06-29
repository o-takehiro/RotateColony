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