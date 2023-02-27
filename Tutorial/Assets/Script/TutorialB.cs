using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialB : MonoBehaviour
{
    public Button nextButton;
    private GameObject runningLayout;

    private string lastVoice;
    private bool[] trigger = new bool[]{false, false};

    public void showMessage(int curCharacterID)
    {
      GameObject[] characters = GetComponent<Controller>().characters;

      float tankhp = characters[0].GetComponent<MyCharacter>().status.curHp / characters[0].GetComponent<MyCharacter>().status.maxHp;
      float selfhp = characters[curCharacterID].GetComponent<MyCharacter>().status.curHp / characters[curCharacterID].GetComponent<MyCharacter>().status.maxHp;
      float bosshp = characters[5].GetComponent<MyCharacter>().status.curHp / characters[5].GetComponent<MyCharacter>().status.maxHp;

      int life = 0;
      life = GetComponent<Controller>().isActive[0] ? life + 1 : life;
      life = GetComponent<Controller>().isActive[1] ? life + 1 : life;
      life = GetComponent<Controller>().isActive[2] ? life + 1 : life;

      bool tauntReady = characters[0].GetComponent<MyCharacter>().status.skillsCoolDown[0] == 0;
      bool tauntMpEnough = characters[0].GetComponent<MyCharacter>().status.curMp >= 20;

      bool depploReady = characters[1].GetComponent<MyCharacter>().status.skillsCoolDown[0] == 0;
      bool depploMpEnough = characters[1].GetComponent<MyCharacter>().status.curMp >= 40;

      bool healingReady = characters[2].GetComponent<MyCharacter>().status.skillsCoolDown[0] == 0;

      int chargingRemaining = 0;

      for(int i = 0; i < characters[5].GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        if(((Buff)characters[5].GetComponent<MyCharacter>().status.buff[i]).id == 8)
        {
          chargingRemaining = ((Buff)characters[5].GetComponent<MyCharacter>().status.buff[i]).remainingTurn;
        }
      }

      int tauntingBuffRemaining = 0;
      for(int i = 0; i < characters[0].GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        if(!GetComponent<Controller>().isActive[0])
        {
          break;
        }

        if(((Buff)characters[0].GetComponent<MyCharacter>().status.buff[i]).id == 1)
        {
          tauntingBuffRemaining = ((Buff)characters[0].GetComponent<MyCharacter>().status.buff[i]).remainingTurn;
        }
      }

      int deathDebuffCount = 0;

      for(int i = 0; i < characters[5].GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        if(((Buff)characters[5].GetComponent<MyCharacter>().status.buff[i]).id == 4)
        {
          deathDebuffCount += 1;
        }
      }

      bool allHealthy = true;

      for(int i = 0; i < 3; i += 1)
      {
        if(!GetComponent<Controller>().isActive[i])
        {
          continue;
        }

        if(characters[i].GetComponent<MyCharacter>().status.curHp / characters[i].GetComponent<MyCharacter>().status.maxHp < 0.9f)
        {
          allHealthy = false;
        }
      }

      bool someoneUnhealthy = false;

      for(int i = 0; i < 3; i += 1)
      {
        if(!GetComponent<Controller>().isActive[i])
        {
          continue;
        }

        if(characters[i].GetComponent<MyCharacter>().status.curHp / characters[i].GetComponent<MyCharacter>().status.maxHp < 0.3f)
        {
          someoneUnhealthy = true;
        }
      }

      bool isBasaka = false;

      for(int i = 0; i < characters[5].GetComponent<MyCharacter>().status.buff.Count; i += 1)
      {
        if(((Buff)characters[5].GetComponent<MyCharacter>().status.buff[i]).id == 12)
        {
          isBasaka = true;
        }
      }

      //if first turn
      if(!trigger[0])
      {
        trigger[0] = true;
        showMessage(curCharacterID, "It's my turn! Use the command to attck enemy!");
        return;
      }

      //bersaka
      if(isBasaka && !trigger[1])
      {
        trigger[1] = true;
        showMessage(curCharacterID, "Watch out! The monster went berserk!");

        return;
      }

      ArrayList possibleVoice = new ArrayList();

      //last part of the battle
      if(bosshp < 0.2f && life < 3)
      {
        possibleVoice.Add("The enemy is weaken, but we are same. We should attack it as we can");
      }

      //last part of battle
      if(curCharacterID == 0 && life <= 2 && deathDebuffCount >= 1 && bosshp < 0.3f)
      {
        possibleVoice.Add("I must hold on to wait the debuff deal the damage to the boss...");
      }

      //all good
      if(allHealthy)
      {
        possibleVoice.Add("We are in the good status, we should attack boss now");
      }

      //charging
      if(chargingRemaining > 0)
      {
        if(tankhp > 0.5f && chargingRemaining < tauntingBuffRemaining)
        {
          possibleVoice.Add("We are safe because of taunting, keeping attack the boss");
        }
      }

      if(chargingRemaining > 0)
      {
        if(curCharacterID == 0 && tauntReady && !allHealthy)
        {
          possibleVoice.Add("Boss is prepare the strong attack, i should use taunt to protect memebers");
        }
        else if(!allHealthy)
        {
          possibleVoice.Add("Boss is prepare the strong attack, we should take defence right now");
        }
      }

      if(curCharacterID == 0 && tauntReady && someoneUnhealthy)
      {
        if(tauntMpEnough)
        {
          possibleVoice.Add("Someone hp is low, it'd be better to use taunt");
        }
        else
        {
          possibleVoice.Add("No MP to taunt, should save more MP before...");
        }
      }

      //strong hit
      if(curCharacterID == 1 && depploReady && depploMpEnough)
      {
        possibleVoice.Add("My strongest skill is ready!");
      }

      //heal
      if(curCharacterID == 2 && healingReady && !allHealthy)
      {
        possibleVoice.Add("Maybe I should heal someone right now");
      }

      if(curCharacterID == 2 && healingReady && someoneUnhealthy)
      {
        possibleVoice.Add("I should heal immediately");
      }

      //choose voice, if none return
      if(possibleVoice.Count > 0)
      {
        int nextVoiceIndex = Random.Range(0, possibleVoice.Count - 1);

        do
        {Debug.Log(possibleVoice.Count);
          if(lastVoice == (string)possibleVoice[nextVoiceIndex])
          {
            possibleVoice.RemoveAt(nextVoiceIndex);
          }
          else
          {
            lastVoice = (string)possibleVoice[nextVoiceIndex];
            showMessage(curCharacterID, lastVoice);

            return;
          }
        }
        while(possibleVoice.Count != 0);
      }

      GetComponent<Controller>().showCommandLayout(true);
    }

    private void showMessage(int curCharacterID, string text)
    {
      runningLayout = GetComponent<Controller>().characters[curCharacterID].GetComponent<MyCharacter>().findObject("ConversationLayout");
      runningLayout.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = getPrefix(curCharacterID) + text;

      nextButton.gameObject.SetActive(true);
      runningLayout.SetActive(true);

      nextButton.onClick.RemoveAllListeners();
      nextButton.onClick.AddListener(
        delegate
        {
          GetComponent<Controller>().showCommandLayout(true);
          runningLayout.SetActive(false);
          nextButton.gameObject.SetActive(false);
        }
      );
    }

    public void startOP()
    {
      int curIndex = 1;

      string[] message = new string[]{
        "We aren't to bully weak monsters, but we won't be easily defeated!",
        "For our mission and glory, we must defeat this monster",
        "I will use my shield to protect you from the attack!",
        "This monster looks very fierce, but we're not weak either",
        "The three of us go together, we can definitely defeat it!",
        "This monster is one of the most powerful enemies that we have met",
        "Are you guys ready?",
        "I will heal you using my power, lets GOoooo!"
      };

      int[] characterIndex = new int[]{0, 0, 0, 1, 1, 0, 0, 2, 2};

      nextButton.onClick.AddListener(
        delegate
        {
          if(runningLayout != null)
          {
            runningLayout.SetActive(false);
          }

          if(curIndex >= message.Length)
          {
            nextButton.gameObject.SetActive(false);
            GetComponent<Controller>().startGame();

            return;
          }

          runningLayout = GetComponent<Controller>().characters[characterIndex[curIndex]].GetComponent<MyCharacter>().findObject("ConversationLayout");
          runningLayout.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = getPrefix(characterIndex[curIndex]) + message[curIndex];

          nextButton.gameObject.SetActive(true);
          runningLayout.SetActive(true);

          curIndex += 1;
        }
      );
    }

    private string getPrefix(int index)
    {
      return "[" + GetComponent<Controller>().characters[index].GetComponent<MyCharacter>().parameter.name + "]\n";
    }

}
