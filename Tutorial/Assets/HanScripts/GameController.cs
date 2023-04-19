using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public StoryScene currentScene;
    public BotBarController botBar;
    public BackgroundController backgroundController;

    private void Start()
    {
        botBar.PlayScene(currentScene);
        setBG(currentScene.background);
    }

    private void setBG(Sprite bgimage)
    {
        backgroundController.SwitchBGImage(bgimage);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (botBar.IsLastSentence())
            {
                currentScene = currentScene.nextScene;
                botBar.PlayScene(currentScene);
                setBG(currentScene.background);
            }
            if (botBar.IsCompleted())
            {
                botBar.PlayNextScene();
                
            }
        }
    }
}
