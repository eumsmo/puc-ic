using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour {
    public static UIController instance;
    public static GameUI game;

    VisualElement startScreen, errorScreen;
    Label errorMessage;
    

   void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        game = GetComponent<GameUI>();
   }

   void Start() {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Button startButton = root.Q<Button>("ComecarButton");
        startButton.clicked += OnStartGameClicked;

        errorScreen = root.Q<VisualElement>("ErrorScreen");
        startScreen = root.Q<VisualElement>("StartScreen");

        errorMessage = root.Q<Label>("ErrorMessage");

        startScreen.style.display = DisplayStyle.Flex;
   }

   public void OnStartGameClicked() {
        GameManager.instance.StartGame();
    }

    public void HandleGameStarted() {
        startScreen.style.display = DisplayStyle.None;
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
