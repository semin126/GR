using System;
using TMPro;
using UnityEngine;

public class CalendarUIController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private MonthGridPopulator populator;
    [SerializeField] private TextMeshProUGUI selectedDateText;

    private void Awake()
    {
        if (populator == null)
            populator = FindFirstObjectByType<MonthGridPopulator>();

        if (selectedDateText != null)
            selectedDateText.text = "선택한 날짜: (없음)";
    }

    public void OnDateSelected(DateTime date)
    {
        if (selectedDateText == null) return;

        // 월요일 시작 달력이라도 요일은 DateTime에서 그대로 계산
        string dowKorean = date.DayOfWeek switch
        {
            DayOfWeek.Monday => "월",
            DayOfWeek.Tuesday => "화",
            DayOfWeek.Wednesday => "수",
            DayOfWeek.Thursday => "목",
            DayOfWeek.Friday => "금",
            DayOfWeek.Saturday => "토",
            DayOfWeek.Sunday => "일",
            _ => ""
        };

        selectedDateText.text = $"선택한 날짜: {date:yyyy-MM-dd} ({dowKorean})";
    }
}
