using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DayCellSelection : MonoBehaviour
{
    [SerializeField] private CalendarSelectionManager manager;

    private Outline selectionOutline;
    private Outline todayOutline;

    private DateTime assignedDate;
    private bool hasDateAssigned;

    private static readonly Color SelectionYellow = new Color(1f, 1f, 0f, 1f);
    private static readonly Color TodayGreen     = new Color(0f, 1f, 0f, 1f);

    private static readonly Vector2 SelectionThickness = new Vector2(6f, -6f);
    private static readonly Vector2 TodayThickness     = new Vector2(9f, -9f);

    private void Awake()
    {
        EnsureOutlines();
        selectionOutline.enabled = false;
        todayOutline.enabled = false;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void Start()
    {
        if (manager == null)
            manager = FindFirstObjectByType<CalendarSelectionManager>();
    }

    private void EnsureOutlines()
    {
        var outlines = GetComponents<Outline>();

        if (outlines.Length == 0)
        {
            selectionOutline = gameObject.AddComponent<Outline>();
            todayOutline = gameObject.AddComponent<Outline>();
        }
        else if (outlines.Length == 1)
        {
            selectionOutline = outlines[0];
            todayOutline = gameObject.AddComponent<Outline>();
        }
        else
        {
            selectionOutline = outlines[0];
            todayOutline = outlines[1];
        }

        selectionOutline.effectColor = SelectionYellow;
        selectionOutline.effectDistance = SelectionThickness;

        todayOutline.effectColor = TodayGreen;
        todayOutline.effectDistance = TodayThickness;
    }

    private void OnClick()
    {
        if (manager != null)
            manager.Select(this);
        else
            SetSelected(true);
    }

    public void SetSelected(bool selected)
    {
        if (selectionOutline != null)
            selectionOutline.enabled = selected;
    }

    // MonthGridPopulator가 매 셀에 날짜를 주입할 때 호출
    public void SetDate(DateTime date)
    {
        assignedDate = date;
        hasDateAssigned = true;

        if (todayOutline != null)
            todayOutline.enabled = (date.Date == DateTime.Today);
    }

    // ✅ CalendarSelectionManager가 읽어갈 함수(이 이름이 중요!)
    public DateTime GetAssignedDate()
    {
        return hasDateAssigned ? assignedDate : DateTime.Today;
    }
}
