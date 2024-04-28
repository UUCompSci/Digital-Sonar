using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;

    public void Start() {
        if (!DataPersistenceManager.instance.HasGameData()) {
            continueGameButton.interactable = false;
        }
    }
    
    public void OnNewGameClicked() {
        DisableMenuButtons();
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadSceneAsync("GameScene2");
    }

    public void OnContinueGameClicked() {
        DisableMenuButtons();
        SceneManager.LoadSceneAsync("GameScene2");
    }

    private void DisableMenuButtons() {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }
}
