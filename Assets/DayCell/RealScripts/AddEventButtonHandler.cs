using System;
using UnityEngine;

public class AddEventButtonHandler : MonoBehaviour
{
    public void AddEvent()
    {
        var db = CalendarEventDatabase.Instance;
        if (db == null)
        {
            Debug.LogError("CalendarEventDatabase not found");
            return;
        }

        DateTime today = DateTime.Today;

        db.CreateEventOnMultipleDates(
            new[] { today },
            "Demo Event",
            null
        );

        Debug.Log("Event added: " + today.ToShortDateString());
    }
}
