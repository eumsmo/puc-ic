using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour {
    public static UIController instance;
    public static GameUI game;
    public static EndScreenUI end;

    VisualElement startScreen, errorScreen, endScreen;
    Label errorMessage;
    

   void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        game = GetComponent<GameUI>();
        end = GetComponent<EndScreenUI>();
   }

   void Start() {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Button startButton = root.Q<Button>("ComecarButton");
        startButton.clicked += OnStartGameClicked;

        errorScreen = root.Q<VisualElement>("ErrorScreen");
        startScreen = root.Q<VisualElement>("StartScreen");
        endScreen = root.Q<VisualElement>("WinScreen");

        errorMessage = root.Q<Label>("ErrorMessage");

        startScreen.style.display = DisplayStyle.Flex;
        errorScreen.style.display = DisplayStyle.None;
        endScreen.style.display = DisplayStyle.None;
   }

   public void OnStartGameClicked() {
        GameManager.instance.StartGame();
    }

    public void HandleGameStarted() {
        startScreen.style.display = DisplayStyle.None;
    }

    public void HandleGameEnded(bool vitoria) {
        end.SetarValores(vitoria);
        endScreen.style.display = DisplayStyle.Flex;
    }

    public void HandleGameError() {
        errorScreen.style.display = DisplayStyle.Flex;
        startScreen.style.display = DisplayStyle.None;
    }

    public void HandleGameError(string error) {
        errorMessage.text = error;
        HandleGameError();
    }
}
