/*
 *  @file    Destructible.cs
 *  @author  oorui
 */
using UnityEngine;

/// <summary>
/// 破壊可能オブジェクトの耐久管理
/// </summary>
public class Destructible : MonoBehaviour {
    private int durability = 400;       // 破壊可能オブジェクトの耐久値

    // 耐久値を減らす
    public void TakeDamage(int amount) {
        durability -= amount;
        if (durability <= 0) {
            // エフェクト再生
            Vector3 efPos = transform.position;
            efPos.y += transform.position.y * -4f;
            EffectManager.Instance.Play("Nuke", efPos);
            // SE再生
            DestroySE();

            // 耐久値がなくなり次第削除
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// SEを再生させる
    /// </summary>
    private async void DestroySE() {
        // SE再生
        await SoundManager.instance.PlaySE(8);
    }


}
