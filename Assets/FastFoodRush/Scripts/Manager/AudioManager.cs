using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace FastFoodRush.Manager
{
    public class AudioManager : MonoBehaviour
    {

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                }

                return _instance;
            }
        }

        private static AudioManager _instance;

        [Serializable]
        public class AudioSourceData
        {
            public string key;
            public AudioClip audioClip;
            public AudioSource audioSource;
        }

        [SerializeField] private List<AudioSourceData> _sfxDataList;
        [SerializeField] private List<AudioSourceData> _bgmDataList;

        private List<AudioSource> _audioSourceList;

        [Button]
        private void AddKey()
        {
            #if UNITY_EDITOR
            foreach (AudioSourceData audioSourceData in _sfxDataList)
            {
                string key = audioSourceData.audioClip.name;
                audioSourceData.key = key;
            }
            
            foreach (AudioSourceData audioSourceData in _bgmDataList)
            {
                audioSourceData.key = "BGM";
            }
            
            EditorUtility.SetDirty(gameObject);
            #endif
        }
        
        private void Start()
        {
            AddAudioSource();
        }

        private void AddAudioSource()
        {
            int count = 5;
            _audioSourceList ??= new List<AudioSource>(5);
            for (int i = 0; i < count; i++)
            {
                var component = gameObject.AddComponent<AudioSource>();
                _audioSourceList.Add(component);
            }
        }

        public void PlaySfX(string key, float volume = 1)
        {
            var audioSource = TryPlayAudio(_sfxDataList, key, volume);
            if (audioSource == null)
            {
                Debug.LogWarning($"failed play sfx audio {key}");
            }
        }
        
        public void StopSfx(string key)
        {
            bool isSuccess = TryStopAudio(_sfxDataList, key);
            if (!isSuccess)
            {
                Debug.LogWarning($"failed stop sfx audio {key}");
            }
        }

        public void StopBGM(string key)
        {
            bool isSuccess = TryStopAudio(_bgmDataList, key);
            if (!isSuccess)
            {
                Debug.LogWarning($"failed stop bgm audio {key}");
            }
        }

        public void PlayBGM(string key, float volume = 1, bool isLoop = true)
        {
            var audioSource = TryPlayAudio(_bgmDataList, key, volume);
            if (audioSource == null)
            {
                Debug.LogWarning($"failed play bgm audio {key}");
                return;
            }

            audioSource.loop = isLoop;
        }
        
        private AudioSource TryPlayAudio(List<AudioSourceData> audioSourceDataList, string key, float volume = 1)
        {
            foreach (AudioSourceData data in audioSourceDataList)
            {
                if (data.key == key)
                {
                    AudioClip audioClip = data.audioClip;
                    var audioSource = _audioSourceList.Find(v => !v.isPlaying);
                    if (audioSource == null)
                    {
                        var component = gameObject.AddComponent<AudioSource>();
                        _audioSourceList.Add(component);
                        audioSource = component;
                    }

                    audioSource.clip = audioClip;
                    audioSource.volume = volume;
                    audioSource.Play();
                    data.audioSource = audioSource;
                    return data.audioSource;
                }
            }

            return null;
        }

        private bool TryStopAudio(List<AudioSourceData> audioSourceDataList, string key)
        {
            foreach (AudioSourceData data in audioSourceDataList)
            {
                if (data.key == key && data.audioSource != null)
                {
                    data.audioSource.Stop();
                    data.audioSource = null;
                    return true;
                }
            }

            return false;
        }
    }
}