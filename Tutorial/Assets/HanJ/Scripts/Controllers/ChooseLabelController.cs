using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ChooseLabelController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Color defaultColor;
    public Color hoverColor;
    private StoryScene scene;
    private TextMeshProUGUI textMesh;
    private ChooseController controller;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.color = defaultColor;
    }

    public float GetHeight()
    {
        return textMesh.rectTransform.sizeDelta.y * textMesh.rectTransform.localScale.y;
    }

    public void Setup(ChooseScene.ChooseLabel label, ChooseController controller, float y)
    {
        scene = label.nextScene;
        textMesh.text = label.text;
        this.controller = controller;

        Vector3 position = textMesh.rectTransform.localPosition;
        position.y = y;
        textMesh.rectTransform.localPosition = position;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //talk with trash with good choice
        if(textMesh.text == "")
        {
          controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().A += 1f;//A跟着talkprogress更新
        }

        //give food
        if(textMesh.text == "Sure, we are magical girl [-1 Pineapple]" || textMesh.text == "Sure, we are magical girl [-1 Bread]" || textMesh.text == "We have been trapped a long time, we don't have enough food...")
        {
          if(textMesh.text == "Sure, we are magical girl [-1 Pineapple]")
          {
            controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().itemRemaining[1] -= 1;

            controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().N += 1f;
          }
          else if(textMesh.text == "Sure, we are magical girl [-1 Bread]")
          {
            controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().itemRemaining[0] -= 1;

            controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().N += 1f;
          }

          controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().foodCount += 1;
        }

        if(textMesh.text == "The monster are all over the forest, your family may deade" || textMesh.text == "We have been a long time, you family may already deade")
        {
          if(controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().flag[0])
          {
            scene = (StoryScene) controller.gameController.scenes[7];
          }
          else
          {
            scene = (StoryScene) controller.gameController.scenes[8];
          }
        }

        if(textMesh.text == "Find")
        {
          if(Random.Range(0f, 1f) < 0.1f)
          {
            controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().E += 1;
            controller.worldController.GetComponent<WorldMapController>().getMap().spots[controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().currentSpotID].type = 0;

            scene = (StoryScene) controller.gameController.scenes[11];
          }
        }

        if(textMesh.text == "Continue to explore this place")
        {
          if(Random.Range(0f, 1f) < 0.5f)
          {
            if(Random.Range(0f, 1f) < 0.5f)
            {
              scene = (StoryScene) controller.gameController.scenes[14];
              controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().itemRemaining[0] += 2;
            }
            else
            {
              scene = (StoryScene) controller.gameController.scenes[15];
              controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().itemRemaining[1] += 1;
            }
          }
          else
          {
            scene = (StoryScene) controller.gameController.scenes[16];
          }

          controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().C += 1f;

          controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().treasureCount += 1;
        }
        else if(textMesh.text == "Leave this place")
        {
          controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().treasureCount += 1;
        }

        controller.PerformChoose(scene);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        textMesh.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textMesh.color = defaultColor;
    }
}
