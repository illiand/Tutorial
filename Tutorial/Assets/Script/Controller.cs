using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public GameObject[] characters;

    private int curCharacterID;
    private int curTargetID;
    private int curSkillID;

    // Start is called before the first frame update
    void Start()
    {
      for(int i = 0; i < 10; i += 1)
      {
        Material hpInstance = Instantiate(characters[i].GetComponent<MyCharacter>().findObject("HP").GetComponent<Image>().material);
        Material mpInstance = Instantiate(characters[i].GetComponent<MyCharacter>().findObject("MP").GetComponent<Image>().material);

        characters[i].GetComponent<MyCharacter>().findObject("HP").GetComponent<Image>().material = hpInstance;
        characters[i].GetComponent<MyCharacter>().findObject("MP").GetComponent<Image>().material = mpInstance;

        characters[i].GetComponent<MyCharacter>().findObject("Image").GetComponent<Button>().onClick.AddListener(
          delegate
          {
            int finalI = i;
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

      initParameterInfo(0, "Tank", 2000, 70, 100, 100, 35);
      initParameterInfo(1, "Dps", 1200, 100, 200, 30, 40);
      initParameterInfo(2, "Healer", 1200, 150, 125, 50, 35);
      initParameterInfo(5, "BOSS", 10000, 500, 300, 25, 80);

      characters[0].GetComponent<MyCharacter>().status.skills = new SkillAbility[]{
        getSkillInfo(0),
        getSkillInfo(3)
      };

      characters[1].GetComponent<MyCharacter>().status.skills = new SkillAbility[]{
        getSkillInfo(0),
        getSkillInfo(3)
      };

      characters[2].GetComponent<MyCharacter>().status.skills = new SkillAbility[]{
        getSkillInfo(0),
        getSkillInfo(3)
      };

      characters[5].GetComponent<MyCharacter>().status.skills = new SkillAbility[]{
        getSkillInfo(0),
        getSkillInfo(2)
      };
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
      characters[id].GetComponent<MyCharacter>().status.maxAtk = atk;

      characters[id].GetComponent<MyCharacter>().parameter.def = def;
      characters[id].GetComponent<MyCharacter>().status.curDef = def;
      characters[id].GetComponent<MyCharacter>().status.maxDef = def;

      characters[id].GetComponent<MyCharacter>().parameter.spd = spd;
      characters[id].GetComponent<MyCharacter>().status.curSpd = spd;
      characters[id].GetComponent<MyCharacter>().status.maxSpd = spd;

      characters[id].GetComponent<MyCharacter>().status.buff = new ArrayList();
    }

    private GameObject findObject(GameObject obj, string name)
    {
      for (int i = 0; i < obj.transform.childCount; i += 1)
      {
        if(obj.transform.GetChild(i).gameObject.name == name)
        {
          return obj.transform.GetChild(i).gameObject;
        }
      }

      return null;
    }

    private void castSkill()
    {
        characters[curCharacterID].GetComponent<Skill>().castSkill(curSkillID, characters[curCharacterID], characters[curTargetID]);
    }

    private SkillAbility getSkillInfo(int id)
    {
      switch(id)
      {
        case 0: return new SkillAbility(0, "Normal Attack", 0, 0, 2, false);
        case 1: return new SkillAbility(1, "Taunt", 20, 7, 0, false);
        case 2: return new SkillAbility(2, "ATK UP", 15, 10, 0, false);
        case 3: return new SkillAbility(3, "HP Regeneration", 0, 0, 0, true);
      }

      return null;
    }
}
