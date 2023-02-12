using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacter : MonoBehaviour
{
  public Parameter parameter;
  public BattleStatus status;

  private GameObject curTarget;
  private int curSkillID;

  void Start()
  {
    parameter = new Parameter();
    status = new BattleStatus();
  }

  void castSkill()
  {
      GetComponent<Skill>().castSkill(curSkillID, gameObject, curTarget);
  }
}

public class Parameter
{
  public float hp;
  public float mp;
  public float atk;
  public float def;
  public float spd;

  public int[] skills;
}

public class BattleStatus
{
  public float maxHp;
  public float curHp;

  public float maxMp;
  public float curMp;

  public float maxAtk;
  public float curAtk;

  public float maxDef;
  public float curDef;

  public float maxSpd;
  public float curSpd;

  public ArrayList buff;
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
