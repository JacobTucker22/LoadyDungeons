﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class GameManager : MonoBehaviour
{
    private static AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle;
    
    [SerializeField]
    private AssetReferenceSprite m_LogoAssetReference;

    private AsyncOperationHandle<Sprite> m_LogoLoadOpHandle;

    public static GameManager Instance { get; private set; }
    
    public static int s_CurrentLevel = 0;

    public static int s_MaxAvailableLevel = 5;

    // The value of -1 means no hats have been purchased
    public static int s_ActiveHat = 0;

    [SerializeField] private Image m_gameLogoImage;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnEnable()
    {
        // When we go to the 
        s_CurrentLevel = 0;

        if (!m_LogoAssetReference.RuntimeKeyIsValid())
        {
            return;
        }

        m_LogoLoadOpHandle = Addressables.LoadAssetAsync<Sprite>(m_LogoAssetReference);
        m_LogoLoadOpHandle.Completed += OnLogoLoadComplete;

    }

    private void OnLogoLoadComplete(AsyncOperationHandle<Sprite> asyncOperationHandle)
    {
        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            m_gameLogoImage.sprite = asyncOperationHandle.Result;
        }
    }

    public void ExitGame()
    {
        s_CurrentLevel = 0;
    }

    public void SetCurrentLevel(int level)
    {
        s_CurrentLevel = level;
    }

    public static void LoadNextLevel()
    {
        m_SceneLoadOpHandle = Addressables.LoadSceneAsync("LoadingScene", activateOnLoad: true);
    }

    public static void LevelCompleted()
    {
        s_CurrentLevel++;

        // Just to make sure we don't try to go beyond the allowed number of levels.
        s_CurrentLevel = s_CurrentLevel % s_MaxAvailableLevel;

        LoadNextLevel();
    }

    public static void ExitGameplay()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
