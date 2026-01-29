using System;
using TMPro;
using UnityEngine;

public class CalendarSelectionManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI selectedDateText;

    private DayCellSelection current;

    private void Start()
    {
        if (selectedDateText != null)
            selectedDateText.text = "Selected Date: (none)";
    }

    public void Select(DayCellSelection cell)
    {
        if (cell == null) return;

        if (current == cell)
        {
            current.SetSelected(false);
            current = null;

            if (selectedDateText != null)
                selectedDateText.text = "Selected Date: (none)";

            return;
        }

        if (current != null)
            current.SetSelected(false);

        current = cell;
        current.SetSelected(true);

        UpdateSelectedText();
    }

    public bool HasSelection()
    {
        return current != null;
    }

    public DateTime GetSelectedDate()
    {
        return current != null ? current.GetAssignedDate() : DateTime.Today;
    }

    public void RefreshSelectedTextWithEventCount()
    {
        UpdateSelectedText();
    }

    private void UpdateSelectedText()
    {
        if (selectedDateText == null) return;

        if (current == null)
        {
            selectedDateText.text = "Selected Date: (none)";
            return;
        }

        DateTime date = current.GetAssignedDate();
        int count = 0;

        var db = CalendarEventDatabase.Instance;
        if (db != null)
            count = db.GetEventsOn(date).Count;

        selectedDateText.text =
            $"Selected Date: {date:yyyy-MM-dd} ({ToDow3(date)})  |  Events: {count}";
    }

    private string ToDow3(DateTime d)
    {
        return d.DayOfWeek switch
        {
            DayOfWeek.Monday => "Mon",
            DayOfWeek.Tuesday => "Tue",
            DayOfWeek.Wednesday => "Wed",
            DayOfWeek.Thursday => "Thu",
            DayOfWeek.Friday => "Fri",
            DayOfWeek.Saturday => "Sat",
            DayOfWeek.Sunday => "Sun",
            _ => ""
        };
    }
}
