using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialA : MonoBehaviour
{
    public Button nextButton;
    public GameObject tutorialMessage;

    private int curMsgIndex;
    private GameObject[] msgQueue;

    // Start is called before the first frame update
    void Start()
    {
      nextButton.onClick.AddListener(
        delegate
        {
          msgQueue[curMsgIndex].SetActive(false);

          curMsgIndex += 1;

          if(curMsgIndex < msgQueue.Length)
          {
            performAction();
            msgQueue[curMsgIndex].SetActive(true);
          }
          else
          {
            //tutorial over, start the game
            GetComponent<Controller>().startGame();
            Destroy(nextButton.gameObject);
          }
        }
      );
    }

    public void startTutorial(int round)
    {
      initMessage(round);

      if(msgQueue.Length > 0)
      {
        performAction();
        msgQueue[0].SetActive(true);
      }
      else
      {
        Destroy(nextButton.gameObject);
      }
    }

    private void initMessage(int round)
    {
      if(round == 1)
      {
        msgQueue = new GameObject[]{
          findObject("Status"),
          findObject("Command"),
          findObject("TL"),
          findObject("Log")
        };
      }
      else if(round == 2)
      {
        msgQueue = new GameObject[]{
          findObject("StrongAtkCaution"),
          findObject("KnowPatternByTL"),
          findObject("KnowPatternByLog")
        };
      }
      else if(round == 3)
      {
        msgQueue = new GameObject[]{
          findObject("EnemySkill"),
          findObject("TankSkill"),
          findObject("Defence")
        };
      }
      else if(round > 3)
      {
        msgQueue = new GameObject[]{
          findObject("SkipTutorial")
        };
      }
    }

    private void performAction()
    {
      if(msgQueue[curMsgIndex].name == "TL")
      {
        GetComponent<Controller>().showCommandLayout(false);
      }

      if(msgQueue[curMsgIndex].name == "Command")
      {
        GetComponent<Controller>().showCommandLayout(true);
      }

      if(msgQueue[curMsgIndex].name == "KnowPatternByTL")
      {
        GetComponent<Controller>().showCommandLayout(false);
      }

      if(msgQueue[curMsgIndex].name == "TankSkill")
      {
        GetComponent<Controller>().showCommandLayout(true);
      }

      if(msgQueue[curMsgIndex].name == "Defence")
      {
        GetComponent<Controller>().showCommandLayout(true);
      }
    }

    private GameObject findObject(string name)
    {
      for (int i = 0; i < tutorialMessage.transform.childCount; i += 1)
      {
        if(tutorialMessage.transform.GetChild(i).gameObject.name == name)
        {
          return tutorialMessage.transform.GetChild(i).gameObject;
        }
      }

      return null;
    }
}
