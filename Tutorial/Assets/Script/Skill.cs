using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public GameObject layout,skillButtons;
    public TMPro.TextMeshProUGUI skillText;
    public GameObject skillS;
    private void Awake()
    {
        layout = GameObject.Find("SkillSelectionLayout").transform.GetChild(0).gameObject;
        skillS= GameObject.Find("SkillSelectionLayout");


    }
    private void Start()
    {


        skillButtons = GameObject.Find("Command Selection Layout").transform.GetChild(1).gameObject;
        Debug.Log("The name is " + skillButtons.name);
        skillButtons.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                skillS.SetActive(true);
            }

        ) ;
    }
    public void castSkill(int id, GameObject self, GameObject target)
    {
      GameObject layout = GameObject.Find("BattleLogText");

      switch(id)
      {
        case 0:
          float damage = getDamage(100, self, target);
          castEncounterBuff(target);

          target.GetComponent<MyCharacter>().status.curHp -= damage;
          layout.GetComponent<TextMeshPro>().text += self.GetComponent<MyCharacter>().parameter.name + " give " + target.GetComponent<MyCharacter>().parameter.name + " " + damage + " damage\n";

          break;

        //skill 1
        case 1:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(1, 3));
          layout.GetComponent<TextMeshPro>().text = self.GetComponent<MyCharacter>().parameter.name + " used Taunt\n";

          break;

        case 2:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(2, 5));
          layout.GetComponent<TextMeshPro>().text = self.GetComponent<MyCharacter>().parameter.name + " used ATK UP\n";

          break;

        case 3:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(3, 999));
          layout.GetComponent<TextMeshPro>().text = self.GetComponent<MyCharacter>().parameter.name + " used passive skill HP Regeneration\n";
          break;
        case 4:

          break;

        case 5:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(5, 999));
          layout.GetComponent<TextMeshPro>().text = self.GetComponent<MyCharacter>().parameter.name + " used passive skill ATK+\n";

          break;

        case 6:
          target.GetComponent<MyCharacter>().status.curHp += target.GetComponent<MyCharacter>().status.maxHp * 0.25f;
          normalizeHPMP(target);

          target.GetComponent<MyCharacter>().status.buff.Add(new Buff(6, 5));

          layout.GetComponent<TextMeshPro>().text = target.GetComponent<MyCharacter>().parameter.name + " recoveried 25% HP and got regenerate buff\n";

          break;

        case 7:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(7, 999));
          layout.GetComponent<TextMeshPro>().text = self.GetComponent<MyCharacter>().parameter.name + " used passive skill MP Regeneration\n";
          break;

        case 8:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(8, 2));
          layout.GetComponent<TextMeshPro>().text = self.GetComponent<MyCharacter>().parameter.name + " used Charge!\n";

          break;
        case 9:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(9, 999));
          layout.GetComponent<TextMeshPro>().text = self.GetComponent<MyCharacter>().parameter.name + " used passive skill Bersaka!\n";

          break;
        case 10:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(10, 3));
          layout.GetComponent<TextMeshPro>().text = self.GetComponent<MyCharacter>().parameter.name + " used passive skill Bersaka!\n";

          break;
        case 11:
          damage = getDamage(400, self, target);
          castEncounterBuff(target);

          target.GetComponent<MyCharacter>().status.curHp -= damage;
          layout.GetComponent<TextMeshPro>().text += self.GetComponent<MyCharacter>().parameter.name + " give " + target.GetComponent<MyCharacter>().parameter.name + " " + damage + " damage\n";

          break;
        case 12:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(12, 999));
          layout.GetComponent<TextMeshPro>().text = self.GetComponent<MyCharacter>().parameter.name + " used Bersaka!\n";

          break;
      }
    }


    public void skillList(int id)
    {
        switch (id)
        {
            case 0:
                const string V = "increase attack";

                skillText = layout.transform.GetComponentInChildren<TextMeshProUGUI>();
                skillText.text = V;
                //layout.AddText(self.GetComponent<MyCharacter>().parameter.name + " give  " + target.GetComponent().parameter.name + " " + getDamage(100, self, target) + " damage");
                break;

            //skill 1
            case 1:


                break;

            case 2:


                break;

            case 3:


                break;
            case 4:

                break;
            case 5:


                skillText = layout.transform.GetComponentInChildren<TextMeshProUGUI>();
                skillText.text = "boss skill";
                break;
        }
    }

    private float getDamage(int amount, GameObject self, GameObject target)
    {
      float atk = self.GetComponent<MyCharacter>().status.curAtk;
      float def = target.GetComponent<MyCharacter>().status.curDef;

      return amount / 100f * (atk * atk / (atk + def)) * castDamageResistBuff(target);
    }

    public void normalizeHPMP()
    {
      normalizeHPMP(gameObject);
    }

    private void normalizeHPMP(GameObject target)
    {
      target.GetComponent<MyCharacter>().status.curHp = target.GetComponent<MyCharacter>().status.curHp > target.GetComponent<MyCharacter>().status.maxHp ? target.GetComponent<MyCharacter>().status.maxHp : target.GetComponent<MyCharacter>().status.curHp;
      target.GetComponent<MyCharacter>().status.curHp = target.GetComponent<MyCharacter>().status.curHp < 0 ? 0 : target.GetComponent<MyCharacter>().status.curHp;

      target.GetComponent<MyCharacter>().status.curMp = target.GetComponent<MyCharacter>().status.curMp > target.GetComponent<MyCharacter>().status.maxMp ? target.GetComponent<MyCharacter>().status.maxMp : target.GetComponent<MyCharacter>().status.curMp;
      target.GetComponent<MyCharacter>().status.curMp = target.GetComponent<MyCharacter>().status.curMp < 0 ? 0 : target.GetComponent<MyCharacter>().status.curMp;
    }

    /**
     *
     * Encounter buff, triggers when the target is got hit
     *
     **/
    private void castEncounterBuff(GameObject target)
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
    private float castDamageResistBuff(GameObject target)
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
