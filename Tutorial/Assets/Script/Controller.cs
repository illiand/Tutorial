using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    private GameObject sceneController;

    public GameObject[] characters;
    public GameObject[] timeLine;

    public Button attackButton;
    public Button skillButton;
    public Button defenceButton;
    public Button itemButton;

    public GameObject skillLayout;
    public Button[] skillButtons;
    public GameObject skillDesLayout;
    public GameObject skillDesText;

    public GameObject nextLevelLayout;
    public Button nextLevelButton;
    public GameObject nextLevelText;

    public bool[] isActive = new bool[10];

    //the unit in movement
    private int curCharacterID;

    //the skill player in use
    private int curSkillID;

    private int viewingCharacterID;
    private bool inTargeting;
    private bool isPlayerTurn;

    private bool isWaiting;
    private float waitingTime;

    public int curTurn = 0;
    private int maxTurn = 100;

    private bool isUsingItem;

    // Start is called before the first frame update
    void Start()
    {
      sceneController = GameObject.Find("SceneController");

      for(int i = 0; i < 10; i += 1)
      {
        int finalI = i;

        Material hpInstance = Instantiate(characters[i].GetComponent<MyCharacter>().findObject("HP").GetComponent<Image>().material);
        Material mpInstance = Instantiate(characters[i].GetComponent<MyCharacter>().findObject("MP").GetComponent<Image>().material);

        characters[i].GetComponent<MyCharacter>().findObject("HP").GetComponent<Image>().material = hpInstance;
        characters[i].GetComponent<MyCharacter>().findObject("MP").GetComponent<Image>().material = mpInstance;

        //player side chara use action goes here
        characters[i].GetComponent<MyCharacter>().findObject("Image").GetComponent<Button>().onClick.AddListener(
          delegate
          {
            if(!inTargeting)
            {
              if(attackButton.gameObject.activeSelf || skillLayout.activeSelf)
              {
                viewingCharacterID = finalI;
                showCommandLayout(false);
                skillLayout.SetActive(true);
                skillDesLayout.SetActive(true);
                setupSkillList(viewingCharacterID);
              }

              return;
            }

            //set cool down and mp cost
            for(int j = 0; j < characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills.Length; j += 1)
            {
              if(characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[j].id == curSkillID)
              {
                characters[curCharacterID].GetComponent<MyCharacter>().status.skillsCoolDown[j] = characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[j].cooldown;
                break;
              }
            }

            //check if wrong target
            if(getSkillInfo(curSkillID).type == 0)
            {
              castSkill(curSkillID, curCharacterID, new int[]{curCharacterID});
            }
            else if(getSkillInfo(curSkillID).type == 1 && finalI >= 5 || getSkillInfo(curSkillID).type == 2 && finalI < 5)
            {
              return;
            }
            else
            {
              castSkill(curSkillID, curCharacterID, new int[]{finalI});
            }

            if(isUsingItem)
            {
              GetComponent<WorldMapController>().getPlayerStatus().itemRemaining[curSkillID - 1000] -= 1;
              isUsingItem = false;
            }


              pauseBetweenTurn();
            characters[curCharacterID].GetComponent<MyCharacter>().status.curMp -= ((SkillAbility)getSkillInfo(curSkillID)).mpCost;

            characters[curCharacterID].GetComponent<MyCharacter>().status.curPos = 100;
            characters[curCharacterID].GetComponent<MyCharacter>().findObject("Background").SetActive(false);

            inTargeting = false;
            showCommandLayout(false);
            skillDesLayout.SetActive(false);

            //updateTurnPosition();
          }
        );
      }

      attackButton.onClick.AddListener(
        delegate
        {
          showCommandLayout(false);

          curSkillID = 0;
          inTargeting = true;
        }
      );

      skillButton.onClick.AddListener(
        delegate
        {
          viewingCharacterID = curCharacterID;
          setupSkillList(curCharacterID);

          showCommandLayout(false);
          skillLayout.SetActive(true);

          isUsingItem = false;
        }
      );

      itemButton.onClick.AddListener(
        delegate
        {
          viewingCharacterID = curCharacterID;
          setupItemList();

          showCommandLayout(false);
          skillLayout.SetActive(true);

          isUsingItem = true;
        }
      );

      defenceButton.onClick.AddListener(
        delegate
        {
          //回合制游戏的醍醐味
          //防御自己也要选目标
          showCommandLayout(false);

          curSkillID = 14;
          inTargeting = true;
          // castSkill(14, curCharacterID, curCharacterID);
          // updateTurnPosition();
          //
          // showCommandLayout(false);
        }
      );

      for(int i = 0; i < 6; i += 1)
      {
        int finalI = i;

        skillButtons[i].onClick.AddListener(
          delegate
          {
            skillDesLayout.SetActive(true);

            if(!isUsingItem)
            {
              skillDesText.GetComponent<TextMeshProUGUI>().text = characters[viewingCharacterID].GetComponent<MyCharacter>().parameter.skills[finalI].name + "\n\n" + characters[viewingCharacterID].GetComponent<MyCharacter>().parameter.skills[finalI].des;

              if(viewingCharacterID == curCharacterID &&
                !characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[finalI].isPassive &&
                characters[curCharacterID].GetComponent<MyCharacter>().status.skillsCoolDown[finalI] == 0 &&
                characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[finalI].mpCost <= characters[curCharacterID].GetComponent<MyCharacter>().status.curMp)
              {
                curSkillID = characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[finalI].id;
                inTargeting = true;

                skillLayout.SetActive(false);
              }
            }
            else
            {
              skillDesText.GetComponent<TextMeshProUGUI>().text = getSkillInfo(1000 + finalI).name + "\n" + getSkillInfo(1000 + finalI).des;

              if(GetComponent<WorldMapController>().getPlayerStatus().itemRemaining[finalI] > 0)
              {
                curSkillID = 1000 + finalI;
                inTargeting = true;

                skillLayout.SetActive(false);
              }
            }
          }
        );
      }

      if(SceneManager.GetActiveScene().name == "SceneA")
      {
        startTutorialVersionA();
      }
      else if(SceneManager.GetActiveScene().name == "SceneB")
      {
        startTutorial();
        GetComponent<TutorialB>().startOP();
      }
      else if(SceneManager.GetActiveScene().name == "Level1")
      {
        startLevel1();
        startGame();
      }
    }

    void Update()
    {
      if(Input.GetMouseButtonDown(1))
      {
        //skill selecting...
        if(!inTargeting && (skillLayout.activeSelf || skillDesLayout.activeSelf))
        {
          skillDesLayout.SetActive(false);
          skillLayout.SetActive(false);
          showCommandLayout(true);
        }

        if(inTargeting)
        {
          inTargeting = false;

          //normal attack targeting
          if(!skillLayout.activeSelf && !skillDesLayout.activeSelf)
          {
            showCommandLayout(true);
          }
          //skill targeting
          else
          {
            skillLayout.SetActive(true);
          }
        }
      }

      //wait between turns
      if(isWaiting)
      {
        if(waitingTime > 2f)
        {
          isWaiting = false;
          updateTurnPosition();
        }

        waitingTime += Time.deltaTime;
      }
    }

    public void startGame()
    {
      //run the game
      parsePassiveBuff();
      updateTurnPosition();
    }

    public void startExistingGame()
    {
      updateTurnPosition();
    }

    public void startTutorialVersionA()
    {
      startTutorial();
      sceneController.GetComponent<SceneController>().tryCount += 1;
      GetComponent<TutorialA>().startTutorial(sceneController.GetComponent<SceneController>().tryCount);
    }

    private void setupSkillList(int index)
    {
      for(int i = 0; i < 6; i += 1)
      {
        if(i >= characters[index].GetComponent<MyCharacter>().parameter.skills.Length)
        {
          skillButtons[i].gameObject.SetActive(false);
          continue;
        }
        else
        {
          if(characters[index].GetComponent<MyCharacter>().status.skillsCoolDown[i] != 0)
          {
            skillButtons[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Can use after " + characters[index].GetComponent<MyCharacter>().status.skillsCoolDown[i] + " Turn";
          }
          else if(characters[index].GetComponent<MyCharacter>().parameter.skills[i].mpCost > characters[index].GetComponent<MyCharacter>().status.curMp)
          {
            skillButtons[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "No Mp...";
          }
          else
          {
            skillButtons[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = characters[index].GetComponent<MyCharacter>().parameter.skills[i].name;
          }

          skillButtons[i].gameObject.SetActive(true);

          if(!characters[index].GetComponent<MyCharacter>().parameter.skills[i].isPassive && viewingCharacterID == curCharacterID)
          {
            skillButtons[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
          }
          else
          {
            skillButtons[i].GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1f);
          }
        }
      }
    }

    private void setupItemList()
    {
      for(int i = 0; i < 6; i += 1)
      {
        if(i >= GetComponent<WorldMapController>().getPlayerStatus().itemRemaining.Length)
        {
          skillButtons[i].gameObject.SetActive(false);
          continue;
        }
        else
        {
          skillButtons[i].gameObject.SetActive(true);
          skillButtons[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = getSkillInfo(1000 + i).name + ": " + GetComponent<WorldMapController>().getPlayerStatus().itemRemaining[i];

          if(GetComponent<WorldMapController>().getPlayerStatus().itemRemaining[i] > 0)
          {
            skillButtons[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
          }
          else
          {
            skillButtons[i].GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1f);
          }
        }
      }
    }

    private void startTutorial()
    {
        // GameObject.Find("SkillSelectionLayout").SetActive(false);
      characters[3].SetActive(false);
      characters[4].SetActive(false);
      characters[6].SetActive(false);
      characters[7].SetActive(false);
      characters[8].SetActive(false);
      characters[9].SetActive(false);

      isActive[0] = true;
      isActive[1] = true;
      isActive[2] = true;
      isActive[5] = true;

      initParameterInfo(0, "Tank", "char_01", 20, 70, 100, 100, 35);
      initParameterInfo(1, "Dps", "char_02", 12, 100, 200, 30, 40);
      initParameterInfo(2, "Healer", "char_03", 12, 150, 125, 50, 35);
      initParameterInfo(5, "BOSS", "char_18", 10000, 500, 300, 25, 80);

      characters[0].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(1), getSkillInfo(2), getSkillInfo(3)};
      characters[0].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0, 0};

      characters[1].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(4), getSkillInfo(5)};
      characters[1].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      characters[2].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(6), getSkillInfo(7)};
      characters[2].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      characters[5].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(8), getSkillInfo(9)};
      characters[5].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      initTurnPosition();
    }

    private void startLevel1()
    {
      characters[3].SetActive(false);
      characters[4].SetActive(false);
      characters[5].SetActive(false);
      characters[7].SetActive(false);
      characters[8].SetActive(false);
      characters[9].SetActive(false);

      isActive[0] = true;
      isActive[1] = true;
      isActive[2] = true;
      isActive[6] = true;

      initParameterInfo(0, "Tank", "char_01", 2000, 70, 100, 100, 35);
      initParameterInfo(1, "Dps", "char_02", 1200, 100, 200, 30, 40);
      initParameterInfo(2, "Healer", "char_03", 1200, 150, 125, 50, 35);
      initParameterInfo(6, "BOSS", "char_47", 10000, 500, 300, 25, 20);

      characters[0].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(1), getSkillInfo(2), getSkillInfo(3)};
      characters[0].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0, 0};

      characters[1].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(4), getSkillInfo(5)};
      characters[1].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      characters[2].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(6), getSkillInfo(7)};
      characters[2].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      characters[6].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(15), getSkillInfo(22), getSkillInfo(21), getSkillInfo(20), getSkillInfo(16), getSkillInfo(29)};
      characters[6].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0, 0, 0, 0, 0};

      initTurnPosition();
    }

    public void exitBattle()
    {
      GetComponent<WorldMapController>().getPlayerStatus().battleCount += 1;

      GetComponent<WorldMapController>().battleLayout.SetActive(false);
      GetComponent<WorldMapController>().mapLayout.SetActive(true);

      //score
      float curHp = 0;
      float maxHp = 0;

      for(int i = 5; i < 10; i += 1)
      {
        if(characters[i].GetComponent<MyCharacter>().status != null)
        {
          curHp = characters[i].GetComponent<MyCharacter>().status.curHp;
          maxHp = characters[i].GetComponent<MyCharacter>().status.maxHp;
        }
      }

      GetComponent<WorldMapController>().getPlayerStatus().O += 1f - curHp / maxHp;

      for(int i = 3; i < 10; i += 1)
      {
        isActive[i] = false;
        characters[i].SetActive(false);
        characters[i].GetComponent<MyCharacter>().parameter = null;
        characters[i].GetComponent<MyCharacter>().status = null;
      }

      for(int i = 0; i < 3; i += 1)
      {
        isActive[i] = true;
        characters[i].SetActive(true);

        if(characters[i].GetComponent<MyCharacter>().status.curHp > 0)
        {
          characters[i].GetComponent<MyCharacter>().status.curHp = characters[i].GetComponent<MyCharacter>().parameter.hp * (characters[i].GetComponent<MyCharacter>().status.curHp / characters[i].GetComponent<MyCharacter>().status.maxHp);
          characters[i].GetComponent<MyCharacter>().status.curHp = Mathf.Min(characters[i].GetComponent<MyCharacter>().status.curHp + 0.25f * characters[i].GetComponent<MyCharacter>().parameter.hp, characters[i].GetComponent<MyCharacter>().parameter.hp);
        }
        else
        {
          characters[i].GetComponent<MyCharacter>().status.curHp = characters[i].GetComponent<MyCharacter>().parameter.hp * 0.5f;
        }

        characters[i].GetComponent<MyCharacter>().status.curMp = characters[i].GetComponent<MyCharacter>().parameter.mp * (characters[i].GetComponent<MyCharacter>().status.curMp / characters[i].GetComponent<MyCharacter>().status.maxMp);

        characters[i].GetComponent<MyCharacter>().status.curSpd = characters[i].GetComponent<MyCharacter>().parameter.spd;
        characters[i].GetComponent<MyCharacter>().status.curDef = characters[i].GetComponent<MyCharacter>().parameter.def;
        characters[i].GetComponent<MyCharacter>().status.curAtk = characters[i].GetComponent<MyCharacter>().parameter.atk;

        characters[i].GetComponent<MyCharacter>().status.skillsCoolDown = new int[characters[i].GetComponent<MyCharacter>().status.skillsCoolDown.Length];
        characters[i].GetComponent<MyCharacter>().status.buff = new ArrayList();
      }
    }

    private void initParameterInfo(int id, string name, string picName, int hp, int mp, int atk, int def, int spd)
    {
      characters[id].GetComponent<MyCharacter>().parameter = new Parameter();
      characters[id].GetComponent<MyCharacter>().status = new BattleStatus();

      characters[id].GetComponent<MyCharacter>().status.index = id;
      characters[id].GetComponent<MyCharacter>().parameter.name = name;
      characters[id].GetComponent<MyCharacter>().parameter.pic = Resources.Load<Sprite>("[VerArc Stash] Mini_Characters/" + picName);
      characters[id].GetComponent<MyCharacter>().findObject("Image").GetComponent<Image>().sprite = characters[id].GetComponent<MyCharacter>().parameter.pic;

      characters[id].GetComponent<MyCharacter>().parameter.hp = hp;
      characters[id].GetComponent<MyCharacter>().status.curHp = hp;
      characters[id].GetComponent<MyCharacter>().status.maxHp = hp;

      characters[id].GetComponent<MyCharacter>().parameter.mp = mp;
      characters[id].GetComponent<MyCharacter>().status.curMp = mp;
      characters[id].GetComponent<MyCharacter>().status.maxMp = mp;

      characters[id].GetComponent<MyCharacter>().parameter.atk = atk;
      characters[id].GetComponent<MyCharacter>().status.curAtk = atk;

      characters[id].GetComponent<MyCharacter>().parameter.def = def;
      characters[id].GetComponent<MyCharacter>().status.curDef = def;

      characters[id].GetComponent<MyCharacter>().parameter.spd = spd;
      characters[id].GetComponent<MyCharacter>().status.curSpd = spd;

      characters[id].GetComponent<MyCharacter>().status.buff = new ArrayList();
    }

    private void parsePassiveBuff()
    {
      for(int i = 0; i < characters.Length; i += 1)
      {
        if(isActive[i])
        {
          for(int j = 0; j < characters[i].GetComponent<MyCharacter>().parameter.skills.Length; j += 1)
          {
            if(characters[i].GetComponent<MyCharacter>().parameter.skills[j].isPassive)
            {
              castSkill(characters[i].GetComponent<MyCharacter>().parameter.skills[j].id, i, getTarget(i, characters[i].GetComponent<MyCharacter>().parameter.skills[j].id));
            }
          }
        }
      }
    }

    private void initTurnPosition()
    {
      for(int i = 0; i < characters.Length; i += 1)
      {
        if(isActive[i])
        {
          characters[i].GetComponent<MyCharacter>().status.curPos = Random.Range(20, 90);
        }
      }

      refreshTimeLine();
    }

    private void refreshTimeLine()
    {
      ArrayList timeLineObj = new ArrayList();

      for(int i = 0; i < characters.Length; i += 1)
      {
        if(isActive[i])
        {
          TimeLineObject obj = new TimeLineObject();
          obj.curDis = characters[i].GetComponent<MyCharacter>().status.curPos;
          obj.speed = characters[i].GetComponent<MyCharacter>().status.curSpd;
          obj.index = i;

          timeLineObj.Add(obj);
        }
      }

      for(int i = 0; i < 7; i += 1)
      {
        float min = 8753;
        int minIndex = -1;

        for(int j = 0; j < timeLineObj.Count; j += 1)
        {
          TimeLineObject curObj = (TimeLineObject) timeLineObj[j];
          float curReqTime = curObj.curDis / curObj.speed;

          if(min > curReqTime)
          {
            min = curReqTime;
            minIndex = j;
          }
        }

        timeLine[i].GetComponent<Image>().sprite = characters[((TimeLineObject)timeLineObj[minIndex]).index].GetComponent<MyCharacter>().parameter.pic;
        ((TimeLineObject)timeLineObj[minIndex]).curDis += 100;
      }
    }

    private void updateTurnPosition()
    {
      int curGameStatus = checkGameStatus();
      if(curGameStatus != 0)
      {
        gameOver(curGameStatus);

        return;
      }

      //avoid loop
      curTurn += 1;

      if(curTurn > maxTurn)
      {
        return;
      }

      float min = 8753;
      int minIndex = -1;

      for(int i = 0; i < characters.Length; i += 1)
      {
        if(isActive[i])
        {
          float time = characters[i].GetComponent<MyCharacter>().status.curPos / characters[i].GetComponent<MyCharacter>().status.curSpd;

          if(min > time)
          {
            min = time;
            minIndex = i;
          }
        }
      }

      for(int i = 0; i < characters.Length; i += 1)
      {
        if(isActive[i])
        {
          characters[i].GetComponent<MyCharacter>().status.curPos -= characters[i].GetComponent<MyCharacter>().status.curSpd * min;
        }
      }

      characters[minIndex].GetComponent<MyCharacter>().status.curPos = 0;
      curCharacterID = minIndex;

      decreaseSkillCoolDown(curCharacterID, 1);
      refreshTimeLine();

      if(minIndex < 5)
      {
        GetComponent<EffectParser>().parseStartTurnBuff(characters[curCharacterID]);
        showCommandLayout(true);
        characters[minIndex].GetComponent<MyCharacter>().findObject("Background").SetActive(true);
        characters[minIndex].GetComponent<MyCharacter>().findObject("Background").GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);

        //use in tutorial B
        if(SceneManager.GetActiveScene().name == "SceneB")
        {
          showCommandLayout(false);
          GetComponent<TutorialB>().showMessage(minIndex);
        }
      }
      else
      {
        monsterMovement();
        pauseBetweenTurn();
      }
    }

    private void monsterMovement()
    {
      characters[curCharacterID].GetComponent<MyCharacter>().status.curPos = 100;

      //AI for monster movement
      if(GetComponent<EffectParser>().isInControl(characters[curCharacterID]))
      {
        GetComponent<EffectParser>().parseStartTurnBuff(characters[curCharacterID]);

        return;
      }

      GetComponent<EffectParser>().parseStartTurnBuff(characters[curCharacterID]);

      for(int i = 0; i < characters[curCharacterID].GetComponent<MyCharacter>().status.skillsCoolDown.Length; i += 1)
      {
        //cast the skill whenever the skill is ready
        //need MP
        //not passive
        if(characters[curCharacterID].GetComponent<MyCharacter>().status.skillsCoolDown[i] == 0 &&
          characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].mpCost <= characters[curCharacterID].GetComponent<MyCharacter>().status.curMp &&
          !characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].isPassive)
        {
          //set cool down, decrease current mp
          characters[curCharacterID].GetComponent<MyCharacter>().status.skillsCoolDown[i] = characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].cooldown;
          characters[curCharacterID].GetComponent<MyCharacter>().status.curMp -= characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].mpCost;

          //use skill
          castSkill(characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].id, curCharacterID, getTarget(curCharacterID, characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].id));

          return;
        }
      }

      //if no skill is ready, use normal Attack
      castSkill(0, curCharacterID, getTarget(curCharacterID, 0));
    }

    private void castSkill(int skillID, int selfIndex, int[] targetIndex)
    {
      //Debug.Log(selfIndex + " -> " + targetIndex[0] + " with " + getSkillInfo(skillID).name);

      for(int i = 0; i < targetIndex.Length; i += 1)
      {
        GetComponent<EffectParser>().castSkill(skillID, characters[selfIndex], characters[targetIndex[i]]);
      }
    }

    public void decreaseSkillCoolDown(int targetIndex, int amount)
    {
      for(int i = 0; i < characters[targetIndex].GetComponent<MyCharacter>().status.skillsCoolDown.Length; i += 1)
      {
        characters[targetIndex].GetComponent<MyCharacter>().status.skillsCoolDown[i] = Mathf.Max(characters[targetIndex].GetComponent<MyCharacter>().status.skillsCoolDown[i] - amount, 0);
      }
    }

    public void showCommandLayout(bool isShow)
    {
      attackButton.gameObject.SetActive(isShow);
      skillButton.gameObject.SetActive(isShow);
      defenceButton.gameObject.SetActive(isShow);
      itemButton.gameObject.SetActive(isShow);
    }

    private void pauseBetweenTurn()
    {
      isWaiting = true;
      waitingTime = 0f;
    }

    public bool containsBuff(int index, int buffID)
    {
      for(int i = 0; i < characters[index].GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
      //  Debug.Log(((Buff)characters[index].GetComponent<MyCharacter>().status.buff[i]).id);
        if(((Buff)characters[index].GetComponent<MyCharacter>().status.buff[i]).id == buffID)
        {
          return true;
        }
      }

      return false;
    }

    /**
     *
     *  choose a target randomly
     *
     **/
    public int[] getTarget(int id, int skillID)
    {
      int type = ((SkillAbility)getSkillInfo(skillID)).type;
      int targetCount = ((SkillAbility)getSkillInfo(skillID)).targetCount;

      ArrayList index = new ArrayList();
      ArrayList target = new ArrayList();

      //if target is self
      if(type == 0)
      {
        index.Add(id);
      }
      //if target is ally, random choose one
      if(type == 1)
      {
        if(id < 5)
        {
          for(int i = 0; i < 5; i += 1)
          {
            if(isActive[i])
            {
              index.Add(i);
            }
          }
        }
        else
        {
          for(int i = 5; i < 10; i += 1)
          {
            if(isActive[i])
            {
              index.Add(i);
            }
          }
        }
      }
      //if target is enemy, random choose one
      else if(type == 2)
      {
        if(id < 5)
        {
          for(int i = 5; i < 10; i += 1)
          {
            if(isActive[i])
            {
              index.Add(i);
            }
          }
        }
        else
        {
          for(int i = 0; i < 5; i += 1)
          {
            if(isActive[i])
            {
              index.Add(i);
            }
          }
        }
      }
      else if(type == 3)
      {
        for(int i = 0; i < 10; i += 1)
        {
          if(isActive[i])
          {
            index.Add(i);
          }
        }
      }

      int maxCount = index.Count;

      do
      {
        int curIndex = Random.Range(0, index.Count);
        //Debug.Log(curIndex + " " + type + " " + target.Count + " " + index.Count);

        //if in taunt
        for(int i = 0; i < index.Count; i += 1)
        {
          if(containsBuff((int)index[i], 1))
          {
            curIndex = i;
            break;
          }
        }

        target.Add(index[curIndex]);
        index.RemoveAt(curIndex);
      }
      while(target.Count != maxCount && target.Count < targetCount);

      int[] targetArray = new int[target.Count];

      for(int i = 0; i < target.Count; i += 1)
      {
        targetArray[i] = (int)target[i];
      }

      return targetArray;
    }

    private void gameOver(int result)
    {
      if(SceneManager.GetActiveScene().name == "SceneA" || SceneManager.GetActiveScene().name == "SceneB")
      {
        nextLevelLayout.SetActive(true);

        if(result == -1)
        {
          nextLevelText.GetComponent<TextMeshProUGUI>().text = "All dead\nTry Again";
        }
        else if(result == 1)
        {
          nextLevelText.GetComponent<TextMeshProUGUI>().text = "Win\nNext Level";
        }

        nextLevelButton.onClick.AddListener(
          delegate
          {
            if(sceneController.GetComponent<SceneController>().level == 0)
            {
              if(result == -1)
              {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
              }
              else if(result == 1)
              {
                SceneManager.LoadScene("Level1");
              }
            }

          }
        );
      }
      else if(SceneManager.GetActiveScene().name == "Level1")
      {
        nextLevelText.SetActive(true);

        if(result == -1)
        {
          float score = characters[6].GetComponent<MyCharacter>().status.curHp / characters[6].GetComponent<MyCharacter>().status.maxHp;
          string degree = "";

          if(score > 0.8f)
          {
            degree = "C";
          }
          else if(score > 0.5f)
          {
            degree = "B";
          }
          else
          {
            degree = "A";
          }

          nextLevelText.GetComponent<TextMeshProUGUI>().text += " " + degree;
        }
        else if(result == 1)
        {
          nextLevelText.GetComponent<TextMeshProUGUI>().text += " S";
        }
      }
      else
      {
        exitBattle();
      }

    }

    //side: -1: enemy 1: ally
    private int getAliveCount(int side)
    {
      int count = 0;

      if(side == 1)
      {
        for (int i = 0; i < 5; i += 1)
        {
          if(isActive[i])
          {
            count += 1;
          }
        }
      }
      else if(side == -1)
      {
        for (int i = 5; i < 10; i += 1)
        {
          if(isActive[i])
          {
            count += 1;
          }
        }
      }

      return count;
    }

    /**
     *
     * -1 lose 0 nothing 1 win
     *
     **/
    private int checkGameStatus()
    {
      if(getAliveCount(-1) == 0)
      {
        return 1;
      }

      if(getAliveCount(1) == 0)
      {
        return -1;
      }

      return 0;
    }

    public void changeSkill(int index, int skillSlotIndex, int skillID)
    {
      if(skillID != -1)
      {
        characters[index].GetComponent<MyCharacter>().parameter.skills[skillSlotIndex] = getSkillInfo(skillID);
        characters[index].GetComponent<MyCharacter>().status.skillsCoolDown[skillSlotIndex] = 0;

        if(getSkillInfo(skillID).isPassive)
        {
          castSkill(skillID, index, getTarget(index, skillID));
        }
      }
      else
      {
        SkillAbility[] copyAbilitiy = characters[index].GetComponent<MyCharacter>().parameter.skills;
        int[] skillsCoolDown = characters[index].GetComponent<MyCharacter>().status.skillsCoolDown;

        characters[index].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[copyAbilitiy.Length - 1];
        characters[index].GetComponent<MyCharacter>().status.skillsCoolDown = new int[skillsCoolDown.Length - 1];

        for(int i = 0, j = 0; i < copyAbilitiy.Length; i += 1)
        {
          if(i != skillSlotIndex)
          {
            characters[index].GetComponent<MyCharacter>().parameter.skills[j] = copyAbilitiy[i];
            characters[index].GetComponent<MyCharacter>().status.skillsCoolDown[j] = skillsCoolDown[i];

            j += 1;
          }
        }
      }
    }

    public int summonRandomly(int curIndex, string name, string picName, int hp, int mp, int atk, int def, int spd, int[] skillsID)
    {
      if(curIndex < 5)
      {
        for(int i = 0; i < 5; i += 1)
        {
          if(!isActive[i])
          {
            summon(i, name, picName, hp, mp, atk, def, spd, skillsID);

            return i;
          }
        }
      }
      else if(curIndex >= 5)
      {
        for(int i = 5; i < 10; i += 1)
        {
          if(!isActive[i])
          {
            summon(i, name, picName, hp, mp, atk, def, spd, skillsID);

            return i;
          }
        }
      }

      return -1;
    }

    public void summon(int id, string name, string picName, int hp, int mp, int atk, int def, int spd, int[] skillsID)
    {
      characters[id].SetActive(true);
      isActive[id] = true;

      initParameterInfo(id, name, picName, hp, mp, atk, def, spd);

      characters[id].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[skillsID.Length];
      characters[id].GetComponent<MyCharacter>().status.skillsCoolDown = new int[skillsID.Length];

      for(int i = 0; i < skillsID.Length; i += 1)
      {
        characters[id].GetComponent<MyCharacter>().parameter.skills[i] = getSkillInfo(skillsID[i]);

        if(getSkillInfo(skillsID[i]).isPassive)
        {
          castSkill(skillsID[i], id, getTarget(id, skillsID[i]));
        }
      }
    }

    public void summonMonster(int id)
    {
      switch(id)
      {
        case 0: summonRandomly(6, "Shi Lai Mu", "Shi Lai Mu", 500, 100, 100, 40, 25, new int[]{31, 32}); break;
        case 1: summonRandomly(6, "Can Speed Up", "Can SpeedUp", 300, 50, 75, 30, 40, new int[]{33, 20}); break;
        case 2: summonRandomly(6, "Can Fire Magic", "Can Fire Magic", 750, 300, 125, 30, 50, new int[]{34}); break;
        case 3: summonRandomly(6, "Can Buff", "Can Buff", 500, 100, 100, 40, 25, new int[]{35, 36, 37}); break;
        case 4: summonRandomly(6, "Hi", "Hi", 500, 0, 0, 200, 50, new int[]{38, 39}); break;
      }
    }

    public int[] getRandomMonsterPair()
    {
      int[][] pairs = new int[9][]
      {
        new int[]{0, 2, 1},
        new int[]{2, 2},
        new int[]{3, 2, 3},
        new int[]{0, 4},
        new int[]{4, 4},
        new int[]{3, 1, 2},
        new int[]{0, 0},
        new int[]{3, 4, 1},
        new int[]{0, 3}
      };

      return pairs[Random.Range(0, 9)];
    }

    private SkillAbility getSkillInfo(int id)
    {
      switch(id)
      {
        case 0: return new SkillAbility(0, "Normal Attack", "Give 100% ATK Damage the enemy", 0, 0, 2, 1, false);
        case 1: return new SkillAbility(1, "Taunt", "Force the enemy attack this unit and Decrease 40% Damage in 2 turn", 10, 5, 0, 1, false);
        case 2: return new SkillAbility(2, "ATK UP", "In 5 turns, whenever received damage by enemy, increase 125% ATK for 3 turns", 5, 10, 0, 1, false);
        case 3: return new SkillAbility(3, "HP Regeneration", "Recovery 5% hp every turn", 0, 0, 0, 1, true);
        case 4: return new SkillAbility(4, "Doppelgänger", "Give 100% ATK damage in each turn", 15, 0, 2, 1, false);
        case 5: return new SkillAbility(5, "ATK+", "Increase 3% ATK every turn", 0, 0, 0, 1, true);
        case 6: return new SkillAbility(6, "Healing", "Recovery 25% Hp\nRecovery 25% Hp in 5 turns", 30, 2, 1, 1, false);
        case 7: return new SkillAbility(7, "MP Regeneration", "Recovery 3% Mp in each turn", 0, 0, 0, 1, true);
        case 8: return new SkillAbility(8, "Charge", "Give 400% ATK Damage after 2 turns stand by", 80, 5, 0, 1, false);
        case 9: return new SkillAbility(9, "Bersaka", "Increase 25% All Parameter when Self Hp below 30%", 0, 0, 0, 1, true);
        case 10: return new SkillAbility(10, "Effect: ATK UP", "triggers by hit", 0, 0, 0, 1, false);
        case 11: return new SkillAbility(11, "Effect: Charge", "charge end", 0, 0, 2, 1, false);
        case 12: return new SkillAbility(12, "Effect: Bersaka", "basaka trigger", 0, 0, 0, 1, false);
        case 13: return new SkillAbility(13, "Effect: IsCharging", "charge process", 0, 0, 0, 1, false);
        case 14: return new SkillAbility(14, "Defence", "Increase 50% DEF for 1 turn", 0, 0, 0, 1, false);
        case 15: return new SkillAbility(15, "Elementa Mixtio", "Give Element Blend Debuff to All Units\n Element Blend: The skill effect enhances/inverts on the target with following/opposite element\nThe element will exchange after receiving effect", 400, 999, 0, 1, false);
        case 16: return new SkillAbility(16, "Element Immune", "The skill effect has no effect by Elementa Mixtio", 0, 0, 0, 1, true);
        case 17: return new SkillAbility(17, "Summon", "Summon two units with Elementa Mixtio", 0, 999, 0, 1, false);
        case 18: return new SkillAbility(18, "Effect: summon with Elementa Mixtio", "triggers when respawn", 0, 0, 0, 1, true);
        case 19: return new SkillAbility(19, "Element Change", "Trigger Elementa Mixtio\n Recover 100 AT", 100, 2, 0, 1, false);
        case 20: return new SkillAbility(20, "MP Recover+", "Recover 90% MP\n Recover 50 AT", 0, 0, 0, 1, false);
        case 21: return new SkillAbility(21, "Imprison", "Imprison the enemy for 3 turns", 250, 10, 2, 1, false);
        case 22: return new SkillAbility(22, "Big AOE", "Give 250%ATK Damage to all enemies", 350, 7, 2, 999, false);
        case 23: return new SkillAbility(23, "Charge Ver2.", "After 1 turns stand by:\n Increase 500% Vulnerabitliy to the enemy for 3 turns\nGive 150% Damage to the enemy", 80, 2, 0, 1, false);
        case 24: return new SkillAbility(24, "Effect: Charge Ver2.", "charge end", 0, 0, 2, 1, false);
        case 25: return new SkillAbility(25, "Effect: cant move", "Trigger called when cant move", 0, 0, 0, 1, false);
        case 26: return new SkillAbility(26, "Effect: Summon Phase", "phase switch", 0, 0, 0, 1, false);
        case 27: return new SkillAbility(27, "Effect: Element change Phase", "phase switch", 0, 0, 0, 1, false);
        case 28: return new SkillAbility(28, "MP Recover", "Recover 90% MP", 0, 0, 0, 1, false);
        case 29: return new SkillAbility(29, "Summoner", "Switch to Summon Phase when Self Hp below 80%", 0, 0, 0, 1, true);
        case 30: return new SkillAbility(30, "Blast", "Switch to Blast Phase when Self Hp below 50%", 0, 0, 0, 1, true);

        case 31: return new SkillAbility(31, "Split", "Create a perfect clone of self", 10, 5, 0, 1, false);
        case 32: return new SkillAbility(32, "Erosion", "Give 150%ATK Damage to the enmey\nIncrease 25% Vulnerabitliy to the enemy for 5 turns", 5, 2, 2, 1, false);
        case 33: return new SkillAbility(33, "SpeedUp", "Decrease 1 turns Skill Cooldown for all allies\nIncrease 50% Speed for all allies for 5 turns", 20, 4, 1, 999, false);
        case 34: return new SkillAbility(34, "Fire Magic", "Give 150%ATK Damage to all enemies", 60, 0, 2, 999, false);
        case 35: return new SkillAbility(35, "Enhance: ATK", "Increase 50% ATK for all allies for 10 turns", 15, 5, 1, 999, false);
        case 36: return new SkillAbility(36, "Enhance: DEF", "Decrease 25% Vulnerabitliy for all allies for 10 turns", 15, 10, 1, 999, false);
        case 37: return new SkillAbility(37, "Group HP Recover", "Recover 25% HP for all allies", 10, 3, 1, 999, false);
        case 38: return new SkillAbility(38, "Break AT", "Increase 300 AT to all enemies after 5 turns stand by", 0, 0, 0, 1, false);
        case 39: return new SkillAbility(39, "Erosive Aura", "Aura: All enemies decrease 5% Current HP for each turn", 0, 0, 0, 1, true);
        case 40: return new SkillAbility(40, "Effect: Break AT End", "-AT", 0, 0, 2, 999, false);
        //public SkillAbility(int id, string name, string des, int mpCost, int cooldown, int type, int targetCount, bool isPassive)
        //item:
        case 1000: return new SkillAbility(1000, "Bread", "Recover 80% HP for the ally", 0, 0, 1, 1, false);
        case 1001: return new SkillAbility(1001, "Pineapple", "Recover 50% MP for the ally", 0, 0, 1, 1, false);
      }

      return null;
    }

    private class TimeLineObject
    {
      public float curDis;
      public float speed;
      public int index;
    }
}
