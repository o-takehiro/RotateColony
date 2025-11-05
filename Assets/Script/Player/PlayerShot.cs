/*
 *  @file   PlayerShot.cs
 *  @author oorui
 */

using UnityEngine;
/// <summary>
/// プレイヤーの射撃処理
/// </summary>
public class PlayerShot : MonoBehaviour {
    private PlayerMove playerMove;

    [Header("Fire Points")]
    [SerializeField] private Transform leftFirePoint;               // 左側の射撃位置
    [SerializeField] private Transform rightFirePoint;              // 右側の射撃位置
    [SerializeField] private Transform shotTargetPoint;             // 的の位置

    [Header("Projectile Settings")]
    [SerializeField] private int damage = 10;                       // 弾一発のダメージ
    [SerializeField] private float maxSpreadAngle = 30f;            // 弾の最大拡散角度

    [Header("Projectile Pools")]
    [SerializeField] private ProjectilePool leftProjectilePool;     // 左側の弾のプール
    [SerializeField] private ProjectilePool rightProjectilePool;    // 右側の弾のプール

    private readonly float FIRE_INTERVAL = 0.1f;                    // 弾の発射レート
    private float leftFireTimer = 0f;                               // 左の射撃後の時間
    private float rightFireTimer = 0f;                              // 右の射撃後の時間

    public bool isShot = false;          // 射撃可能フラグ
    private const int _FIRE_SE_ID = 2;   // 発射音ID

    private void Start() {
        // 移動クラスの取得
        playerMove = GetComponent<PlayerMove>();
        // フラグの初期化
        isShot = false;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private async void Update() {
        if (playerMove == null || !playerMove.GetIsMoving()) return;
        // 射撃確認後の処理
        if (isShot) {
            // 弾の発射
            Fire();
            // SE再生
            await SoundManager.instance.PlaySE(_FIRE_SE_ID);
        }
    }

    // 左右で射撃位置から弾を生成する
    private void FireLeft() => FireFromPoint(leftFirePoint, leftProjectilePool);
    private void FireRight() => FireFromPoint(rightFirePoint, rightProjectilePool);

    /// <summary>
    /// 指定FirePointから弾を発射
    /// </summary>
    private void FireFromPoint(Transform firePoint, ProjectilePool pool) {
        Projectile projScript = pool.GetProjectile();
        if (projScript == null) return;

        // 座標、角度の更新
        projScript.transform.position = firePoint.position;
        projScript.transform.rotation = Quaternion.identity;
        projScript.damage = damage;

        // 左右で弾道の方向を調整
        Vector3 forward = (shotTargetPoint.position - firePoint.position).normalized;
        Vector3 rightDir = Vector3.Cross(Vector3.up, forward);
        Vector3 offsetDir = firePoint == leftFirePoint ? -rightDir : rightDir;

        projScript.Initialize(firePoint.position, shotTargetPoint, maxSpreadAngle, offsetDir, pool);
    }

    /// <summary>
    /// 発射間隔の管理
    /// </summary>
    private void Fire() {
        leftFireTimer += Time.deltaTime;
        rightFireTimer += Time.deltaTime;

        if (leftFireTimer >= FIRE_INTERVAL) {
            FireLeft();
            leftFireTimer = 0f;
        }

        if (rightFireTimer >= FIRE_INTERVAL) {
            FireRight();
            rightFireTimer = 0f;
        }
    }
}
