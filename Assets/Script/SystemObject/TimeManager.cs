/*
 *  @file   TimeManager
 *  @author oorui
 */

using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 時間を管理するクラス
/// </summary>
public class TimeManager : SystemObject {

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await UniTask.CompletedTask;
    }

    public static TimeManager Instance { get; private set; }    // 自身の取得

    private float _time;                // 時間用フラグ
    private bool _isRunning = false;    // 動いているかどうか

    /// <summary>
    /// Update前初期化
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;

        }
        else {
            // 消す
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 更新
    /// </summary>
    private void Update() {
        if (_isRunning) {
            // 経過時間を更新
            _time += Time.deltaTime;
        }
    }
    /// <summary>
    /// ゲーム開始から終わりまでの時間を図る
    /// </summary>
    public void StartTimer() {
        _time = 0f;
        _isRunning = true;
    }

    /// <summary>
    /// 時間を止める
    /// </summary>
    public void StopTimer() {
        _isRunning = false;
    }

    /// <summary>
    /// ゲッター
    /// 時間の取得
    /// </summary>
    /// <returns></returns>
    public float GetTime() => _time;
}
