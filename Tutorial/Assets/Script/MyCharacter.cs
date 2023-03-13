using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCharacter : MonoBehaviour
{
  public Parameter parameter;
  public BattleStatus status;

  void Update()
  {
    if(status != null)
    {
      Vector3 hpColor = new Vector3();
      if(status.curHp / status.maxHp > 0.5f)
      {
        hpColor = Vector3.Lerp(new Vector3(1, 1, 0), new Vector3(0, 1, 0), status.curHp / status.maxHp * 2f - 1f);
      }
      else
      {
        hpColor = Vector3.Lerp(new Vector3(1, 0, 0), new Vector3(1, 1, 0), status.curHp / status.maxHp * 2f);
      }

      findObject("HP").GetComponent<Image>().material.SetColor("_color", new Color(hpColor.x, hpColor.y, hpColor.z, 1f));
      findObject("HP").GetComponent<Image>().material.SetFloat("_percentage", status.curHp / status.maxHp);
      findObject("MP").GetComponent<Image>().material.SetFloat("_percentage", status.curMp / status.maxMp);

    }
  }

  public GameObject findObject(string name)
  {
    for (int i = 0; i < transform.childCount; i += 1)
    {
      if(transform.GetChild(i).gameObject.name == name)
      {
        return transform.GetChild(i).gameObject;
      }
    }

    return null;
  }
}

public class Parameter
{
  public string name;
  public Sprite pic;
  public float hp;
  public float mp;
  public float atk;
  public float def;
  public float spd;

  public SkillAbility[] skills;
}

public class BattleStatus
{
  //the position in the battlefield
  public int index;
  public float maxHp;
  public float curHp;

  public float maxMp;
  public float curMp;

  public float curAtk;
  public float curDef;
  public float curSpd;

  public int[] skillsCoolDown;
  public ArrayList buff;

  public float curPos;
}

public class Buff
{
  public int id;
  public int remainingTurn;
  public int from;
  public float value;
  public int[] opt;

  public Buff(int id, int turn, int from = -1, float value = 0)
  {
    this.id = id;
    this.remainingTurn = turn;
    this.from = from;
    this.value = value;
  }
}

public class SkillAbility
{
  public int id;
  public string name;
  public string des;
  public int mpCost;
  public int cooldown;
  public int type;
  public int targetCount;
  public bool isPassive;

  public SkillAbility(int id, string name, string des, int mpCost, int cooldown, int type, int targetCount, bool isPassive)
  {
    this.id = id;
    this.name = name;
    this.des = des;
    this.mpCost = mpCost;
    this.cooldown = cooldown;
    //0: self 1: ally 2: enemy
    this.type = type;
    //-1: all allies or enemies depends on type
    //0~X: count
    this.targetCount = targetCount;
    this.isPassive = isPassive;
  }
}
