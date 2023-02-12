using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public void castSkill(int id, GameObject self, GameObject target)
    {
      switch(id)
      {
        case 0:
          target.GetComponent<MyCharacter>().status.curHp -= getDamage(100, self, target);
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

    private float getDamage(int amount, GameObject self, GameObject target)
    {
      float atk = self.GetComponent<MyCharacter>().status.curAtk;
      float def = target.GetComponent<MyCharacter>().status.curDef;

      return amount / 100f * (atk * atk / (atk + def));
    }
}
