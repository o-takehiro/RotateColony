/*
 *  @file    StageManager.cs
 *  @author  oorui
 */

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using static GameConst;
/// <summary>
/// ステージ全体の管理クラス
/// </summary>
public class StageManager : SystemObject {
    [SerializeField] private Transform player;                  // プレイヤーの位置
    [SerializeField] private List<GameObject> stagePrefabs;     // ステージのPrefabリスト
    [SerializeField] private GameObject goalPrefab;             // ゴールステージのPrefab

    private const float SEGMENT_LENGTH = 70f;                   // 1ステージあたりの長さ
    private const int INITIAL_SEGMENTS = 5;                     // 初期に生成するステージ数
    private const int MAX_SEGMENTS = 10;                        // 最大で同時に保持するステージ数

    private readonly List<GameObject> activeSegments = new List<GameObject>(); // 現在シーン上に存在するステージオブジェクトのリスト
    private float spawnZ = 40f;                                 // 次のステージを生成するZ座標
    private int totalGeneratedCount = 0;                        // これまで生成されたステージの総数
    private int passedStageCount = 0;                           // プレイヤーが通過したステージ数

    public int PassedStageCount => passedStageCount;            // 通過ステージ数を外部から参照可能にするプロパティ
    public static StageManager instance { get; private set; } = null; // シングルトンインスタンス

    public event System.Action OnGoalReached;                   // ゴールに到達した際に通知されるイベント

    private IStageGenerationStrategy stageGenerationStrategy;   // ステージの生成方式を決めるクラス
    public GameModeState CurrentMode { get; private set; } = GameModeState.Normal; // 現在のゲームモード

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override async UniTask Initialize() {
        instance = this; // 自身をシングルトンとして登録

        // 初期はノーマルモードを設定
        SetupStrategy(GameModeState.Normal);

        // 各種カウンタ-初期化
        passedStageCount = 0;
        totalGeneratedCount = 0;
        spawnZ = 40f;

        // 初期ステージを複数生成
        for (int i = 0; i < INITIAL_SEGMENTS; i++) {
            // ステージ生成
            SpawnSegment();
        }

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// ゲームモードに応じたステージ生成を設定する
    /// </summary>
    public void SetupStrategy(GameModeState mode) {
        if (mode == GameModeState.Invalid) return;
        CurrentMode = mode; // 現在のモードを記録

        switch (mode) {
            case GameModeState.Normal:
                // ノーマルモード用のステージ生成
                stageGenerationStrategy = new NormalStageMode(stagePrefabs, goalPrefab);
                break;
            case GameModeState.Endless:
                // エンドレスモード用のステージ生成
                stageGenerationStrategy = new EndlessStageMode(stagePrefabs);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update() {
        if (player == null) return; // プレイヤーが未設定なら処理しない

        CheckPassedSegments();   // 通過済みステージをチェック
        TrySpawnNewSegment();    // 必要に応じて新しいステージを生成
        UpdateActiveSegment();   // プレイヤーに最も近いステージを更新（回転制御など）
    }

    /// <summary>
    /// プレイヤーが通過したステージを検出する
    /// </summary>
    private void CheckPassedSegments() {
        foreach (var segment in activeSegments) {
            var script = segment.GetComponent<StageSegment>(); // ステージのスクリプトを取得
            if (script == null || script.hasPassed) continue;  // すでに通過済みならスキップ
            if (player.position.z <= segment.transform.position.z + SEGMENT_LENGTH / 2f) continue; // まだ通過していない場合

            script.hasPassed = true;  // 通過フラグを立てる
            passedStageCount++;       // 通過カウントを増やす

            // ゴールステージならイベント
            if (segment.CompareTag(_GOAL_TAG)) {
                OnGoalReached?.Invoke();
            }
        }
    }

    /// <summary>
    /// プレイヤーの進行に応じて新たなステージを生成する
    /// </summary>
    private void TrySpawnNewSegment() {
        // プレイヤーが一定位置を超えたら
        if (player.position.z > spawnZ - SEGMENT_LENGTH * (INITIAL_SEGMENTS - 1)) {
            // 次のステージを生成
            SpawnSegment();
            // 古いステージを削除
            RemoveOldSegment();
        }
    }

    /// <summary>
    /// 新しいステージを生成する
    /// </summary>
    private void SpawnSegment() {
        if (stageGenerationStrategy == null) return;

        // 次のPrefabを取得
        GameObject prefab = stageGenerationStrategy.GetNextStagePrefab(totalGeneratedCount, MAX_SEGMENTS);
        if (prefab == null) return;

        // ステージの傾きをランダムに設定
        float randomX = Random.Range(-90f, 90f);

        // ステージPrefabを指定位置に生成
        GameObject segment = Instantiate(
            prefab,
            new Vector3(0, 5f, spawnZ),
            Quaternion.Euler(randomX, 90f, 90f)
        );

        // リストに追加して管理
        activeSegments.Add(segment);
        spawnZ += SEGMENT_LENGTH;   // 次の生成位置を更新
        totalGeneratedCount++;      // 総生成数をカウント
    }

    /// <summary>
    /// 古いステージを削除
    /// </summary>
    private void RemoveOldSegment() {
        if (activeSegments.Count > INITIAL_SEGMENTS) {
            GameObject old = activeSegments[0]; // 一番古いステージを取得
            activeSegments.RemoveAt(0);         // リストから除外
            Destroy(old);                       // ゲームオブジェクトを破棄
        }
    }

    /// <summary>
    /// プレイヤーに一番近い処理を実行
    /// </summary>
    private void UpdateActiveSegment() {
        GameObject nearest = null;              // 最も近いステージを保持する変数
        float minDistance = float.MaxValue;     // 距離の初期値を最大値に設定

        // 生成された全ステージをみて、一番近いものを探す
        foreach (GameObject segment in activeSegments) {
            float distance = Mathf.Abs(player.position.z - segment.transform.position.z);
            if (distance < minDistance) {
                minDistance = distance;
                nearest = segment;
            }
        }

        // 一番近いステージのみ、回転可能にする
        foreach (GameObject segment in activeSegments) {
            var script = segment.GetComponent<StageSegment>();
            if (script != null) {
                script.EnableRotation(segment == nearest);
            }
        }
    }

    /// <summary>
    /// プレイヤーの参照を外部から設定する
    /// </summary>
    public void SetPlayer(Transform _player) => player = _player;

    /// <summary>
    /// すべてのステージを削除
    /// </summary>
    public void ClearAllSegments() {
        foreach (GameObject segment in activeSegments) {
            // ステージオブジェクトを削除
            if (segment != null) Destroy(segment);
        }

        activeSegments.Clear(); // リストをクリア
        spawnZ = 40f;           // 生成位置を初期化
        totalGeneratedCount = 0;
        passedStageCount = 0;
    }

    /// <summary>
    /// 現在存在するステージを取得
    /// </summary>
    public List<StageSegment> AllSegments {
        get {
            List<StageSegment> list = new(); // 戻り値用リストを作成
            foreach (var obj in activeSegments) {
                var script = obj.GetComponent<StageSegment>();
                if (script != null) list.Add(script); // スクリプトがあるものだけ追加
            }
            return list;
        }
    }
}