using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Metacore
{
    public class NavigationPanel : MonoBehaviour
    {
        private Transform _contentRoot;
        private PressableButton _sceneButtonPrefab;

        private void Start()
        {
            _contentRoot = Util.FindChild<Transform>(gameObject, "Content");
            _sceneButtonPrefab = Util.FindChild<PressableButton>(gameObject, "Scene Button");

            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for (int i = 1; i < sceneCount; i++)
            {
                string sceneName = GetSceneName(SceneUtility.GetScenePathByBuildIndex(i));
                CreateSceneButton(i, sceneName);
            }

            if (_sceneButtonPrefab.gameObject.activeSelf)
            {
                _sceneButtonPrefab.gameObject.SetActive(false);
            }
        }

        private void CreateSceneButton(int i, string sceneName)
        {
            PressableButton button = GameObject.Instantiate(_sceneButtonPrefab, _contentRoot);
            TMP_Text text = Util.FindChild<TMP_Text>(button.gameObject, "NameText");
            if (text == null)
            {
                Debug.LogError("No NameText object");
                return;
            }
            else
            {
                text.text = sceneName;
            }

            button.OnClicked.AddListener(() => SceneManager.LoadSceneAsync(i));
        }

        private string GetSceneName(string sceneName)
        {
            string[] strings = sceneName.Split('/');
            return strings[strings.Length - 1];
        }
    }
}
