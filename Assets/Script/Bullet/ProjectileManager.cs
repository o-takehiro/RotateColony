using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾をプールで管理するクラス
/// </summary>
public class ProjectileManager : MonoBehaviour {
    // 自身の取得
    public static ProjectileManager Instance { get; private set; } = null;
    
    // 弾のPrefab
    [SerializeField]
    private GameObject projectilePrefab;
    // プールするサイズ
    [SerializeField]
    private int initialPoolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

}
