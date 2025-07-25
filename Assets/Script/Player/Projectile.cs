using Cysharp.Threading.Tasks;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [Header("Projectile Settings")]
    public int damage = 10;

    // 中間点のオフセット量
    private const float _MID_OFFSET_AMOUNT = 8f;

    // 前方方向に引っ張る量
    private const float _ACCELERATION_PULL_AMOUNT = 9f;

    // Y軸方向の軌道の膨らみ
    private const float _VERTICAL_CURVE_AMOUNT = 9f;

    // 射出位置
    private Vector3 _startPos;

    // 中間点
    private Vector3 _midPos;

    // 着弾点
    private Vector3 _targetPos;

    // 弾の飛行経過時間
    private float _elapsedTime = 0f;

    // 飛行時間
    private float _flightDuration;

    private bool _initialized = false;

    /// <summary>
    /// 弾を初期化
    /// </summary>
    public void Initialize(Vector3 origin, Transform target, float spreadAngle, Vector3 sideOffsetDirection) {
        _startPos = origin;
        // 目標位置を指定
        _targetPos = CalculateRandomTargetOffset(target.position);
        // 2次ペジェを行うための中間点を指定
        _midPos = CalculateMidPoint(_startPos, _targetPos, sideOffsetDirection);
        // 目標到達までの時間をランダムで指定
        _flightDuration = Random.Range(0.6f, 1.5f);

        // 値とフラグを初期化
        _elapsedTime = 0f;
        _initialized = true;
    }

    /// <summary>
    /// ランダムにずらしたターゲット座標を計算
    /// </summary>
    private Vector3 CalculateRandomTargetOffset(Vector3 baseTarget) {
        float xOffset = Random.Range(-3.5f, 3.5f);
        float yOffset = Random.Range(-3.0f, 3.0f);
        return baseTarget + new Vector3(xOffset, yOffset, 0f);
    }

    /// <summary>
    /// 中間点の計算
    /// </summary>
    private Vector3 CalculateMidPoint(Vector3 from, Vector3 to, Vector3 sideOffsetDirection) {
        var center = Vector3.Lerp(from, to, 0.5f);

        // 中間点を横方向にずらす
        var offsetDir = sideOffsetDirection.normalized;
        var mid = center + offsetDir * _MID_OFFSET_AMOUNT;

        // 前方方向に引っ張る
        var accelDir = (to - from).normalized;
        mid += accelDir * _ACCELERATION_PULL_AMOUNT;

        // Y方向にふくらみ
        mid += Vector3.up * Random.Range(-_VERTICAL_CURVE_AMOUNT, _VERTICAL_CURVE_AMOUNT);

        return mid;
    }

    private void Update() {
        if (!_initialized) return;

        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / _flightDuration);

        // 加速風(EaseIn)
        float easedT = t * t;

        // 2次ベジェ
        Vector3 bezierPos =
            Mathf.Pow(1 - easedT, 2) * _startPos +
            2 * (1 - easedT) * easedT * _midPos +
            Mathf.Pow(easedT, 2) * _targetPos;

        transform.position = bezierPos;

        // 進行方向に向きを合わせる
        Vector3 tangent =
            2 * (1 - easedT) * (_midPos - _startPos) +
            2 * easedT * (_targetPos - _midPos);

        transform.forward = tangent.normalized;

        if (t >= 1f) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 衝突処理
    /// </summary>
    private async void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Obstacle")) {
            var destructible = other.GetComponent<Destructible>();
            if (destructible != null) {
                // エフェクト再生
                EffectManager.Instance.Play("ex", transform.position);
                destructible.TakeDamage(damage);
            }

            // 1フレーム待機してから無効化
            // フラグ追加

            await UniTask.Yield();
            gameObject.SetActive(false);
        }
    }
}