using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Talk : MonoBehaviour
{
    public GameObject name;
    public GameObject text;
    public GameObject pic;

    public GameObject layout;
    public Button nextButton;
    public Button[] selectionButtons;

    private CharacterTalkEvent curEvent;
    private int talkingIndex;
    private bool isTalking;
    private float playInterval = 0.05f;

    void Start()
    {
      for(int i = 0; i < selectionButtons.Length; i += 1)
      {
        int finalI = i;

        selectionButtons[i].onClick.AddListener(
          delegate
          {
            showSelectionButton(false);
            curEvent = curEvent.nextEvents[finalI];

            talkingIndex = 0;
            updateTextAndImage();
          }
        );
      }

      nextButton.onClick.AddListener(
        delegate
        {
          //in selection
          if(talkingIndex == curEvent.talks.Length)
          {
            return;
          }

          //pre-click
          if(isTalking)
          {
            text.GetComponent<TextMeshProUGUI>().text = curEvent.talks[talkingIndex].text;
            isTalking = false;
          }
          //next dia
          else
          {
            talkingIndex += 1;

            //this talk scene end
            if(talkingIndex == curEvent.talks.Length)
            {
              //show selections
              if(curEvent.nextEvents != null)
              {
                //enter a new event
                if(curEvent.nextEvents[0].selection != null)
                {
                  showSelectionButton(true);
                }
                else
                {
                  curEvent = curEvent.nextEvents[0];

                  talkingIndex = 0;
                  updateTextAndImage();
                }

              }
              //end scene
              else
              {
                layout.SetActive(false);
              }
            }
            //start next sentence
            else
            {
              updateTextAndImage();
            }
          }
        }
      );
    }

    void Update()
    {
      if(curEvent != null && talkingIndex != curEvent.talks.Length)
      {
        if(isTalking)
        {
          if(text.GetComponent<TextMeshProUGUI>().text.Length != curEvent.talks[talkingIndex].text.Length)
          {
            playInterval += Time.deltaTime;

            if(playInterval < 0.05f)
            {
              return;
            }

            text.GetComponent<TextMeshProUGUI>().text += curEvent.talks[talkingIndex].text[text.GetComponent<TextMeshProUGUI>().text.Length];
            playInterval = 0;
          }
          else
          {
            isTalking = false;
          }
        }

      }
    }

    public void startEvent(int id)
    {
      layout.SetActive(true);

      if(id == 0)
      {
        CharacterTalkEvent event1 = new CharacterTalkEvent();
        event1.talks = new CharacterTalk[]
        {
          new CharacterTalk("魔法少女A", "哇，这片森林好大啊，走不出去了", "Chara1"),
          new CharacterTalk("魔法少女B", "是啊，我们已经走了好久了，还是找不到出路", "Chara2"),
          new CharacterTalk("魔法少女C", "这可怎么办啊。。。", "Chara3"),
          new CharacterTalk("魔法少女B", "哇，你看那边有一个人！", "Chara2"),
          new CharacterTalk("魔法少女A", "哦！是熟悉这片森林的人吗？", "Chara1"),
          new CharacterTalk("魔法少女C", "有可能，快去问问她！", "Chara3"),
          new CharacterTalk("废物少女", "你们好，我是废物少女。我在这片森林中迷路了，还和家人走散了。你们能帮我找找他们吗？", "Chara4"),
        };

        CharacterTalkEvent event2_1 = new CharacterTalkEvent();
        event2_1.selection = "不帮忙";
        event2_1.talks = new CharacterTalk[]
        {
          new CharacterTalk("魔法少女A", "我们也被困在这里很久了，我们必须得先找到出路", "Chara1"),
          new CharacterTalk("废物少女", "。。是么", "Chara4"),
          new CharacterTalk("魔法少女A", "没关系，如果我们有余力，我们会帮你找到家人的", "Chara1"),
          new CharacterTalk("废物少女", "谢谢你们，我很感激", "Chara4"),
        };

        CharacterTalkEvent event2_2 = new CharacterTalkEvent();
        event2_2.selection = "帮忙";
        event2_2.talks = new CharacterTalk[]
        {
          new CharacterTalk("魔法少女A", "当然可以！我们是魔法少女，我们能使用魔法在这片深林中开路", "Chara1"),
          new CharacterTalk("废物少女", "非常感谢你们，我真的不知道该怎么办", "Chara4"),
          new CharacterTalk("魔法少女A", "没关系，我们是来帮助你的，一起努力，我们一定会找到你的家人", "Chara1"),
          new CharacterTalk("废物少女", "谢谢你们，我很感激", "Chara4"),
        };

        event1.nextEvents = new CharacterTalkEvent[]{event2_1, event2_2};

        CharacterTalkEvent event3 = new CharacterTalkEvent();
        event3.talks = new CharacterTalk[]
        {
          new CharacterTalk("魔法少女A", "那我们快点开始移动吧，再不出去就要寄乐", "Chara1")
        };

        event2_1.nextEvents = new CharacterTalkEvent[]{event3};
        event2_2.nextEvents = new CharacterTalkEvent[]{event3};

        curEvent = event1;
      }

      talkingIndex = 0;
      updateTextAndImage();
    }

    private void updateTextAndImage()
    {
      name.GetComponent<TextMeshProUGUI>().text = curEvent.talks[talkingIndex].name;
      text.GetComponent<TextMeshProUGUI>().text = "";
      pic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Characters/" + curEvent.talks[talkingIndex].pic);

      isTalking = true;
    }

    private void showSelectionButton(bool show)
    {
      for(int i = 0; i < selectionButtons.Length; i += 1)
      {
        selectionButtons[i].gameObject.SetActive(show);

        if(show)
        {
          selectionButtons[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = curEvent.nextEvents[i].selection;
        }
      }
    }
}

public class CharacterTalkEvent
{
  public CharacterTalk[] talks;
  public string selection;

  public CharacterTalkEvent[] nextEvents;
}

public class CharacterTalk
{
  public string name;
  public string text;
  public string pic;

  public CharacterTalk(string name, string text, string pic)
  {
    this.name = name;
    this.text = text;
    this.pic = pic;
  }
}
