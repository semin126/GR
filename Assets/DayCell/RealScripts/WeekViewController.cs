using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeekViewController : MonoBehaviour
{
    [Header("Roots")]
    [SerializeField] private GameObject monthViewRoot;
    [SerializeField] private GameObject weekViewRoot;

    [Header("Week UI")]
    [SerializeField] private TextMeshProUGUI weekTitleText;
    [SerializeField] private Button backButton;

    [Header("7 Day Headers (Mon..Sun)")]
    [SerializeField] private TextMeshProUGUI[] dayHeaderTexts = new TextMeshProUGUI[7];

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(ShowMonthView);

        ShowMonthView();
    }

    // ✅ WeekFromMonthGridPicker가 호출하는 함수 이름: ShowWeekOf
    public void ShowWeekOf(DateTime anyDateInWeek)
    {
        DateTime monday = GetMonday(anyDateInWeek);

        if (monthViewRoot != null) monthViewRoot.SetActive(false);
        if (weekViewRoot != null) weekViewRoot.SetActive(true);

        if (weekTitleText != null)
            weekTitleText.text = $"WEEK OF {monday:yyyy-MM-dd}";

        for (int i = 0; i < 7; i++)
        {
            DateTime d = monday.AddDays(i);
            if (dayHeaderTexts != null && i < dayHeaderTexts.Length && dayHeaderTexts[i] != null)
                dayHeaderTexts[i].text = $"{ToDow3(d)} {d:MM-dd}";
        }
    }

    public void ShowMonthView()
    {
        if (monthViewRoot != null) monthViewRoot.SetActive(true);
        if (weekViewRoot != null) weekViewRoot.SetActive(false);
    }

    private DateTime GetMonday(DateTime d)
    {
        int idx = ((int)d.DayOfWeek + 6) % 7; // Mon=0..Sun=6
        return d.Date.AddDays(-idx);
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
