using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public float flightDuration = 1f; // 飛行時間
    public int damage = 10;

    private Vector3 startPos;
    private Vector3 targetPos;

    private float maxSpreadAngle; // 最大広がり角度（度）
    private float elapsedTime = 0f;

    private bool initialized = false;

    // 初期化: 発射位置、目標位置、最大広がり角度を渡す
    public void Initialize(Vector3 start, Vector3 target, float spreadAngle) {
        startPos = start;
        targetPos = target;
        maxSpreadAngle = spreadAngle;
        elapsedTime = 0f;
        initialized = true;
    }

    private void Update() {
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
        float spreadRadians = currentSpreadAngle * Mathf.Deg2Rad;


        Vector3 offset = Quaternion.Euler(0, currentSpreadAngle, 0) * direction * 0.5f * (1 - t);

        transform.position = forwardPos + offset;

        // 最終的にtargetに向ける
        transform.forward = (targetPos - transform.position).normalized;

        if (t >= 1f) {
            HitTarget();
        }
    }

    private void HitTarget() {
        // ダメージ処理
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var col in hits) {
            if (col.CompareTag("Obstacle")) {
                // 使用しなくなった弾を削除する
                Destructible destructible = col.GetComponent<Destructible>();
                if (destructible != null) {
                    destructible.TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject);
    }
}