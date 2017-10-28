using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

/// <summary>
///	管理日期
/// </summary>
public class DateManager : MonoBehaviour {

    DateTime dateTime;

    public Text lblDate;
	
    // Use this for initialization
    void Start () {
        dateTime = new DateTime();
        TickWheelManager.instance.AddEvent(TickEvent.CreateRepeat(1, 30, UpdateDate));
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	void UpdateDate () {
		DebugUtils.LogError("Date : " +TickWheelManager.instance.currentTick + "");
        dateTime = dateTime.AddDays(1);
        lblDate.text = dateTime.ToShortDateString();
    }
}
