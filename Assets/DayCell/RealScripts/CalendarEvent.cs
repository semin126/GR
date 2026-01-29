using System;
using UnityEngine;

[Serializable]
public class CalendarEvent
{
    public string id;
    public DateTime date;
    public string title;
    public Sprite icon;

    // CalendarEventDatabase가 요구하는 생성자:
    // new CalendarEvent(id, d, title, icon)
    public CalendarEvent(string id, DateTime date, string title, Sprite icon)
    {
        this.id = id;
        this.date = date.Date;
        this.title = title;
        this.icon = icon;
    }
}
