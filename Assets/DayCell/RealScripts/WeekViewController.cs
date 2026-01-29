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

    [Header("Week Grid (Bottom white cells)")]
    [Tooltip("WeekGridRoot that contains Cell(0)~Cell(6) as children")]
    [SerializeField] private Transform weekGridRoot;

    [Tooltip("Optional: TMP child name inside each cell for date number (if exists). Leave as DateText if you created it.")]
    [SerializeField] private string cellDateTextName = "DateText";

    [Header("Popup (Week View)")]
    [SerializeField] private GameObject eventDetailPopup;
    [SerializeField] private TextMeshProUGUI popupTitleText;
    [SerializeField] private TextMeshProUGUI popupDateText;
    [SerializeField] private Button popupCloseButton;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(ShowMonthView);

        if (popupCloseButton != null)
            popupCloseButton.onClick.AddListener(ClosePopup);

        ClosePopup();
    }

    // ✅ WeekFromMonthGridPicker가 호출하는 함수 이름: ShowWeekOf
    public void ShowWeekOf(DateTime anyDateInWeek)
    {
        DateTime monday = GetMonday(anyDateInWeek);

        if (monthViewRoot != null) monthViewRoot.SetActive(false);
        if (weekViewRoot != null) weekViewRoot.SetActive(true);

        if (weekTitleText != null)
            weekTitleText.text = $"WEEK OF {monday:yyyy-MM-dd}";

        // 상단 헤더 (Day0~Day6)
        for (int i = 0; i < 7; i++)
        {
            DateTime d = monday.AddDays(i);
            if (dayHeaderTexts != null && i < dayHeaderTexts.Length && dayHeaderTexts[i] != null)
                dayHeaderTexts[i].text = $"{ToDow3(d)} {d:MM-dd}";
        }

        // 하단 흰 칸 (Cell 0~6) 클릭/날짜 세팅
        SetupWeekCells(monday);

        ClosePopup();
    }

    public void ShowMonthView()
    {
        if (monthViewRoot != null)
            monthViewRoot.SetActive(true);

        ClosePopup();

        // 핵심
        StartCoroutine(DisableWeekViewNextFrame());
    }

    private System.Collections.IEnumerator DisableWeekViewNextFrame()
    {
        yield return null; // 다음 프레임
        if (weekViewRoot != null)
            weekViewRoot.SetActive(false);
    }


    private void SetupWeekCells(DateTime monday)
    {
        if (weekGridRoot == null) return;
        if (weekGridRoot.childCount < 7) return;

        for (int i = 0; i < 7; i++)
        {
            Transform cell = weekGridRoot.GetChild(i);
            if (cell == null) continue;

            DateTime d = monday.AddDays(i);

            // (옵션) Cell 안에 DateText가 있으면 날짜 숫자 넣어줌
            var dateTmp = FindTmpInCell(cell, cellDateTextName);
            if (dateTmp != null)
                dateTmp.text = d.Day.ToString();

            // Cell에 Button이 없으면 런타임에서 붙여줌(최소구현용)
            Button btn = cell.GetComponent<Button>();
            if (btn == null)
            {
                btn = cell.gameObject.AddComponent<Button>();

                // TargetGraphic 자동 지정(있으면)
                var img = cell.GetComponent<Image>();
                if (img != null) btn.targetGraphic = img;
            }

            // 클릭 리스너
            btn.onClick.RemoveAllListeners();
            DateTime captured = d; // 클로저 버그 방지
            btn.onClick.AddListener(() => OpenPopup(captured));
        }
    }

    private TextMeshProUGUI FindTmpInCell(Transform cell, string childName)
    {
        // 1) 이름으로 찾기
        if (!string.IsNullOrEmpty(childName))
        {
            Transform t = cell.Find(childName);
            if (t != null)
            {
                var tmp = t.GetComponent<TextMeshProUGUI>();
                if (tmp != null) return tmp;
            }
        }

        // 2) 없으면 자식 TMP 하나라도 찾기(혹시 DateText 이름 안 맞아도 동작)
        return cell.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    private void OpenPopup(DateTime date)
    {
        if (eventDetailPopup != null) eventDetailPopup.SetActive(true);

        if (popupTitleText != null)
            popupTitleText.text = "Event Detail";

        if (popupDateText != null)
            popupDateText.text = date.ToString("yyyy-MM-dd");
    }

    private void ClosePopup()
    {
        if (eventDetailPopup != null)
            eventDetailPopup.SetActive(false);
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
