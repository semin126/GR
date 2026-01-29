using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeekEventDetailsPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    public void Show(ARCalendarEventData ev)
    {
        if (ev == null) return;
        if (titleText != null) titleText.text = ev.title;
        if (dateText != null) dateText.text = $"{ev.date:yyyy-MM-dd}";
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);
}
