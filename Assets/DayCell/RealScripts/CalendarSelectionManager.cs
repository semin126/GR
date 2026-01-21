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

    /// <summary>
    /// Called by DayCellSelection when a cell is clicked
    /// </summary>
    public void Select(DayCellSelection cell)
    {
        if (cell == null) return;

        // 클릭한 셀이 이미 선택된 셀이면 토글 해제
        if (current == cell)
        {
            current.SetSelected(false);
            current = null;

            if (selectedDateText != null)
                selectedDateText.text = "Selected Date: (none)";

            return;
        }

        // 기존 선택 해제
        if (current != null)
            current.SetSelected(false);

        // 새 선택
        current = cell;
        current.SetSelected(true);

        // 선택 날짜 표시
        if (selectedDateText != null)
        {
            DateTime date = cell.GetAssignedDate();
            selectedDateText.text = $"Selected Date: {date:yyyy-MM-dd} ({ToDow3(date)})";
        }
    }

    private string ToDow3(DateTime d)
    {
        // Mon Tue Wed Thu Fri Sat Sun
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
