using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Controller : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject[] timeLine;

    public Button attackButton;
    public Button skillButton;
    public Button defenceButton;

    public GameObject skillLayout;
    public Button[] skillButtons;
    public GameObject skillDesLayout;
    public GameObject skillDesText;

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

            //set cool down and mp cost
            for(int j = 0; j < characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills.Length; j += 1)
            {
              if(characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[j].id == curSkillID)
              {
                characters[curCharacterID].GetComponent<MyCharacter>().status.skillsCoolDown[j] = characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[j].cooldown;
                break;
              }
            }

            characters[curCharacterID].GetComponent<MyCharacter>().status.curMp -= ((SkillAbility)getSkillInfo(curSkillID)).mpCost;

            //check if wrong target
            if(((SkillAbility)getSkillInfo(curSkillID)).type == 0)
            {
              castSkill(curSkillID, curCharacterID, curCharacterID);
            }
            else if(((SkillAbility)getSkillInfo(curSkillID)).type == 1 && finalI >= 5 || ((SkillAbility)getSkillInfo(curSkillID)).type == 2 && finalI < 5)
            {
              return;
            }
            else
            {
              castSkill(curSkillID, curCharacterID, finalI);
            }

            characters[curCharacterID].GetComponent<MyCharacter>().status.curPos = 100;
            characters[curCharacterID].GetComponent<MyCharacter>().findObject("Background").SetActive(false);

            inTargeting = false;
            showCommandLayout(false);
            skillDesLayout.SetActive(false);

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

      skillButton.onClick.AddListener(
        delegate
        {
          setupSkillList(curCharacterID);

          showCommandLayout(false);
          skillLayout.SetActive(true);
        }
      );

      for(int i = 0; i < 6; i += 1)
      {
        int finalI = i;

        skillButtons[i].onClick.AddListener(
          delegate
          {
            skillDesLayout.SetActive(true);
            skillDesText.GetComponent<TextMeshProUGUI>().text = characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[finalI].des;

            if(!characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[finalI].isPassive &&
              characters[curCharacterID].GetComponent<MyCharacter>().status.skillsCoolDown[finalI] == 0 &&
              characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[finalI].mpCost <= characters[curCharacterID].GetComponent<MyCharacter>().status.curMp)
            {
              curSkillID = characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[finalI].id;
              inTargeting = true;

              skillLayout.SetActive(false);
            }
          }
        );
      }


      startLevel1();
      pauseBetweenTurn();

      //run the game
      parsePassiveBuff();
      updateTurnPosition();
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

          if(!characters[index].GetComponent<MyCharacter>().parameter.skills[i].isPassive)
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
              castSkill(characters[i].GetComponent<MyCharacter>().parameter.skills[j].id, i, getTarget(characters[i].GetComponent<MyCharacter>().parameter.skills[j].type));
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
      //Debug.Log(selfIndex + " -> " + targetIndex + " with " + getSkillInfo(skillID).name);

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

    private bool containsBuff(int unitID, int buffID)
    {
      for(int i = 0; i < characters[unitID].GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        if(((Buff)characters[unitID].GetComponent<MyCharacter>().status.buff[i]).id == buffID)
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

      //if in taunt
      for(int i = 0; i < index.Count; i += 1)
      {
        if(containsBuff((int)index[i], 1))
        {
          return (int)index[i];
        }
      }

      return (int)index[Random.Range(0, index.Count)];
    }

    private SkillAbility getSkillInfo(int id)
    {
      switch(id)
      {
        case 0: return new SkillAbility(0, "Normal Attack", "Give 100% ATK Damage the enemy", 0, 0, 2, false);
        case 1: return new SkillAbility(1, "Taunt", "Force the enemy attack this unit and Decrease 40% Damage in 2 turn", 20, 5, 0, false);
        case 2: return new SkillAbility(2, "ATK UP", "In 5 turns, whenever received damage by enemy, increase 50% ATK for 3 turns", 15, 10, 0, false);
        case 3: return new SkillAbility(3, "HP Regeneration", "Recovery 5% hp every turn", 0, 0, 0, true);
        case 4: return new SkillAbility(4, "Doppelgänger", "Give 50% ATK damage in each turn", 50, 5, 2, false);
        case 5: return new SkillAbility(5, "ATK+", "Increase 3% ATK every turn", 0, 0, 0, true);
        case 6: return new SkillAbility(6, "Healing", "Recovery 25% Hp\nRecovery 25% Hp in 5 turns", 30, 3, 1, false);
        case 7: return new SkillAbility(7, "MP Regeneration", "Recovery 3% Mp in each turn", 0, 0, 0, true);
        case 8: return new SkillAbility(8, "Charge", "Give 400% ATK Damage after 2 turns stand by", 80, 5, 0, false);
        case 9: return new SkillAbility(9, "Bersaka", "Increase 25% All Parameter when Self Hp below 30%", 0, 0, 0, true);
        case 10: return new SkillAbility(10, "Effect: ATK UP", "triggers by hit", 0, 0, 0, false);
        case 11: return new SkillAbility(11, "Effect: Charge", "charge end", 0, 0, 0, false);
        case 12: return new SkillAbility(12, "Effect: Bersaka", "basaka trigger", 0, 0, 0, false);
        case 13: return new SkillAbility(13, "Effect: IsCharging", "charge process", 0, 0, 0, false);
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
