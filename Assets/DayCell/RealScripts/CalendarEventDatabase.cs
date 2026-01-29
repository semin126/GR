using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CalendarEventDatabase : MonoBehaviour
{
    public static CalendarEventDatabase Instance { get; private set; }

    private readonly Dictionary<DateTime, List<CalendarEvent>> eventsByDate = new();
    private readonly Dictionary<string, CalendarEvent> eventsById = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public IReadOnlyList<CalendarEvent> GetEventsOn(DateTime date)
    {
        date = date.Date;
        if (eventsByDate.TryGetValue(date, out var list)) return list;
        return Array.Empty<CalendarEvent>();
    }

    public List<CalendarEvent> CreateEventOnMultipleDates(IEnumerable<DateTime> dates, string title, Sprite icon)
    {
        var created = new List<CalendarEvent>();
        if (dates == null) return created;

        foreach (var d in dates.Select(x => x.Date).Distinct())
        {
            string id = Guid.NewGuid().ToString("N");
            var ev = new CalendarEvent(id, d, title, icon);
            AddInternal(ev);
            created.Add(ev);
        }

        return created;
    }

    private void AddInternal(CalendarEvent ev)
    {
        eventsById[ev.id] = ev;

        if (!eventsByDate.TryGetValue(ev.date, out var list))
        {
            list = new List<CalendarEvent>();
            eventsByDate[ev.date] = list;
        }
        list.Add(ev);
    }

    // (테스트용) Inspector 우클릭 메뉴
    [ContextMenu("DEBUG: Print All Events")]
    private void DebugPrintAll()
    {
        foreach (var kv in eventsByDate.OrderBy(k => k.Key))
        {
            Debug.Log($"[{kv.Key:yyyy-MM-dd}] count={kv.Value.Count} titles={string.Join(" | ", kv.Value.Select(e => e.title))}");
        }
    }
}
