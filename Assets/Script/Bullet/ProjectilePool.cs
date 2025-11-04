/*
 *  @file ProjectilePool.cs
 *  @author oorui
 */

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾のプール管理
/// </summary>
public class ProjectilePool : MonoBehaviour {
    const int DEFAULT_POOLSIZE = 10;
    [SerializeField] private Projectile projectilePrefab;               // プールする弾
    [SerializeField] private int poolSize = DEFAULT_POOLSIZE;           // プールの初期数

    private Queue<Projectile> pool = new Queue<Projectile>();           // 弾の待ち行列

    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake() {
        // 初期プール生成
        for (int i = 0; i < poolSize; i++) {
            Projectile proj = Instantiate(projectilePrefab, transform);
            proj.gameObject.SetActive(false);
            // 末尾に追加
            pool.Enqueue(proj);
        }
    }

    /// <summary>
    /// プールから弾を取得
    /// </summary>
    public Projectile GetProjectile() {
        // プールが空であれば新しい弾を生成、追加
        if (pool.Count == 0) {
            // 弾を生成
            Projectile proj = Instantiate(projectilePrefab, transform);
            proj.gameObject.SetActive(false);
            // 末尾に追加
            pool.Enqueue(proj);
        }
        // 先頭から弾を取り出して返す
        return pool.Dequeue();
    }

    /// <summary>
    /// 使用済みの弾をプールに戻す
    /// </summary>
    public void ReturnProjectile(Projectile proj) {
        proj.gameObject.SetActive(false);
        // 末尾に追加
        pool.Enqueue(proj);
    }
}
