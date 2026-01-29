using System;
using UnityEngine;

public class WeekFromMonthGridPicker : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform monthGridRoot;     // MonthGrid (42 children)
    [SerializeField] private WeekViewController weekView; // ✅ Inspector로 연결
    [SerializeField] private GameObject weekViewRoot;

    private void Awake()
    {
        if (monthGridRoot == null)
            monthGridRoot = transform;
    }

    public void OpenWeek(int weekIndex)
    {
        if (weekViewRoot == null || weekView == null)
        {
            Debug.LogError("[WeekFromMonthGridPicker] WeekView references not set", this);
            return;
        }

        if (monthGridRoot == null || monthGridRoot.childCount < 42)
        {
            Debug.LogError(
                $"[WeekFromMonthGridPicker] MonthGridRoot needs 42 cells, current: {(monthGridRoot == null ? 0 : monthGridRoot.childCount)}",
                this
            );
            return;
        }

        weekIndex = Mathf.Clamp(weekIndex, 1, 6);
        int row = weekIndex - 1;
        int rowStart = row * 7;

        DayCellSelection mondayCell = null;
        DayCellSelection firstCell = null;

        for (int i = 0; i < 7; i++)
        {
            int idx = rowStart + i;
            var cell = monthGridRoot.GetChild(idx);
            var sel = cell.GetComponent<DayCellSelection>();
            if (sel == null) continue;

            if (firstCell == null)
                firstCell = sel;

            DateTime d = sel.GetAssignedDate();
            if (d.DayOfWeek == DayOfWeek.Monday)
            {
                mondayCell = sel;
                break;
            }
        }

        var chosen = mondayCell != null ? mondayCell : firstCell;
        if (chosen == null) return;

        // 🔴 이 한 줄이 핵심
        weekViewRoot.SetActive(true);

        weekView.ShowWeekOf(chosen.GetAssignedDate());
    }

}
