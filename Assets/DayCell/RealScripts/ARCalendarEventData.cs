using System;
using UnityEngine;

[Serializable]
public class ARCalendarEventData
{
    public string id;
    public DateTime date;   // Date only
    public string title;

    public ARCalendarEventData(string id, DateTime date, string title)
    {
        this.id = id;
        this.date = date.Date;
        this.title = title;
    }
}
