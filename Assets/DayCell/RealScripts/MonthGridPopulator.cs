using System;
using UnityEngine;
using TMPro;

public class MonthGridPopulator : MonoBehaviour
{
    [Header("Target Month")]
    [SerializeField] private int year = 2026;
    [SerializeField] private int month = 1; // 1~12

    [Header("Grid Root (must have 42 children = day cells)")]
    [SerializeField] private Transform monthGridRoot;

    [Header("Title UI (optional)")]
    [SerializeField] private TextMeshProUGUI titleText; // TitleText 연결(없어도 됨)

    [Header("Number Colors")]
    [SerializeField] private Color inMonthColor = Color.black;
    [SerializeField] private Color outMonthColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private void Awake()
    {
        if (monthGridRoot == null)
            monthGridRoot = transform;
    }

    private void Start()
    {
        Populate();
    }

    public void GoPrevMonth()
    {
        if (month == 1)
        {
            month = 12;
            year--;
        }
        else
        {
            month--;
        }

        Populate();
    }

    public void GoNextMonth()
    {
        if (month == 12)
        {
            month = 1;
            year++;
        }
        else
        {
            month++;
        }

        Populate();
    }

    [ContextMenu("Populate Now")]
    public void Populate()
    {
        if (monthGridRoot == null)
            monthGridRoot = transform;

        if (monthGridRoot.childCount < 42)
        {
            Debug.LogError($"[MonthGridPopulator] Need 42 cells, current: {monthGridRoot.childCount}", this);
            return;
        }

        // 타이틀 업데이트
        if (titleText != null)
            titleText.text = $"{year:D4}-{month:D2}";

        DateTime firstDay = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);

        // Monday=0 ... Sunday=6
        int firstDayIndex = ((int)firstDay.DayOfWeek + 6) % 7;

        // prev month info
        int prevYear = (month == 1) ? year - 1 : year;
        int prevMonth = (month == 1) ? 12 : month - 1;
        int daysInPrevMonth = DateTime.DaysInMonth(prevYear, prevMonth);

        int prevCount = firstDayIndex;
        int prevStartDay = daysInPrevMonth - prevCount + 1;

        // fill prev tail
        for (int i = 0; i < prevCount; i++)
        {
            int d = prevStartDay + i;
            SetCell(i, new DateTime(prevYear, prevMonth, d), outMonthColor);
        }

        // fill current month
        int startIndex = firstDayIndex;
        int lastIndex = startIndex + daysInMonth - 1;

        for (int day = 1; day <= daysInMonth; day++)
        {
            int idx = startIndex + (day - 1);
            SetCell(idx, new DateTime(year, month, day), inMonthColor);
        }

        // fill next month
        int nextYear = (month == 12) ? year + 1 : year;
        int nextMonth = (month == 12) ? 1 : month + 1;

        int nextDay = 1;
        for (int i = lastIndex + 1; i < 42; i++)
        {
            SetCell(i, new DateTime(nextYear, nextMonth, nextDay), outMonthColor);
            nextDay++;
        }
    }

    private void SetCell(int index, DateTime date, Color numberColor)
    {
        Transform cell = monthGridRoot.GetChild(index);

        // 날짜 숫자
        var tmp = cell.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp != null)
        {
            tmp.text = date.Day.ToString();
            tmp.color = numberColor;
        }

        // 오늘 초록 표시(있으면)
        var sel = cell.GetComponent<DayCellSelection>();
        if (sel != null)
            sel.SetDate(date);
    }
}
