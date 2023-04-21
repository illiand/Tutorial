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
        controller.PerformChoose(scene);

        //choose to find food in the danger zone
        if(textMesh.text == "")
        {
          controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().C += 1f;

          controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().treasureCount += 1;
        }

        //talk with trash with good choice
        if(textMesh.text == "")
        {
          controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().A += 1f;
        }

        //give food
        if(textMesh.text == "")
        {
          controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().N += 1f;

          controller.worldController.GetComponent<WorldMapController>().getPlayerStatus().foodCount += 1;
        }
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
