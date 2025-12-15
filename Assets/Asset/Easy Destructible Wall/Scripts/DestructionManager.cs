/*
 *  @file   EasyDestuctibleWall
 *  @outhor oorui
 */

using UnityEngine;

/// <summary>
/// 破壊壁
/// 使用アセット
/// Easy Destructible Wall
namespace EasyDestuctibleWall {
    /// </summary>
    public class DestructionManager : MonoBehaviour {

        private float _HEALTH = 50f;                // 耐久値
        private float _IMPACT_MULTIPLIER = 10f;
        private float _TWIST_MULTIPLIER = 0.1f;

        private const float _DESTROYFRAME = 3.0f;   // 消すまでの時間

        // Rigidbodyのキャッシュ
        private Rigidbody cachedRigidbody;
        // プレイヤーのクラス
        [SerializeField]
        private PlayerMove _playerMove = null;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake() {
            cachedRigidbody = GetComponent<Rigidbody>();
            // プレイヤーを取得
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) {
                _playerMove = playerObj.GetComponent<PlayerMove>();
            }
        }

        /// <summary>
        /// 毎フレーム更新
        /// </summary>
        private void FixedUpdate() {
            ApplyTwistDamage();
            CheckHealth();
        }

        /// <summary>
        /// オブジェクトの回転速度からダメージを算出し、ヘルスを減少させる
        /// </summary>
        private void ApplyTwistDamage() {
            // プレイヤーが加速中のみ回転ダメージを適用
            if (_playerMove == null || !_playerMove.boostFlag) return;

            float angularDamage = Mathf.Round(cachedRigidbody.angularVelocity.sqrMagnitude * _TWIST_MULTIPLIER);
            _HEALTH -= angularDamage;
        }

        /// <summary>
        /// ヘルスが0以下なら、子オブジェクトを分離し、自身を破壊する
        /// </summary>
        private void CheckHealth() {
            if (_HEALTH > 0f) return;

            foreach (Transform child in transform) {
                // 子オブジェクトにRigidbodyを追加して独立させる
                Rigidbody childRigidbody = child.gameObject.AddComponent<Rigidbody>();

                // 親子関係を解除
                child.parent = null;

                // 元の速度と回転を引き継ぐ
                childRigidbody.velocity = cachedRigidbody.GetPointVelocity(child.position);
                childRigidbody.AddTorque(cachedRigidbody.angularVelocity, ForceMode.VelocityChange);

                // 3秒後に消す
                Destroy(child.gameObject, _DESTROYFRAME);
            }

            // 親オブジェクトを破壊
            Destroy(gameObject);
        }

        /// <summary>
        /// 他オブジェクトと衝突した際の処理。衝突強度に応じてダメージを計算
        /// </summary>
        /// <param name="collision">衝突情報</param>
        private void OnCollisionEnter(Collision collision) {

            // 速度によってダメージ増加
            float relativeVelocity = collision.relativeVelocity.sqrMagnitude;

            // 衝突相手にRigidBodyがあるかどうか
            // あればそれを使う、なければ1
            float mass = collision.rigidbody ? collision.rigidbody.mass : 1f;

            // ダメージ量
            float damage = relativeVelocity * _IMPACT_MULTIPLIER * mass;

            // 耐久値からダメージ分ひく
            _HEALTH -= damage;
        }
    }
}