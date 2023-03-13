using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        value = 4;
      }
      //bad
      else if(selfElement + 1 == targetElement || selfElement == 1 && targetElement == 3)
      {
        value = -1;
      }

      int copy = elementRecorder[self.GetComponent<MyCharacter>().status.index];
      elementRecorder[self.GetComponent<MyCharacter>().status.index] = elementRecorder[target.GetComponent<MyCharacter>().status.index];
      elementRecorder[target.GetComponent<MyCharacter>().status.index] = copy;

      return value;
    }
}
