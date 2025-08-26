using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾のプール管理
/// Scene直下やプレイヤー直下に置いても使用可能
/// </summary>
public class ProjectilePool : MonoBehaviour {
    [SerializeField] private Projectile projectilePrefab; // プールする弾
    [SerializeField] private int poolSize = 10;           // プールの初期数

    private Queue<Projectile> pool = new Queue<Projectile>();

    private void Awake() {
        // 初期プール生成
        for (int i = 0; i < poolSize; i++) {
            Projectile proj = Instantiate(projectilePrefab, transform);
            proj.gameObject.SetActive(false);
            pool.Enqueue(proj);
        }
    }

    /// <summary>
    /// プールから弾を取得
    /// </summary>
    public Projectile GetProjectile() {
        if (pool.Count == 0) {
            Projectile proj = Instantiate(projectilePrefab, transform);
            proj.gameObject.SetActive(false);
            pool.Enqueue(proj);
        }

        return pool.Dequeue();
    }

    /// <summary>
    /// 使用済みの弾をプールに戻す
    /// </summary>
    public void ReturnProjectile(Projectile proj) {
        proj.gameObject.SetActive(false);
        pool.Enqueue(proj);
    }
}
