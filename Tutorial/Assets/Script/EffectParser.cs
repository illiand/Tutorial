using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EffectParser : MonoBehaviour
{
    // public GameObject layout,skillButtons;
    // public GameObject skillS;

    // private void Awake()
    // {
    //     layout = GameObject.Find("SkillSelectionLayout").transform.GetChild(0).gameObject;
    //     skillS= GameObject.Find("SkillSelectionLayout");
    //
    //
    // }
    // private void Start()
    // {
    //
    //
    //     skillButtons = GameObject.Find("Command Selection Layout").transform.GetChild(1).gameObject;
    //     //Debug.Log("The name is " + skillButtons.name);
    //     skillButtons.GetComponent<Button>().onClick.AddListener(
    //         delegate
    //         {
    //             skillS.SetActive(true);
    //         }
    //
    //     ) ;
    // }


    public GameObject EffectobjectToSpawn;
    public GameObject BossEffectobjectToSpawn;
    public GameObject damageText;
    public GameObject[] atkEffect;
    public Transform mCanvas;

    private bool startShowingText =false;
    private float textTimeRemaining = 3f;

    public void castSkill(int id, GameObject self, GameObject target)
    {
      GameObject skillText = GameObject.Find("BattleLogText");

      skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().status.index >= 5 ? "<color=#5A0007>" : "<color=#19251A>";

      Vector3 tempPos = target.transform.position;
      tempPos.y += 100;

      Vector3 tempPosSword = target.transform.position;
      tempPosSword.y += 175;
      tempPosSword.x -= 40;
      
      GameObject temp;

        switch (id)
      {
        case 0://only for characters
          float damage = getDamage(-100, self, target);
          castEncounterBuff(self, target);

          target.GetComponent<MyCharacter>().status.curHp += damage;
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " give " + target.GetComponent<MyCharacter>().parameter.name + " " + damage + " damage\n";
          //Debug.Log(self.GetComponent<MyCharacter>().parameter.name + " is atking!");

            //play attacking effect
                 
            temp = (GameObject)Instantiate(atkEffect[0], tempPos, target.transform.rotation);
            temp.transform.SetParent(mCanvas);

          break;

        //skill 1
        case 1:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(1, 2));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used Taunt\n";

          break;

        case 2:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(2, 5));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used ATK UP\n";
          temp = (GameObject)Instantiate(atkEffect[4], tempPos, target.transform.rotation);
          temp.transform.SetParent(mCanvas);
          break;

        case 3:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(3, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used passive skill HP Regeneration\n";
          break;
        case 4:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(4, 999, target.GetComponent<MyCharacter>().status.index, getDebuffDamage(-100, self, target)));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used Doppelgänger on " + target.GetComponent<MyCharacter>().parameter.name + "\n";

          break;

        case 5:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(5, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used passive skill ATK+\n";
    
          //temp = (GameObject)Instantiate(atkEffect[4], tempPosSword, target.transform.rotation);
          //temp.transform.SetParent(mCanvas);

          break;

        case 6:
          target.GetComponent<MyCharacter>().status.curHp += getDamage(target.GetComponent<MyCharacter>().status.maxHp * 0.25f, self, target);
          normalizeHPMP(target);

          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(6, 5));

          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " recoveried 25% HP and got regenerate buff\n";
          temp = (GameObject)Instantiate(atkEffect[5], tempPos, target.transform.rotation);
          temp.transform.SetParent(mCanvas);
          break;

        case 7:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(7, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used passive skill MP Regeneration\n";
          break;

        case 8:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(8, 3));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used Charge!\n";

          break;
        case 9:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(9, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used passive skill Bersaka!\n";

          break;
        case 10:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(10, 3));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " increased 50% ATK by receiving damage\n";
          
          temp = (GameObject)Instantiate(atkEffect[4], tempPosSword, target.transform.rotation);
          temp.transform.SetParent(mCanvas);
          Debug.Log("case 10");

          break;
        case 11:
          damage = getDamage(-400, self, target);
          castEncounterBuff(self, target);

          target.GetComponent<MyCharacter>().status.curHp += damage;
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " give " + target.GetComponent<MyCharacter>().parameter.name + " " + damage + " damage\n";

          break;
        case 12:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(12, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used Bersaka!\n";

          break;
        case 13:
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " is charging...\n";

          break;
        case 14:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(14, 1));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used Defence\n";

          break;

        //nwq
        case 15:
          GetComponent<Trigger_ElementBlend>().init();
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used Elementa Mixtio\n";
          break;

        case 16:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(16, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used passive skill Element Immune\n";
          break;
        case 17:
          GetComponent<Trigger_ElementBlend>().change(GetComponent<Controller>().summonRandomly(target.GetComponent<MyCharacter>().status.index, "Monster", "char_18", 1000, 200, 50, 25, 80, new int[]{23, 28, 16}));
          GetComponent<Trigger_ElementBlend>().change(GetComponent<Controller>().summonRandomly(target.GetComponent<MyCharacter>().status.index, "Monster", "char_18", 1000, 200, 50, 25, 80, new int[]{23, 28, 16}));

          break;
        case 18:
          GetComponent<Trigger_ElementBlend>().change(target.GetComponent<MyCharacter>().status.index);

          break;
        case 19:
          GetComponent<Trigger_ElementBlend>().changeAll();
          target.GetComponent<MyCharacter>().status.curPos -= 100;
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used Element Change\n";
          break;

        case 20:
          target.GetComponent<MyCharacter>().status.curMp += target.GetComponent<MyCharacter>().status.maxMp * 0.9f;
          normalizeHPMP(target);
          target.GetComponent<MyCharacter>().status.curPos -= 50;
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used MP Recover+\n";
          
          temp = (GameObject)Instantiate(atkEffect[6], tempPos, target.transform.rotation);
          temp.transform.SetParent(mCanvas);
          
          break;
        case 21:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(21, 3));
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " used Imprison on " + target.GetComponent<MyCharacter>().parameter.name + "\n";
          break;
        case 22:
          damage = getDamage(-250, self, target);
          castEncounterBuff(self, target);

          target.GetComponent<MyCharacter>().status.curHp += damage;
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " give " + target.GetComponent<MyCharacter>().parameter.name + " " + damage + " damage\n";
            
            temp = (GameObject)Instantiate(atkEffect[1], tempPos, target.transform.rotation);
            temp.transform.SetParent(mCanvas);
                //Debug.Log("case 22");
          break;
        case 23:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(23, 2));
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used Charge Ver2.\n";
          break;
        case 24:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(23, 3));
          damage = getDamage(-150, self, target);
          castEncounterBuff(self, target);

          target.GetComponent<MyCharacter>().status.curHp += damage;
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " give " + target.GetComponent<MyCharacter>().parameter.name + " " + damage + " damage\n";

            temp = (GameObject)Instantiate(atkEffect[2], tempPos, target.transform.rotation);
            temp.transform.SetParent(mCanvas);
            Debug.Log("case 24");
          break;

        case 25:
          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " can't move...\n";
          break;
        case 28:
          target.GetComponent<MyCharacter>().status.curMp += target.GetComponent<MyCharacter>().status.maxMp * 0.9f;
          normalizeHPMP(target);

          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " used MP Recover\n";

            temp = (GameObject)Instantiate(atkEffect[6], tempPos, target.transform.rotation);
            temp.transform.SetParent(mCanvas);
                
          break;
        case 29:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(26, 999));
          break;
        case 30:
          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(27, 999));
          break;
      }

      //check dead people
      if(self.GetComponent<MyCharacter>().status.curHp <= 0)
      {
        skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " dead...\n";
        GetComponent<Controller>().isActive[self.GetComponent<MyCharacter>().status.index] = false;
        self.SetActive(false);
      }

      if(target.GetComponent<MyCharacter>().status.curHp <= 0)
      {
        skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " dead...\n";
        GetComponent<Controller>().isActive[target.GetComponent<MyCharacter>().status.index] = false;
        target.SetActive(false);
      }

      skillText.GetComponent<TextMeshProUGUI>().text += "</color>";

      GameObject.Find("ViewportContent").GetComponent<RectTransform>().localPosition = new Vector2(0, 9999);
    }

    private float getDamage(float amount, GameObject self, GameObject target)
    {
      float damage = 0f;

      //damage
      if(amount < 0)
      {
        float atk = self.GetComponent<MyCharacter>().status.curAtk;
        float def = target.GetComponent<MyCharacter>().status.curDef;
        damage = amount / 100f * (atk * atk / (atk + def)) * castDamageResistBuff(target) * castSpecialDamageResistBuff(self, target);

        damageText.transform.SetParent(mCanvas);
        damageText.GetComponent<RectTransform>().position = target.GetComponent<RectTransform>().position;

        showDamageValue(damage, target);
      }
      //heal
      else
      {
        damage = amount * castSpecialDamageResistBuff(self, target);

        damageText.transform.SetParent(mCanvas);
        damageText.GetComponent<RectTransform>().position = target.GetComponent<RectTransform>().position;

        showDamageValue(damage, target);
      }

      return damage;
    }

    private float getDebuffDamage(float amount, GameObject self, GameObject target)
    {
      float damage = 0f;
      //damage
      if(amount < 0)
      {
        float atk = self.GetComponent<MyCharacter>().status.curAtk;
        float def = target.GetComponent<MyCharacter>().status.curDef;
        damage = amount / 100f * (atk * atk / (atk + def)) * castDamageResistBuff(target);
      }
      //heal
      else
      {
        damage = amount;
      }

      return damage;
    }

    public void showDamageValue(float damage, GameObject target)
    {
        startShowingText = true;

        //这里偷懒了 ，应该根据技能来播放特效
        //KE HAI XING
        //if (target.GetComponent<MyCharacter>().status.index >= 5)//if target is boss
        //{
        //    Vector3 tempPos = target.transform.position;
        //    tempPos.y += 100;
        //    GameObject temp = (GameObject)Instantiate(atkEffect[0], tempPos, target.transform.rotation);
        //    temp.transform.SetParent(mCanvas);
        //}
        //else
        //{
        //    Vector3 tempPosn = target.transform.position;
        //    tempPosn.y += 100;
        //    GameObject temp = (GameObject)Instantiate(atkEffect[1], tempPosn, target.transform.rotation);
        //    temp.transform.SetParent(mCanvas);
        //}

        if(damage < 0)
        {
          damageText.GetComponentInChildren<TextMeshProUGUI>().text = "-" + (int)-damage;
          damageText.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1f, 0f, 0f);
        }
        else
        {
          damageText.GetComponentInChildren<TextMeshProUGUI>().text = "+" + (int)damage;
          damageText.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0f, 1f, 0f);
        }
    }

    private void Update()
    {
        if (startShowingText)
        {
            if (textTimeRemaining > 0)
            {
                textTimeRemaining -= Time.deltaTime;
            }
            else
            {
                startShowingText = false;
                textTimeRemaining = 3f;
            }
        }
        else
        {
            damageText.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    public void normalizeHPMP(GameObject target)
    {
      target.GetComponent<MyCharacter>().status.curHp = target.GetComponent<MyCharacter>().status.curHp > target.GetComponent<MyCharacter>().status.maxHp ? target.GetComponent<MyCharacter>().status.maxHp : target.GetComponent<MyCharacter>().status.curHp;
      target.GetComponent<MyCharacter>().status.curHp = target.GetComponent<MyCharacter>().status.curHp < 0 ? 0 : target.GetComponent<MyCharacter>().status.curHp;// if <0 then =0; else status.curHp

      target.GetComponent<MyCharacter>().status.curMp = target.GetComponent<MyCharacter>().status.curMp > target.GetComponent<MyCharacter>().status.maxMp ? target.GetComponent<MyCharacter>().status.maxMp : target.GetComponent<MyCharacter>().status.curMp;
      target.GetComponent<MyCharacter>().status.curMp = target.GetComponent<MyCharacter>().status.curMp < 0 ? 0 : target.GetComponent<MyCharacter>().status.curMp;
    }

    /**
     *
     * if the unit is in debuff may skip turn
     *
     **/
    public bool isInControl(GameObject unit)
    {
      for(int i = 0; i < unit.GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        int id = ((Buff)unit.GetComponent<MyCharacter>().status.buff[i]).id;
        int remainingTurn = ((Buff)unit.GetComponent<MyCharacter>().status.buff[i]).remainingTurn;

        if((id == 8 || id == 23) && remainingTurn >= 1)
        {
          if(remainingTurn >= 2)
          {
            castSkill(13, unit, unit);
          }

          return true;
        }

        if((id == 21) && remainingTurn >= 1)
        {
          if(remainingTurn >= 2)
          {
            castSkill(25, unit, unit);
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
    public void parseStartTurnBuff(GameObject unit)
    {
      GameObject skillText = GameObject.Find("BattleLogText");

      for(int i = 0; i < unit.GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        skillText.GetComponent<TextMeshProUGUI>().text += unit.GetComponent<MyCharacter>().status.index >= 5 ? "<color=#5A0007>" : "<color=#19251A>";

        switch(((Buff)unit.GetComponent<MyCharacter>().status.buff[i]).id)
        {
          case 3:
            unit.GetComponent<MyCharacter>().status.curHp += unit.GetComponent<MyCharacter>().status.maxHp * 0.05f;
            normalizeHPMP(unit);

            break;

          case 4:
            unit.GetComponent<MyCharacter>().status.curHp += ((Buff)unit.GetComponent<MyCharacter>().status.buff[i]).value;
            castEncounterBuff(GetComponent<Controller>().characters[((Buff)unit.GetComponent<MyCharacter>().status.buff[i]).from], unit);
            normalizeHPMP(unit);

            skillText.GetComponent<TextMeshProUGUI>().text += unit.GetComponent<MyCharacter>().parameter.name + " decrease " + ((Buff)unit.GetComponent<MyCharacter>().status.buff[i]).value + " HP by Doppelgänger\n";

            break;
          case 5:
            unit.GetComponent<MyCharacter>().status.curAtk += unit.GetComponent<MyCharacter>().parameter.atk * 0.03f;
            break;

          case 6:
            unit.GetComponent<MyCharacter>().status.curHp += unit.GetComponent<MyCharacter>().status.maxHp * 0.05f;
            normalizeHPMP(unit);

            break;

          case 7:
            unit.GetComponent<MyCharacter>().status.curMp += unit.GetComponent<MyCharacter>().status.maxMp * 0.05f;
            normalizeHPMP(unit);
            break;
        }

        skillText.GetComponent<TextMeshProUGUI>().text += "</color>";

        ((Buff)unit.GetComponent<MyCharacter>().status.buff[i]).remainingTurn -= 1;

        if(((Buff)unit.GetComponent<MyCharacter>().status.buff[i]).remainingTurn == 0)
        {
          //remove parameter increase/decrease effect
          switch(((Buff)unit.GetComponent<MyCharacter>().status.buff[i]).id)
          {
            case 8:
              castSkill(11, unit, GetComponent<Controller>().characters[GetComponent<Controller>().getTarget(unit.GetComponent<MyCharacter>().status.index, 11)[0]]);

              break;
            case 10:
              unit.GetComponent<MyCharacter>().status.curAtk -= unit.GetComponent<MyCharacter>().parameter.atk * 2.5f;
              break;
            case 23:
              castSkill(24, unit, GetComponent<Controller>().characters[GetComponent<Controller>().getTarget(unit.GetComponent<MyCharacter>().status.index, 24)[0]]);
              break;
          }

          unit.GetComponent<MyCharacter>().status.buff.RemoveAt(i);
          i -= 1;
        }

      }
    }

    /**
     *
     * after the turn end Buffs trigger
     *
     **/
    public void parseEndTurnBuff(GameObject target)
    {

    }

    /**
     *
     * Encounter buff, triggers when the target is got hit
     *
     **/
    public void castEncounterBuff(GameObject self, GameObject target)
    {
      for (int i = 0; i < target.GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        //ATK UP skill
        if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id == 2)
        {
          target.GetComponent<MyCharacter>().status.curAtk += target.GetComponent<MyCharacter>().parameter.atk * 2.5f;
          castSkill(10, target, target);
        }

        //bersaka
        if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id == 9)
        {
          if(target.GetComponent<MyCharacter>().status.curHp / target.GetComponent<MyCharacter>().status.maxHp < 0.3f)
          {
            castSkill(12, target, target);

            target.GetComponent<MyCharacter>().status.curHp += target.GetComponent<MyCharacter>().parameter.hp * 0.25f;
            target.GetComponent<MyCharacter>().status.maxHp += target.GetComponent<MyCharacter>().parameter.hp * 0.25f;
            target.GetComponent<MyCharacter>().status.curMp += target.GetComponent<MyCharacter>().parameter.mp * 0.25f;
            target.GetComponent<MyCharacter>().status.maxMp += target.GetComponent<MyCharacter>().parameter.mp * 0.25f;
            target.GetComponent<MyCharacter>().status.curAtk += target.GetComponent<MyCharacter>().parameter.atk * 0.25f;
            target.GetComponent<MyCharacter>().status.curDef += target.GetComponent<MyCharacter>().parameter.def * 0.25f;
            target.GetComponent<MyCharacter>().status.curSpd += target.GetComponent<MyCharacter>().parameter.spd * 0.25f;

            target.GetComponent<MyCharacter>().status.buff.RemoveAt(i);
            i -= 1;

            continue;
          }
        }

        //summon phase
        if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id == 26)
        {
          if(target.GetComponent<MyCharacter>().status.curHp / target.GetComponent<MyCharacter>().status.maxHp < 0.8f)
          {
            Debug.Log(self.GetComponent<MyCharacter>().status.index + " " + target.GetComponent<MyCharacter>().status.index);
            GetComponent<Controller>().changeSkill(target.GetComponent<MyCharacter>().status.index, 0, 17);
            GetComponent<Controller>().changeSkill(target.GetComponent<MyCharacter>().status.index, 5, 30);

            target.GetComponent<MyCharacter>().status.buff.RemoveAt(i);
            i -= 1;

            continue;
          }
        }

        //element change phase
        if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id == 27)
        {
          if(target.GetComponent<MyCharacter>().status.curHp / target.GetComponent<MyCharacter>().status.maxHp < 0.5f)
          {
            GetComponent<Controller>().changeSkill(target.GetComponent<MyCharacter>().status.index, 0, 19);
            GetComponent<Controller>().changeSkill(target.GetComponent<MyCharacter>().status.index, 5, -1);

            target.GetComponent<MyCharacter>().status.buff.RemoveAt(i);
            i -= 1;

            continue;
          }
        }
      }
    }

    /**
     *
     *  Damage resist buff, calculate when got hit
     *
     **/
    public float castDamageResistBuff(GameObject target)
    {
      float value = 1f;

      for (int i = 0; i < target.GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        //taunt buff
        if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id == 1)
        {
          value *= 0.6f;
        }
        //defence
        else if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id == 14)
        {
          value *= 0.7f;
        }
        //Charge Ver2.
        else if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id == 23)
        {
          value *= 5f;
        }
      }

      return value;
    }

    public float castSpecialDamageResistBuff(GameObject self, GameObject target)
    {
      float value = 1f;

      //Element blend, only two all have the buff,
      //the special effect can occur
      if(GetComponent<Trigger_ElementBlend>() != null)
      {
        if(GetComponent<Trigger_ElementBlend>().elementRecorder[self.GetComponent<MyCharacter>().status.index] != 0 && GetComponent<Trigger_ElementBlend>().elementRecorder[target.GetComponent<MyCharacter>().status.index] != 0)
        {
          //no effect if self immune
          if(!GetComponent<Controller>().containsBuff(self.GetComponent<MyCharacter>().status.index, 16))
          {
            value *= GetComponent<Trigger_ElementBlend>().getDamageDegree(self, target);
          }
        }
      }

      for (int i = 0; i < self.GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {

      }

      return value;
    }
}
