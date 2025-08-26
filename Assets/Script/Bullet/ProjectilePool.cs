using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾のオブジェクトプール管理
/// </summary>
public class ProjectilePool : MonoBehaviour {
    [SerializeField] private GameObject projectilePrefab; // プール対象の弾プレハブ
    [SerializeField] private int initialPoolSize = 20;    // 初期プール数

    private List<GameObject> pool = new List<GameObject>();
    private Transform poolParent;

    private void Awake() {
        // プール用の空オブジェクトを作成
        poolParent = new GameObject("bulletPool").transform;
        poolParent.parent = this.transform;

        // 初期プール生成
        for (int i = 0; i < initialPoolSize; i++) {
            GameObject proj = Instantiate(projectilePrefab, poolParent);
            proj.SetActive(false);
            pool.Add(proj);
        }
    }

    /// <summary>
    /// 弾を取得（非アクティブのものを再利用、無ければ新規生成）
    /// </summary>
    public GameObject GetProjectile() {
        foreach (var proj in pool) {
            if (!proj.activeInHierarchy) {
                proj.SetActive(true);
                return proj;
            }
        }

        // プールに空きが無ければ新規生成
        GameObject newProj = Instantiate(projectilePrefab, poolParent);
        pool.Add(newProj);
        return newProj;
    }
}
