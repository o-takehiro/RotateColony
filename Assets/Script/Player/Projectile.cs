using UnityEngine;

public class Projectile : MonoBehaviour {
    public int damage = 10;

    private Vector3 startPos;
    private Vector3 midPos;
    private Vector3 targetPos;

    private float elapsedTime = 0f;
    private bool initialized = false;

    private float individualFlightDuration;

    // 軌道のふくらみ・加速風の変形の量
    private const float midOffsetAmount = 8f;           // 中間点
    private const float accelerationPullAmount = 9f;    // X軸
    private const float verticalCurveAmount = 9f;       // Y軸

    // エフェクト再生用
    public GameObject effectPrefab;

    public void Initialize(Vector3 _start, Transform _targetTransform, float _spreadAngle, Vector3 _sideOffsetDirection) {
        startPos = _start;

        // 着弾点をランダムで少しずらす（X, Y方向）
        Vector3 baseTargetPos = _targetTransform.position;
        float xOffset = Random.Range(-3.5f, 3.5f);
        float yOffset = Random.Range(-3.0f, 3.0f);
        targetPos = baseTargetPos + new Vector3(xOffset, yOffset, 0f);

        // 中間点（軌道のふくらみ）
        Vector3 center = Vector3.Lerp(startPos, targetPos, 0.5f);
        Vector3 offsetDir = _sideOffsetDirection.normalized;

        midPos = center + offsetDir * midOffsetAmount;

        // 前方方向へ引っ張る（加速風）
        Vector3 accelDir = (targetPos - startPos).normalized;
        midPos += accelDir * accelerationPullAmount;

        // Y軸方向にもオフセット（上下にふくらむ軌道）
        midPos += Vector3.up * Random.Range(-verticalCurveAmount, verticalCurveAmount);

        // ランダム飛行時間
        individualFlightDuration = Random.Range(0.6f, 1.5f);

        elapsedTime = 0f;
        initialized = true;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update() {
        if (!initialized) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / individualFlightDuration);

        // 加速風（EaseIn）
        t = t * t;

        // 2次ベジエ補間
        Vector3 bezierPos = Mathf.Pow(1 - t, 2) * startPos +
                            2 * (1 - t) * t * midPos +
                            Mathf.Pow(t, 2) * targetPos;

        transform.position = bezierPos;

        // 向き補正
        Vector3 tangent = 2 * (1 - t) * (midPos - startPos) + 2 * t * (targetPos - midPos);
        transform.forward = tangent.normalized;

        // 不要になったら削除
        if (t >= 1f) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 衝突判定
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Obstacle")) {
            var destructible = other.GetComponent<Destructible>();
            if (destructible != null || effectPrefab != null) {
                Instantiate(effectPrefab, transform.position, Quaternion.identity);
                destructible.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}