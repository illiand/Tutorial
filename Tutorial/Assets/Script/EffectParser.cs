using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EffectParser : MonoBehaviour
{
    public GameObject layout,skillButtons;
    public GameObject skillS;

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
    public void castSkill(int id, GameObject self, GameObject target)
    {
      GameObject skillText = GameObject.Find("BattleLogText");

      skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().status.index >= 5 ? "<color=#99001c>" : "<color=#009908>";

      switch(id)
      {
        case 0:
          float damage = getDamage(100, self, target);
          castEncounterBuff(target);

          target.GetComponent<MyCharacter>().status.curHp -= damage;
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " give " + target.GetComponent<MyCharacter>().parameter.name + " " + damage + " damage\n";

          break;

        //skill 1
        case 1:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(1, 3));
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " used Taunt\n";

          break;

        case 2:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(2, 5));
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " used ATK UP\n";

          break;

        case 3:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(3, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " used passive skill HP Regeneration\n";
          break;
        case 4:

          break;

        case 5:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(5, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " used passive skill ATK+\n";

          break;

        case 6:
          target.GetComponent<MyCharacter>().status.curHp += target.GetComponent<MyCharacter>().status.maxHp * 0.25f;
          normalizeHPMP(target);

          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(6, 5));

          skillText.GetComponent<TextMeshProUGUI>().text += target.GetComponent<MyCharacter>().parameter.name + " recoveried 25% HP and got regenerate buff\n";

          break;

        case 7:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(7, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " used passive skill MP Regeneration\n";
          break;

        case 8:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(8, 3));
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " used Charge!\n";

          break;
        case 9:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(9, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " used passive skill Bersaka!\n";

          break;
        case 10:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(10, 3));
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " increased 10% ATK from ATK up\n";

          break;
        case 11:
          damage = getDamage(400, self, target);
          castEncounterBuff(target);

          target.GetComponent<MyCharacter>().status.curHp -= damage;
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " give " + target.GetComponent<MyCharacter>().parameter.name + " " + damage + " damage\n";

          break;
        case 12:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(12, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " used Bersaka!\n";

          break;
        case 13:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(13, 999));
          skillText.GetComponent<TextMeshProUGUI>().text += self.GetComponent<MyCharacter>().parameter.name + " is charging...\n";

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


    // public void skillList(int id)
    // {
    //     switch (id)
    //     {
    //         case 0:
    //             const string V = "increase attack";
    //
    //             skillText = layout.transform.GetComponentInChildren<TextMeshProUGUI>();
    //             skillText.text = V;
    //             //layout.AddText(self.GetComponent<MyCharacter>().parameter.name + " give  " + target.GetComponent().parameter.name + " " + getDamage(100, self, target) + " damage");
    //             break;
    //
    //         //skill 1
    //         case 1:
    //
    //
    //             break;
    //
    //         case 2:
    //
    //
    //             break;
    //
    //         case 3:
    //
    //
    //             break;
    //         case 4:
    //
    //             break;
    //         case 5:
    //
    //
    //             skillText = layout.transform.GetComponentInChildren<TextMeshProUGUI>();
    //             skillText.text = "boss skill";
    //             break;
    //     }
    // }

    private float getDamage(int amount, GameObject self, GameObject target)
    {
      float atk = self.GetComponent<MyCharacter>().status.curAtk;
      float def = target.GetComponent<MyCharacter>().status.curDef;

      return amount / 100f * (atk * atk / (atk + def)) * castDamageResistBuff(target);
    }

    public void normalizeHPMP(GameObject target)
    {
      target.GetComponent<MyCharacter>().status.curHp = target.GetComponent<MyCharacter>().status.curHp > target.GetComponent<MyCharacter>().status.maxHp ? target.GetComponent<MyCharacter>().status.maxHp : target.GetComponent<MyCharacter>().status.curHp;
      target.GetComponent<MyCharacter>().status.curHp = target.GetComponent<MyCharacter>().status.curHp < 0 ? 0 : target.GetComponent<MyCharacter>().status.curHp;

      target.GetComponent<MyCharacter>().status.curMp = target.GetComponent<MyCharacter>().status.curMp > target.GetComponent<MyCharacter>().status.maxMp ? target.GetComponent<MyCharacter>().status.maxMp : target.GetComponent<MyCharacter>().status.curMp;
      target.GetComponent<MyCharacter>().status.curMp = target.GetComponent<MyCharacter>().status.curMp < 0 ? 0 : target.GetComponent<MyCharacter>().status.curMp;
    }

    /**
     *
     * if the unit is in debuff may skip turn
     *
     **/
    public bool isInControl(GameObject target)
    {
      for(int i = 0; i < target.GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id == 8)
        {
          if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).remainingTurn != 1)
          {
            castSkill(13, target, target);
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
    public void parseStartTurnBuff(GameObject target)
    {
      for(int i = 0; i < target.GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        switch(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id)
        {
          case 3:
            target.GetComponent<MyCharacter>().status.curHp += target.GetComponent<MyCharacter>().status.maxHp * 0.05f;
            normalizeHPMP(target);

            break;
          case 5:
            target.GetComponent<MyCharacter>().status.curAtk += target.GetComponent<MyCharacter>().parameter.atk * 0.03f;
            break;

          case 6:
            target.GetComponent<MyCharacter>().status.curHp += target.GetComponent<MyCharacter>().status.maxHp * 0.05f;
            normalizeHPMP(target);
            break;

          case 7:
            target.GetComponent<MyCharacter>().status.curMp += target.GetComponent<MyCharacter>().status.maxMp * 0.05f;
            normalizeHPMP(target);
            break;
        }

        ((Buff)target.GetComponent<MyCharacter>().status.buff[i]).remainingTurn -= 1;

        if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).remainingTurn == 0)
        {
          //remove parameter increase/decrease effect
          switch(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id)
          {
            case 8:
              castSkill(11, target, GetComponent<Controller>().characters[GetComponent<Controller>().getTarget(2)]);

              break;
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
    public void parseEndTurnBuff(GameObject target)
    {

    }

    /**
     *
     * Encounter buff, triggers when the target is got hit
     *
     **/
    public void castEncounterBuff(GameObject target)
    {
      for (int i = 0; i < target.GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        //ATK UP skill
        if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id == 2)
        {
          target.GetComponent<MyCharacter>().status.curAtk += target.GetComponent<MyCharacter>().parameter.atk * 0.1f;
          castSkill(10, target, target);
        }

        //bersaka
        if(((Buff)target.GetComponent<MyCharacter>().status.buff[i]).id == 9)
        {
          if(target.GetComponent<MyCharacter>().status.curHp / target.GetComponent<MyCharacter>().status.maxHp < 0.25f)
          {
            target.GetComponent<MyCharacter>().status.curHp *= 1.25f;
            target.GetComponent<MyCharacter>().status.maxHp *= 1.25f;
            target.GetComponent<MyCharacter>().status.curMp *= 1.25f;
            target.GetComponent<MyCharacter>().status.maxMp *= 1.25f;
            target.GetComponent<MyCharacter>().status.curAtk *= 1.25f;
            target.GetComponent<MyCharacter>().status.curDef *= 1.25f;
            target.GetComponent<MyCharacter>().status.curSpd *= 1.25f;

            target.GetComponent<MyCharacter>().status.buff.RemoveAt(i);
            i -= 1;
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
          value *= 0.4f;
        }
      }

      return value;
    }
}
