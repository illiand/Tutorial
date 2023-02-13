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
      Vector3 hpColor = Vector3.Lerp(new Vector3(1, 0, 0), new Vector3(0, 1, 0), status.curHp / status.maxHp);
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
  public float hp;
  public float mp;
  public float atk;
  public float def;
  public float spd;

  public SkillAbility[] skills;
}

public class BattleStatus
{
  public float maxHp;
  public float curHp;

  public float maxMp;
  public float curMp;

  public float curAtk;
  public float curDef;
  public float curSpd;

  public int[] skillsCoolDown;
  public ArrayList buff;

  public float turnPosition;
}

public class Buff
{
  public int id;
  public int remainingTurn;

  public Buff(int id, int turn)
  {
    this.id = id;
    this.remainingTurn = turn;
  }
}

public class SkillAbility
{
  public int id;
  public string name;
  public int mpCost;
  public int cooldown;
  public int type;
  public bool isPassive;

  public SkillAbility(int id, string name, int mpCost, int cooldown, int type, bool isPassive)
  {
    this.id = id;
    this.name = name;
    this.mpCost = mpCost;
    this.cooldown = cooldown;
    this.type = type;
    this.isPassive = isPassive;
  }
}
