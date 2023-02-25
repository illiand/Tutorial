using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject[] timeLine;

    public Button attackButton;
    public Button skillButton;
    public Button defenceButton;

    public bool[] isActive = new bool[10];

    //the unit in movement
    private int curCharacterID;

    //the skill player in use
    private int curSkillID;

    private bool inTargeting;
    private bool isPlayerTurn;

    private bool isWaiting;
    private float waitingTime;

    private int curTurn = 0;
    private int maxTurn = 100;

    // Start is called before the first frame update
    void Start()
    {
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
              return;
            }

            castSkill(curSkillID, curCharacterID, finalI);

            characters[curCharacterID].GetComponent<MyCharacter>().status.curPos = 100;
            characters[curCharacterID].GetComponent<MyCharacter>().findObject("Background").SetActive(false);

            showCommandLayout(false);
            updateTurnPosition();
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

      startLevel1();
      pauseBetweenTurn();

      //run the game
      updateTurnPosition();
    }

    void Update()
    {
      if(inTargeting && Input.GetMouseButtonDown(1))
      {
        inTargeting = false;
        showCommandLayout(true);
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


    private void setUpSkillList(int i)
    {
    //    characters[i].GetComponent<EffectParser>().skillList(i);
    }

    private void startLevel1()
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

      initParameterInfo(0, "Tank", "char_01", 2000, 70, 100, 100, 35);
      initParameterInfo(1, "Dps", "char_02", 1200, 100, 200, 30, 40);
      initParameterInfo(2, "Healer", "char_03", 1200, 150, 125, 50, 35);
      initParameterInfo(5, "BOSS", "char_18", 10000, 500, 300, 25, 80);

      characters[0].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(0), getSkillInfo(3)};
      characters[0].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      characters[1].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(0), getSkillInfo(3)};
      characters[1].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      characters[2].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(0), getSkillInfo(3)};
      characters[2].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      characters[5].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(8), getSkillInfo(9)};
      characters[5].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      initTurnPosition();
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
        showCommandLayout(true);
        characters[minIndex].GetComponent<MyCharacter>().findObject("Background").SetActive(true);
        characters[minIndex].GetComponent<MyCharacter>().findObject("Background").GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);
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
          characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].mpCost < characters[curCharacterID].GetComponent<MyCharacter>().status.curMp &&
          !characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].isPassive)
        {
          //set cool down, decrease current mp
          characters[curCharacterID].GetComponent<MyCharacter>().status.skillsCoolDown[i] = characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].cooldown;
          characters[curCharacterID].GetComponent<MyCharacter>().status.curMp -= characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].mpCost;

          //use skill
          castSkill(characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].id, curCharacterID, getTarget(characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].type));

          return;
        }
      }

      //if no skill is ready, use normal Attack
      castSkill(0, curCharacterID, getTarget(2));
    }

    private void castSkill(int skillID, int selfIndex, int targetIndex)
    {
      Debug.Log(selfIndex + " -> " + targetIndex + " with " + getSkillInfo(skillID).name);

      GetComponent<EffectParser>().castSkill(skillID, characters[selfIndex], characters[targetIndex]);
    }

    private void decreaseSkillCoolDown(int targetIndex, int amount)
    {
      for(int i = 0; i < characters[targetIndex].GetComponent<MyCharacter>().status.skillsCoolDown.Length; i += 1)
      {
        characters[targetIndex].GetComponent<MyCharacter>().status.skillsCoolDown[i] = Mathf.Max(characters[targetIndex].GetComponent<MyCharacter>().status.skillsCoolDown[i] - amount, 0);
      }
    }

    private void showCommandLayout(bool isShow)
    {
      attackButton.gameObject.SetActive(isShow);
      skillButton.gameObject.SetActive(isShow);
      defenceButton.gameObject.SetActive(isShow);
    }

    private void pauseBetweenTurn()
    {
      isWaiting = true;
      waitingTime = 0f;
    }

    /**
     *
     *  choose a target randomly
     *
     **/
    public int getTarget(int type)
    {
      ArrayList index = new ArrayList();

      //if target is self
      if(type == 0)
      {
        index.Add(curCharacterID);
      }
      //if target is ally, random choose one
      else if(type == 1)
      {
        if(curCharacterID < 5)
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
        if(curCharacterID < 5)
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

      return (int)index[Random.Range(0, index.Count)];
    }

    private SkillAbility getSkillInfo(int id)
    {
      switch(id)
      {
        case 0: return new SkillAbility(0, "Normal Attack", 0, 0, 2, false);
        case 1: return new SkillAbility(1, "Taunt", 20, 7, 0, false);
        case 2: return new SkillAbility(2, "ATK UP", 15, 10, 0, false);
        case 3: return new SkillAbility(3, "HP Regeneration", 0, 0, 0, true);
        case 4: return new SkillAbility(4, "Doppelgänger", 50, 10, 0, false);
        case 5: return new SkillAbility(5, "ATK+", 0, 0, 0, true);
        case 6: return new SkillAbility(6, "Healing", 30, 2, 1, false);
        case 7: return new SkillAbility(7, "MP Regeneration", 0, 0, 0, true);
        case 8: return new SkillAbility(8, "Charge", 80, 5, 0, false);
        case 9: return new SkillAbility(9, "Bersaka", 0, 0, 0, true);
        case 10: return new SkillAbility(10, "Effect: ATK UP", 0, 0, 0, false);
        case 11: return new SkillAbility(11, "Effect: Charge", 0, 0, 0, false);
        case 12: return new SkillAbility(12, "Effect: Bersaka", 0, 0, 0, false);
        case 13: return new SkillAbility(13, "Effect: IsCharging", 0, 0, 0, false);
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
