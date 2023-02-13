using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public GameObject[] characters;
    private bool[] isActive = new bool[10];

    private int curCharacterID;
    private int curTargetID;
    private int curSkillID;

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

        characters[i].GetComponent<MyCharacter>().findObject("Image").GetComponent<Button>().onClick.AddListener(
          delegate
          {
            curTargetID = finalI;
          }
        );
      }

      startLevel1();
    }

    private void startLevel1()
    {
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

      initParameterInfo(0, "Tank", 2000, 70, 100, 100, 35);
      initParameterInfo(1, "Dps", 1200, 100, 200, 30, 40);
      initParameterInfo(2, "Healer", 1200, 150, 125, 50, 35);
      initParameterInfo(5, "BOSS", 10000, 500, 300, 25, 80);

      characters[0].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(0), getSkillInfo(3)};
      characters[0].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      characters[1].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(0), getSkillInfo(3)};
      characters[1].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      characters[2].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(0), getSkillInfo(3)};
      characters[2].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      characters[5].GetComponent<MyCharacter>().parameter.skills = new SkillAbility[]{getSkillInfo(1), getSkillInfo(2)};
      characters[5].GetComponent<MyCharacter>().status.skillsCoolDown = new int[]{0, 0};

      initTurnPosition();
    }

    private void initParameterInfo(int id, string name, int hp, int mp, int atk, int def, int spd)
    {
      characters[id].GetComponent<MyCharacter>().parameter = new Parameter();
      characters[id].GetComponent<MyCharacter>().status = new BattleStatus();

      characters[id].GetComponent<MyCharacter>().parameter.name = name;

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
          characters[i].GetComponent<MyCharacter>().status.turnPosition = Random.Range(20, 90);
        }
      }
    }

    private void updateTurnPosition()
    {
      float min = 8753;
      int minIndex = -1;

      for(int i = 0; i < characters.Length; i += 1)
      {
        if(isActive[i])
        {
          float time = characters[i].GetComponent<MyCharacter>().status.turnPosition / characters[i].GetComponent<MyCharacter>().status.curSpd;

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
          characters[i].GetComponent<MyCharacter>().status.turnPosition -= characters[i].GetComponent<MyCharacter>().status.curSpd * min;
        }
      }

      characters[minIndex].GetComponent<MyCharacter>().status.turnPosition = 0;
      curCharacterID = minIndex;

      if(minIndex < 5)
      {
        //TODO Player movement

      }
      else
      {
        monsterMovement();
        updateTurnPosition();
      }
    }

    private void monsterMovement()
    {
      //AI for monster movement
      if(inControl(characters[curCharacterID]))
      {
        parseStartTurnBuff(characters[curCharacterID]);

        return;
      }

      parseStartTurnBuff(characters[curCharacterID]);

      int[] skillsCoolDown = characters[curCharacterID].GetComponent<MyCharacter>().status.skillsCoolDown;
      for(int i = 0; i < skillsCoolDown.Length; i += 1)
      {
        //cast the skill whenever the skill is ready
        //need MP
        if(skillsCoolDown[i] == 0 && characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].mpCost < characters[curCharacterID].GetComponent<MyCharacter>().status.curMp)
        {
          getTarget(characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].type);
          curSkillID = characters[curCharacterID].GetComponent<MyCharacter>().parameter.skills[i].id;

          castSkill();
          return;
        }
      }

      //if no skill is ready, use normal Attack
      getTarget(2);
      curSkillID = 0;

      castSkill();
    }

    private void castSkill()
    {
        characters[curCharacterID].GetComponent<Skill>().castSkill(curSkillID, characters[curCharacterID], characters[curTargetID]);
    }

    /**
     *
     * if the unit is in debuff may skip turn
     *
     **/
    private bool inControl(GameObject target)
    {
      for(int i = 0; i < target.GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id == 8)
        {
          //charge skill
          if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).remainingTurn == 1)
          {
            getTarget(2);
            curSkillID = 11;
          }

          return true;
        }
      }

      return false;
    }

    /**
     *
     * after the turn start, Buffs trigger
     *
     **/
    private void parseStartTurnBuff(GameObject target)
    {
      for(int i = 0; i < target.GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        switch(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id)
        {
          case 3:
            target.GetComponent<MyCharacter>().status.curHp += target.GetComponent<MyCharacter>().status.maxHp * 0.05f;
            target.GetComponent<Skill>().normalizeHPMP();

            break;
          case 5:
            target.GetComponent<MyCharacter>().status.curAtk += target.GetComponent<MyCharacter>().parameter.atk * 0.03f;
            break;

          case 6:
            target.GetComponent<MyCharacter>().status.curHp += target.GetComponent<MyCharacter>().status.maxHp * 0.05f;
            target.GetComponent<Skill>().normalizeHPMP();
            break;

          case 7:
            target.GetComponent<MyCharacter>().status.curMp += target.GetComponent<MyCharacter>().status.maxMp * 0.05f;
            target.GetComponent<Skill>().normalizeHPMP();
            break;
        }

        ((Buff)target.GetComponent<MyCharacter>().status.buff[i]).remainingTurn -= 1;

        if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).remainingTurn == 0)
        {
          //remove parameter increase/decrease effect
          switch(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id)
          {
            case 10:
              target.GetComponent<MyCharacter>().status.curAtk -= target.GetComponent<MyCharacter>().parameter.atk * 0.1f;
              break;
          }
          target.GetComponent<MyCharacter>().status.buff.RemoveAt(i);
          i -= 1;
        }

      }
    }

    /**
     *
     * after the turn end Buffs trigger
     *
     **/
    private void parseEndTurnBuff(GameObject target)
    {

    }

    /**
     *
     *  choose a target randomly
     *
     **/
    private void getTarget(int type)
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

      curTargetID = (int) index[Random.Range(0, index.Count)];
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
        case 8: return new SkillAbility(8, "Charge", 0, 0, 0, false);
        case 9: return new SkillAbility(9, "Bersaka", 0, 0, 0, true);
      }

      return null;
    }
}
