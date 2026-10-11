using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Scales a button up while the mouse is over it. Attach to the Button's GameObject.
[RequireComponent(typeof(Button))]
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverScale = 1.1f;

    private Vector3 originalScale;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;   // No hover while the controls are locked during a spin

        transform.localScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
}