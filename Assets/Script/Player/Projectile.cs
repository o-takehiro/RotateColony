using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public float flightDuration = 1f; // 飛行時間
    public int damage = 10;           // 与えるダメージ

    // 撃ち始めの位置
    private Vector3 startPos;
    // 撃った後の中間点
    private Vector3 midPos;
    // 狙う位置
    private Vector3 targetPos;

    // 広がる最大の角度
    //private float maxSpreadAngle;
    private float elapsedTime = 0f;

    private bool initialized = false;

    // 初期化: 発射位置、目標位置、最大広がり角度を渡す
    public void Initialize(Vector3 _start, Transform _targetTransform, float _spreadAngle, Vector3 _sideOffsetDirection) {
        startPos = _start;
        targetPos = _targetTransform.position;

        Vector3 center = Vector3.Lerp(startPos, targetPos, 1f);

        // 弾の軌道の広がる角度
        float offsetAmount = 15f;

        midPos = center + _sideOffsetDirection.normalized * offsetAmount;

        elapsedTime = 0f;
        initialized = true;
    }

    private void Update() {
        if (!initialized) return;
        // 経過時間をプラス
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / flightDuration);

        // 二次ベジエで滑らかなカーブを描く
        Vector3 bezierPos = Mathf.Pow(1 - t, 2) * startPos +
                            2 * (1 - t) * t * midPos +
                            Mathf.Pow(t, 2) * targetPos;

        transform.position = bezierPos;

        // 飛翔方向を補正
        Vector3 tangent = 2 * (1 - t) * (midPos - startPos) + 2 * t * (targetPos - midPos);
        transform.forward = tangent.normalized;

        if (t >= 1f) {
            //HitTarget();
            Destroy(gameObject);
        }

#if false

        if (!initialized) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / flightDuration);

        // t=0で最大広がり、t=1で中心（target方向）に収束するイメージ
        // 放射角を時間経過で補間（広がり→収束）
        float angleFactor = Mathf.Sin(Mathf.PI * t); // 0->1->0で波形

        // 座標計算
        Vector3 direction = (targetPos - startPos).normalized;

        // 正面方向に沿った距離を球面補間
        float distance = Vector3.Distance(startPos, targetPos);
        Vector3 forwardPos = Vector3.Slerp(startPos, targetPos, t);

        // 放射方向（最大角度に対して現在の拡散角度を設定）
        float currentSpreadAngle = maxSpreadAngle * angleFactor;

        // 放射状方向はY軸を中心に回転させる
        // float spreadRadians = currentSpreadAngle * Mathf.Deg2Rad;


        Vector3 offset = Quaternion.Euler(0, currentSpreadAngle, 0) * direction * 0.5f * (1 - t);

        transform.position = forwardPos + offset;

        // 最終的にtargetに向ける
        transform.forward = (targetPos - transform.position).normalized;

        if (t >= 1f) {
            HitTarget();
        }
#endif

    }
    
    // 障害物と弾の破棄
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Obstacle")) {
            Destructible destructible = other.GetComponent<Destructible>();
            if (destructible != null) {
                destructible.TakeDamage(damage);
            }
            Destroy(gameObject); // 衝突時に削除
        }
    }
}