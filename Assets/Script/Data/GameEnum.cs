

/*
 * @file GameEnum.cs
 * @brief 列挙体定義
 * @author oorui
 */

/// <summary>
/// ゲームのパート
/// <summary>
public enum eGamePart {
    Invalid = -1,
    Standby,        // 待機
    Title,          // タイトル
    MainGame,       // メインゲーム
    Ending,         // エンディング
    Max,

}


/// <summary>
/// ゲームの経過状態
/// </summary>
public enum GameResultType {
    None,
    Clear,      // ゲームクリア
    GameOver    // ゲームオーバー
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