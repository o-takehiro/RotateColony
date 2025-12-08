/*
 *  @file   Prihectile.cs
 *  @author oorui
 */

using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 弾の動作
/// ベジエ曲線でターゲットに向かって飛ぶ
/// </summary>
public class Projectile : MonoBehaviour {
    [Header("Projectile Settings")]
    public int damage = 10;                              // 与えるダメージ
    [SerializeField] private TrailRenderer trail;        // 軌跡エフェクト
    [SerializeField] private GameObject visual;          // 弾の見た目部分（非表示で再利用する）

    // 曲線を作る際の調整用定数
    private const float _MID_OFFSET_AMOUNT = 8f;         // 中間点の横方向オフセット
    private const float _ACCELERATION_PULL_AMOUNT = 9f;  // 前方向への引っ張り量
    private const float _VERTICAL_CURVE_AMOUNT = 9f;     // 上下方向の揺らぎ量

    // 飛行制御用の座標
    private Vector3 _startPos;
    private Vector3 _midPos;
    private Vector3 _targetPos;

    private float _elapsedTime = 0f;   // 経過時間
    private float _flightDuration;     // 飛行時間
    private bool _initialized = false; // 発射済みかどうか
    private ProjectilePool _pool;      // 所属プール

    /// <summary>
    /// 弾を初期化して発射する
    /// </summary>
    public void Initialize(Vector3 origin, Transform target, float spreadAngle, Vector3 sideOffsetDirection, ProjectilePool pool) {
        // 初期位置や管理フラグを設定
        _startPos = origin;
        _elapsedTime = 0f;
        _initialized = true;
        _pool = pool;

        // ランダムにターゲットをずらして着弾位置を決定
        _targetPos = CalculateRandomTargetOffset(target.position);

        // ベジエ曲線の中間点を計算
        _midPos = CalculateMidPoint(_startPos, _targetPos, sideOffsetDirection);

        // 飛行時間をランダムに設定（動きにバラつきを出す）
        //_flightDuration = Random.Range(0.6f, 1.5f);
        _flightDuration = 1.5f;

        // 有効化
        gameObject.SetActive(true);

        // エフェクトや見た目をリセット
        if (trail != null) trail.Clear();
        if (visual != null) {
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.SetActive(true);
        }

        // コライダーを有効化
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
    }

    /// <summary>
    /// ターゲット位置をランダムにずらす
    /// </summary>
    private Vector3 CalculateRandomTargetOffset(Vector3 baseTarget) {
        float xOffset = Random.Range(-3.5f, 3.5f);
        float yOffset = Random.Range(-3.0f, 3.0f);
        return baseTarget + new Vector3(xOffset, yOffset, 0f);
    }

    /// <summary>
    /// 中間点を計算して曲線を作る
    /// </summary>
    private Vector3 CalculateMidPoint(Vector3 from, Vector3 to, Vector3 sideOffsetDirection) {
        var center = Vector3.Lerp(from, to, 0.5f);       // 始点と終点の中間
        var offsetDir = sideOffsetDirection.normalized;  // 横方向のずれ
        var mid = center + offsetDir * _MID_OFFSET_AMOUNT;

        // 飛ぶ方向に少し引っ張る
        mid += (to - from).normalized * _ACCELERATION_PULL_AMOUNT;

        // 上下にランダムなカーブを付与
        mid += Vector3.up * Random.Range(-_VERTICAL_CURVE_AMOUNT, _VERTICAL_CURVE_AMOUNT);
        return mid;
    }

    private void Update() {
        if (!_initialized) return;

        // 飛行時間の進行度
        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / _flightDuration);

        // 緩急を付けるため二乗
        float easedT = t * t;

        // 二次ベジエ曲線で位置を算出
        Vector3 bezierPos =
            Mathf.Pow(1 - easedT, 2) * _startPos +
            2 * (1 - easedT) * easedT * _midPos +
            Mathf.Pow(easedT, 2) * _targetPos;

        transform.position = bezierPos;

        // 接線方向を向かせて進行方向を自然に見せる
        Vector3 tangent = 2 * (1 - easedT) * (_midPos - _startPos) + 2 * easedT * (_targetPos - _midPos);
        transform.forward = tangent.normalized;

        // 非アクティブ化
        if (t >= 1f) Deactivate();
    }

    /// <summary>
    /// 障害物に衝突したときの処理
    /// </summary>
    private async void OnTriggerEnter(Collider other) {
        if (!_initialized) return;
        if (other == null) return;
        // 破壊可能オブジェクトならダメージを与える
        if (other.CompareTag("Obstacle")) {
            var destructible = other.GetComponent<Destructible>();
            if (destructible != null) {
                // エフェクト再生
                EffectManager.Instance.Play(EffectID._EX, transform.position);
                // ダメージを与える
                destructible.TakeDamage(damage);
            }

            // 多段ヒット対策
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // 1フレーム待ってから非アクティブ化
            await UniTask.Yield();
            Deactivate();
        }
    }

    /// <summary>
    /// 弾を非アクティブ化し、プールに戻す
    /// </summary>
    private void Deactivate() {
        _initialized = false;
        if (visual != null) visual.SetActive(false);
        _pool?.ReturnProjectile(this);
    }
}
