using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public static UIController instance;
    public static GameUI game;
    public static EndScreenUI end;

    public GameObject startScreen, gameScreen, errorScreen, endScreen;
    public Text errorMessage;
    

   void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        game = GetComponentInChildren<GameUI>(true);
        end = GetComponentInChildren<EndScreenUI>(true);
   }

   void Start() {
        startScreen.SetActive(true);
        gameScreen.SetActive(false);
        errorScreen.SetActive(false);
        endScreen.SetActive(false);
   }

   public void OnStartGameClicked() {
        GameManager.instance.StartGame();
    }

    public void HandleGameStarted() {
        startScreen.SetActive(false);
        gameScreen.SetActive(true);
    }

    public void HandleGameEnded(bool vitoria) {
        end.SetarValores(vitoria);
        gameScreen.SetActive(false);
        endScreen.SetActive(true);
    }

    public void HandleGameError() {
        errorScreen.SetActive(true);
        gameScreen.SetActive(false);
        startScreen.SetActive(false);
    }

    public void HandleGameError(string error) {
        errorMessage.text = error;
        HandleGameError();
    }
}
