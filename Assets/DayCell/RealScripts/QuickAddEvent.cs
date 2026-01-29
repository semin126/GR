using System;
using UnityEngine;

public class QuickAddEvent : MonoBehaviour
{
    public void AddTodayEvent()
    {
        var db = CalendarEventDatabase.Instance;
        if (db == null) return;

        DateTime today = DateTime.Today;

        db.CreateEventOnMultipleDates(
            new[] { today },
            "Demo Event",
            null
        );
    }
}
