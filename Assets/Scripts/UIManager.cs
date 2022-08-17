using System.Collections;
using System.Collections.Generic;
using GameManagerNamespace;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject successPanel;

    private void Awake()
    {
        HideAllPanel();
        menuPanel.SetActive(true);
    }

    public void StartGameButton()
    {
        GameManager.Instance.StartGame();
        HideAllPanel();
        gamePanel.SetActive(true);
    }

    public void NextLevelButton()
    {
        GameManager.Instance.NextLevel();
        HideAllPanel();
        menuPanel.SetActive(true);
    }

    public void RestartLevelButton()
    {
        GameManager.Instance.RestartLevel();
        HideAllPanel();
        menuPanel.SetActive(true);
    }

    public void OnFail()
    {
        HideAllPanel();
        failPanel.SetActive(true);
    }

    public void OnSuccess()
    {
        HideAllPanel();
        successPanel.SetActive(true);
    }

    public void HideAllPanel()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(false);
        failPanel.SetActive(false);
        successPanel.SetActive(false);
    }
}