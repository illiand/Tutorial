using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameScene[] scenes;
    public GameScene currentScene;
    public BottomBarController bottomBar;
    public BackgroundController backgroundController;
    public ChooseController chooseController;

    private State state = State.IDLE;
    public bool InMap;
    public bool InBattle;

    public GameObject worldController;

    private enum State
    {
        IDLE, ANIMATE, CHOOSE
    }

    private void Awake()
    {
        InMap = false;
        InBattle = false;
    }

    void Start()
    {
        //InMap = false;
        //InBattle = false;
        //if (currentScene is StoryScene)
        //{
        //    StoryScene storyScene = currentScene as StoryScene;
        //    bottomBar.PlayScene(storyScene);
        //    backgroundController.SetImage(storyScene.background);
        //}
    }

    void Update()
    {
        if (InMap == false && InBattle == false)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                if (state == State.IDLE && bottomBar.IsCompleted())
                {
                    if (bottomBar.IsLastSentence())
                    {
                        if(bottomBar.barText.text ==  "Treasure?")
                        {
                          if(worldController.GetComponent<WorldMapController>().getPlayerStatus().flag[0])
                          {
                            PlaySceneNow(5);
                          }
                          else
                          {
                            PlaySceneNow(6);
                          }
                        }
                        else if(bottomBar.barText.text ==  "I am hungry, could you give some food?")
                        {
                          if(worldController.GetComponent<WorldMapController>().getPlayerStatus().itemRemaining[0] > 0)
                          {
;                           PlayScene(scenes[13]);
                          }
                          else
                          {
                            PlayScene(scenes[12]);
                          }
                        }
                        else
                        {
                          if(bottomBar.barText.text == "It's monster! Be awared!")
                          {
                            worldController.GetComponent<WorldMapController>().getMap().obj.SetActive(false);
                            worldController.GetComponent<WorldMapController>().battleLayout.SetActive(true);

                            worldController.GetComponent<WorldMapController>().summonMonsterNow();
                            worldController.GetComponent<Controller>().startExistingGame();
                          }
                          else if(bottomBar.barText.text == "Quiet, aren't we the same = =")
                          {
                            worldController.GetComponent<WorldMapController>().getPlayerStatus().flag[0] = true;
                          }

                          PlayScene((currentScene as StoryScene).nextScene);
                        }
                    }
                    else
                    {
                        bottomBar.PlayNextSentence();
                    }
                }
            }
        }
    }

    public void PlayScene(GameScene scene)
    {
        StartCoroutine(SwitchScene(scene));
    }

    public void PlaySceneNow(int index)
    {
      gameObject.SetActive(true);

      currentScene = scenes[index];
      StoryScene storyScene = currentScene as StoryScene;
      bottomBar.PlayScene(storyScene);
      backgroundController.SetImage(storyScene.background);
    }

    private IEnumerator SwitchScene(GameScene scene)
    {
        state = State.ANIMATE;
        currentScene = scene;
      //  bottomBar.Hide();
        yield return new WaitForSeconds(1f);
        if (scene is StoryScene)
        {
            StoryScene storyScene = scene as StoryScene;
            backgroundController.SwitchImage(storyScene.background);
            yield return new WaitForSeconds(1f);
            bottomBar.ClearText();
            bottomBar.Show();
            yield return new WaitForSeconds(1f);
            bottomBar.PlayScene(storyScene);
            state = State.IDLE;
        }
        else if (scene is ChooseScene)
        {
            state = State.CHOOSE;
            chooseController.SetupChoose(scene as ChooseScene);
        }
        else
        {
          gameObject.SetActive(false);
          state = State.IDLE;
        }
    }
}
