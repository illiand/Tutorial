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
      switch(id)
      {
        case 0:
          target.GetComponent<MyCharacter>().status.curHp -= getDamage(100, self, target);
          //layout.AddText(self.GetComponent<MyCharacter>().parameter.name + " give  " + target.GetComponent().parameter.name + " " + getDamage(100, self, target) + " damage");
          break;

        //skill 1
        case 1:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(1, 3));

          break;

        case 2:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(2, 5));

          break;

        case 3:
          self.GetComponent<MyCharacter>().status.buff.Add(new Buff(3, 999));

          break;
        case 4:

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

      return amount / 100f * (atk * atk / (atk + def));
    }
}
