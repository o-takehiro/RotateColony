using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 弾（Projectile）の動作
/// プール対応済みで、着弾後は非アクティブ化して再利用可能
/// </summary>
public class Projectile : MonoBehaviour {
    [Header("Projectile Settings")]
    public int damage = 10;

    // 中間点のオフセット量
    private const float _MID_OFFSET_AMOUNT = 8f;
    private const float _ACCELERATION_PULL_AMOUNT = 9f;
    private const float _VERTICAL_CURVE_AMOUNT = 9f;

    private Vector3 _startPos;   // 射出位置
    private Vector3 _midPos;     // 中間点
    private Vector3 _targetPos;  // 着弾点

    private float _elapsedTime = 0f;    // 経過時間
    private float _flightDuration;      // 飛行時間
    private bool _initialized = false;  // 初期化済みフラグ

    /// <summary>
    /// 弾を初期化して発射準備
    /// </summary>
    public void Initialize(Vector3 origin, Transform target, float spreadAngle, Vector3 sideOffsetDirection) {
        _startPos = origin;
        _targetPos = CalculateRandomTargetOffset(target.position);
        _midPos = CalculateMidPoint(_startPos, _targetPos, sideOffsetDirection);
        _flightDuration = Random.Range(0.6f, 1.5f);

        _elapsedTime = 0f;
        _initialized = true;
        gameObject.SetActive(true); // 再利用時にアクティブ化
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

        var accelDir = (to - from).normalized;
        mid += accelDir * _ACCELERATION_PULL_AMOUNT;

        mid += Vector3.up * Random.Range(-_VERTICAL_CURVE_AMOUNT, _VERTICAL_CURVE_AMOUNT);

        return mid;
    }

    private void Update() {
        if (!_initialized) return;

        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / _flightDuration);
        float easedT = t * t; // 加速風(EaseIn)

        // 2次ベジェ計算
        Vector3 bezierPos =
            Mathf.Pow(1 - easedT, 2) * _startPos +
            2 * (1 - easedT) * easedT * _midPos +
            Mathf.Pow(easedT, 2) * _targetPos;

        transform.position = bezierPos;

        Vector3 tangent =
            2 * (1 - easedT) * (_midPos - _startPos) +
            2 * easedT * (_targetPos - _midPos);
        transform.forward = tangent.normalized;

        // 飛行終了時は非アクティブ化
        if (t >= 1f) {
            _initialized = false;
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 衝突処理（障害物に当たったら非アクティブ化）
    /// </summary>
    private async void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Obstacle")) {
            var destructible = other.GetComponent<Destructible>();
            if (destructible != null) {
                EffectManager.Instance.Play("ex", transform.position);
                destructible.TakeDamage(damage);
            }

            // 1フレーム待機してから非アクティブ化
            await UniTask.Yield();
            _initialized = false;
            gameObject.SetActive(false);
        }
    }
}
