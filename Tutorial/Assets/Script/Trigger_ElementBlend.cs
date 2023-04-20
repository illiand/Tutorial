using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trigger_ElementBlend : MonoBehaviour
{
    private int element = 1;
    public int[] elementRecorder = new int[10];

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      for(int i = 0; i < 10; i += 1)
      {
        if(GetComponent<Controller>().isActive[i])
        {
                if(elementRecorder[i] ==1)
                {
                    GetComponent<Controller>().characters[i].GetComponent<MyCharacter>().findObject("Ele Image").GetComponent<Image>().color = new Color32(20, 63, 211, 244);//blue
                }
                if (elementRecorder[i] == 2)
                {
                    GetComponent<Controller>().characters[i].GetComponent<MyCharacter>().findObject("Ele Image").GetComponent<Image>().color = new Color32(211, 23, 20, 244);//red
                }
                if (elementRecorder[i] == 3)
                {
                    GetComponent<Controller>().characters[i].GetComponent<MyCharacter>().findObject("Ele Image").GetComponent<Image>().color = new Color32(221, 140, 20, 244);//yellow

                }


                //TODO set element color
            }
      }
    }

    //init with fixed value
    public void init()
    {
      for(int i = 0; i < 10; i += 1)
      {
        if(GetComponent<Controller>().isActive[i])
        {
          elementRecorder[i] = element;

          element = element == 3 ? 1 : element + 1;
        }
      }
    }

    //change by fixed increaseing 1
    public void change(int index)
    {
      if(index < 0 || 9 < index)
      {
        return;
      }

      elementRecorder[index] = element;
      element = element == 3 ? 1 : element + 1;
    }

    //random
    public void changeAll()
    {
      for(int i = 0; i < 10; i += 1)
      {
        if(GetComponent<Controller>().isActive[i])
        {
          elementRecorder[i] = Random.Range(1, 3);
        }
      }
    }

    public float getDamageDegree(GameObject self, GameObject target)
    {
      //if same element
      float value = 0.5f;

      int selfElement = elementRecorder[self.GetComponent<MyCharacter>().status.index];
      int targetElement = elementRecorder[target.GetComponent<MyCharacter>().status.index];

      //good
      if(selfElement + 1 == targetElement || selfElement == 3 && targetElement == 1)
      {
        value = 2;
      }
      //bad
      else if(selfElement - 1 == targetElement || selfElement == 1 && targetElement == 3)
      {
        value = -1;
      }

      int copy = elementRecorder[self.GetComponent<MyCharacter>().status.index];
      elementRecorder[self.GetComponent<MyCharacter>().status.index] = elementRecorder[target.GetComponent<MyCharacter>().status.index];
      elementRecorder[target.GetComponent<MyCharacter>().status.index] = copy;


      return value;
    }
}
