using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 弾の動作（プール対応）
/// </summary>
public class Projectile : MonoBehaviour {
    [Header("Projectile Settings")]
    public int damage = 10;
    [SerializeField] private TrailRenderer trail; // 弾の軌跡
    [SerializeField] private GameObject visual;   // 弾の見た目

    private const float _MID_OFFSET_AMOUNT = 8f;
    private const float _ACCELERATION_PULL_AMOUNT = 9f;
    private const float _VERTICAL_CURVE_AMOUNT = 9f;

    private Vector3 _startPos;
    private Vector3 _midPos;
    private Vector3 _targetPos;

    private float _elapsedTime = 0f;
    private float _flightDuration;
    private bool _initialized = false;
    private ProjectilePool _pool; // 所属プール

    /// <summary>
    /// 弾を初期化して発射
    /// </summary>
    public void Initialize(Vector3 origin, Transform target, float spreadAngle, Vector3 sideOffsetDirection, ProjectilePool pool) {
        _startPos = origin;
        _elapsedTime = 0f;
        _initialized = true;
        _pool = pool;

        _targetPos = CalculateRandomTargetOffset(target.position);
        _midPos = CalculateMidPoint(_startPos, _targetPos, sideOffsetDirection);
        _flightDuration = Random.Range(0.6f, 1.5f);

        gameObject.SetActive(true);

        if (trail != null) trail.Clear();
        if (visual != null) {
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.SetActive(true);
        }

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
    }

    private Vector3 CalculateRandomTargetOffset(Vector3 baseTarget) {
        float xOffset = Random.Range(-3.5f, 3.5f);
        float yOffset = Random.Range(-3.0f, 3.0f);
        return baseTarget + new Vector3(xOffset, yOffset, 0f);
    }

    private Vector3 CalculateMidPoint(Vector3 from, Vector3 to, Vector3 sideOffsetDirection) {
        var center = Vector3.Lerp(from, to, 0.5f);
        var offsetDir = sideOffsetDirection.normalized;
        var mid = center + offsetDir * _MID_OFFSET_AMOUNT;
        mid += (to - from).normalized * _ACCELERATION_PULL_AMOUNT;
        mid += Vector3.up * Random.Range(-_VERTICAL_CURVE_AMOUNT, _VERTICAL_CURVE_AMOUNT);
        return mid;
    }

    private void Update() {
        if (!_initialized) return;

        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / _flightDuration);
        float easedT = t * t;

        Vector3 bezierPos =
            Mathf.Pow(1 - easedT, 2) * _startPos +
            2 * (1 - easedT) * easedT * _midPos +
            Mathf.Pow(easedT, 2) * _targetPos;

        transform.position = bezierPos;

        Vector3 tangent = 2 * (1 - easedT) * (_midPos - _startPos) + 2 * easedT * (_targetPos - _midPos);
        transform.forward = tangent.normalized;

        if (t >= 1f) Deactivate();
    }

    private async void OnTriggerEnter(Collider other) {
        if (!_initialized) return;

        if (other.CompareTag("Obstacle")) {
            var destructible = other.GetComponent<Destructible>();
            if (destructible != null) {
                EffectManager.Instance.Play("ex", transform.position);
                destructible.TakeDamage(damage);
            }

            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            await UniTask.Yield();
            Deactivate();
        }
    }

    /// <summary>
    /// 弾を非アクティブ化してプールに戻す
    /// </summary>
    private void Deactivate() {
        _initialized = false;

        if (visual != null) visual.SetActive(false);
        _pool?.ReturnProjectile(this);
    }
}
