using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace MinutesGames
{
    public class MinutesClient : MonoBehaviour
    {
        public static UnityEvent<bool> onPause = new UnityEvent<bool>();
        public static bool isPausing { get; private set; }

        private float m_GameVolume;

        [DllImport("__Internal")]
        private static extern void OnSoundPauseComplete();

        [DllImport("__Internal")]
        private static extern void OnSoundResumeComplete();

        [DllImport("__Internal")]
        private static extern void AlreadyMounted();

        [DllImport("__Internal")]
        private static extern void SetPluginVersion();

        private void Start()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            AlreadyMounted();
            #endif
        }

        public void PauseGame()
        {
            if (isPausing) return;

            isPausing = true;
            m_GameVolume = AudioListener.volume;
            AudioListener.volume = 0;

            MountingConfirmation();

            onPause?.Invoke(true);

            #if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(InvokeSoundPauseComplete());
            #endif
        }

        public void MountingConfirmation(){
            AlreadyMounted();
        }

        public void ResumeGame()
        {
            if (isPausing == false) return;

            isPausing = false;
            AudioListener.volume = m_GameVolume;

            onPause?.Invoke(false);

            #if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(InvokeSoundResumeComplete());
            #endif
        }

        private IEnumerator InvokeSoundPauseComplete()
        {
            yield return null; // 1フレーム待つ
            OnSoundPauseComplete();
        }

        private IEnumerator InvokeSoundResumeComplete()
        {
            yield return null; // 1フレーム待つ
            OnSoundResumeComplete();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnGameLoad()
        {
            var gameObject = new GameObject("MinutesClient");
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            DontDestroyOnLoad(gameObject);

            gameObject.AddComponent<MinutesClient>();
        }
    }
}
