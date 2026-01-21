using UnityEngine;
using UnityEngine.EventSystems;

public class WeekLabelButton : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("1~6")]
    [SerializeField] private int weekIndex = 1;

    public void OnPointerClick(PointerEventData eventData)
    {
        var picker = FindFirstObjectByType<WeekFromMonthGridPicker>();
        if (picker != null)
            picker.OpenWeek(weekIndex);
    }
}
