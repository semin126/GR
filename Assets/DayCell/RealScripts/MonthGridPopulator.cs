using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MonthGridPopulator : MonoBehaviour
{
    [Header("Target Month")]
    [SerializeField] private int year = 2026;
    [SerializeField] private int month = 1; // 1~12

    [Header("Grid Root (must have 42 children = day cells)")]
    [SerializeField] private Transform monthGridRoot;

    [Header("Title UI (optional)")]
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Number Colors")]
    [SerializeField] private Color inMonthColor = Color.black;
    [SerializeField] private Color outMonthColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("Event Icon Rendering (Month View)")]
    [Tooltip("DayCell 안의 아이콘 루트 오브젝트 이름")]
    [SerializeField] private string eventIconsRootName = "EventIconRoot";

    [Tooltip("한 날짜에 최대 몇 개 아이콘까지 보여줄지")]
    [SerializeField] private int maxIconsPerDayCell = 3;

    private readonly Color[] debugColors =
    {
        new Color(0.9f, 0.3f, 0.3f, 1f),
        new Color(0.3f, 0.9f, 0.3f, 1f),
        new Color(0.3f, 0.6f, 0.95f, 1f),
        new Color(0.95f, 0.85f, 0.3f, 1f),
        new Color(0.75f, 0.3f, 0.95f, 1f)
    };

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
        month--;
        if (month < 1)
        {
            month = 12;
            year--;
        }
        Populate();
    }

    public void GoNextMonth()
    {
        month++;
        if (month > 12)
        {
            month = 1;
            year++;
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

        if (titleText != null)
            titleText.text = $"{year:D4}-{month:D2}";

        DateTime firstDay = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);

        // Monday=0 ... Sunday=6
        int firstDayIndex = ((int)firstDay.DayOfWeek + 6) % 7;

        int prevYear = (month == 1) ? year - 1 : year;
        int prevMonth = (month == 1) ? 12 : month - 1;
        int daysInPrevMonth = DateTime.DaysInMonth(prevYear, prevMonth);

        int prevCount = firstDayIndex;
        int prevStartDay = daysInPrevMonth - prevCount + 1;

        for (int i = 0; i < prevCount; i++)
        {
            int d = prevStartDay + i;
            SetCell(i, new DateTime(prevYear, prevMonth, d), outMonthColor);
        }

        int startIndex = firstDayIndex;
        int lastIndex = startIndex + daysInMonth - 1;

        for (int day = 1; day <= daysInMonth; day++)
        {
            int idx = startIndex + (day - 1);
            SetCell(idx, new DateTime(year, month, day), inMonthColor);
        }

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

        var tmp = cell.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp != null)
        {
            tmp.text = date.Day.ToString();
            tmp.color = numberColor;
        }

        var sel = cell.GetComponent<DayCellSelection>();
        if (sel != null)
            sel.SetDate(date);

        RenderEventIconsForCell(cell, date);
    }

    private void RenderEventIconsForCell(Transform cell, DateTime date)
    {
        Transform root = cell.Find(eventIconsRootName);
        if (root == null) return;

        // 기존 아이콘 제거
        for (int i = root.childCount - 1; i >= 0; i--)
            Destroy(root.GetChild(i).gameObject);

        var db = CalendarEventDatabase.Instance;
        if (db == null) return;

        var events = db.GetEventsOn(date);
        int count = Mathf.Min(maxIconsPerDayCell, events.Count);

        for (int i = 0; i < count; i++)
        {
            var ev = events[i];

            var go = new GameObject("EvIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(root, false);

            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(16, 16);

            var img = go.GetComponent<Image>();

            // 아이콘이 있으면 스프라이트로, 없으면 색으로 표시(마감용 안전장치)
            if (ev.icon != null)
            {
                img.sprite = ev.icon;
                img.preserveAspect = true;
                img.color = Color.white;
            }
            else
            {
                img.color = debugColors[i % debugColors.Length];
            }
        }
    }
}
