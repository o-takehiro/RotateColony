using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;
public class SoundManager : SystemObject {

    // BGM再生用オーディオソース
    [SerializeField]
    private AudioSource _bgmAudioSource = null;
    // SE再生用オーディオソース
    [SerializeField]
    private AudioSource[] _seAudioSource = null;

    // BGMのリスト
    [SerializeField]
    private BGMAssign _bgmAssign = null;
    [SerializeField]
    private SEAssign _seAssign = null;

    public static SoundManager instance { get; private set; } = null;


    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        instance = this;
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="bgmID"></param>
    public void PlayBGM(int bgmID) {
        if (!IsEnableIndex(_bgmAssign.bgmArray, bgmID)) return;

        _bgmAudioSource.clip = _bgmAssign.bgmArray[bgmID];
        _bgmAudioSource.Play();
    }

    /// <summary>
    /// BGM停止
    /// </summary>
    public void StopBGM() {
        _bgmAudioSource.Stop();
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="seID"></param>
    public async UniTask PlaySE(int seID) {
        if (!IsEnableIndex(_seAssign.seArray, seID)) return;

        // 再生中ではないオーディオソースを探してそれで再生
        for (int i = 0; i < _seAudioSource.Length; i++) {
            AudioSource audioSource = _seAudioSource[i];

            // AudioSource が null または再生中ならスキップ
            if (audioSource == null || audioSource.isPlaying) continue;

            // clip をセットして再生
            audioSource.clip = _seAssign.seArray[seID];
            audioSource.Play();

            // SEの終了待ち（途中で AudioSource が破棄された場合も安全）
            while (audioSource != null && audioSource.isPlaying) {
                await UniTask.DelayFrame(1);
            }

            return;
        }

    }




}
