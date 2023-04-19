using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotBarController : MonoBehaviour
{
    public TextMeshProUGUI barText;
    public TextMeshProUGUI characterNameText;
    public Image potrait;

    private int sentenceIndex = 0;
    public StoryScene currentScene;
    private State state = State.COMPLETED;

    private enum State
    {
        PLAYING, COMPLETED
    }

    // Start is called before the first frame update
    public void PlayNextScene()
    {
        StartCoroutine(TypeText(currentScene.sentences[++sentenceIndex].text));
        characterNameText.text = currentScene.sentences[sentenceIndex].speaker.speakerName;
        characterNameText.color = currentScene.sentences[sentenceIndex].speaker.textColor;
        potrait.sprite = currentScene.sentences[sentenceIndex].speaker.chaPotrait;
    }

    public void PlayScene(StoryScene scene)
    {
        currentScene = scene;
        sentenceIndex = -1;
        PlayNextScene();
    }

    private IEnumerator TypeText(string text)
    {
        barText.text = "";
        state = State.PLAYING;
        int wordIndex = 0;

        while (state != State.COMPLETED)
        {
            barText.text += text[wordIndex];
            yield return new WaitForSeconds(0.05f);
            if(++wordIndex == text.Length)
            {
                state = State.COMPLETED;
                break;
            }
        }
    }


    public bool IsCompleted()
    {
        return state == State.COMPLETED;
    }

    public bool IsLastSentence()
    {
        return sentenceIndex + 1 == currentScene.sentences.Count;

    }
}
