using System;
using System.Collections;
using UnityEngine;

public class TimerManager : Singleton<TimerManager>
{
    [SerializeField, Tooltip("Event du timer")]
    private EventsTrigger m_event;
    
    [SerializeField, Tooltip("Le timer est de X minutes")]
    private int m_minuteStart;

    private WaitForSeconds m_waitOneSeconde = new WaitForSeconds(1);
    
    private bool m_isRunning;
    private bool m_isStart;
    
    private int m_timerMinuteValue;
    private int m_timerHourValue;
    private string m_valueString;
    
    public string StringValue => m_valueString;
    
    private void OnEnable()
    {
        if (m_event == null) return;
        m_event.OnTrigger += StartOrStopTimer;
    }
    private void OnDisable()
    {
        if (m_event == null) return;
        m_event.OnTrigger -= StartOrStopTimer;
    }

    private void StartOrStopTimer(bool p_isStart)
    {
        if (p_isStart)
        {
            m_isStart = true;
            StartCoroutine(IncreaseTime());
            return;
        }
        
        m_isStart = false;
        StopAllCoroutines();
    }
    
    /// <summary>
    /// relancer ou mettre en pause le timer
    /// </summary>
    /// <param name="p_isStart"> true: lancer / flase: arrêter</param>
    public void PauseOrRestartTimer(bool p_isStart)
    {
        if (p_isStart)
        {
            m_isRunning = true;
            
            m_timerHourValue = m_minuteStart;
            m_timerMinuteValue = 0;
            StartCoroutine(IncreaseTime());
            return;
        }
        
        m_isRunning = false;
        StopAllCoroutines();
    }
    
    //Minuteur
    private void UpdateTimerValue()
    {
        if(!m_isRunning || !m_isStart) return;

        m_timerMinuteValue--;

        if (m_timerMinuteValue <= 0)
        {
            if (m_timerHourValue == 0)
            {
                PlayerManager.Instance.Death();
                return;
            }
            m_timerHourValue--;
            m_timerMinuteValue = 59;
        }

        UpdateStringValue();
        
        Debug.Log(m_valueString);
        
        StartCoroutine(IncreaseTime());
    }

    //Mettre les Minutes et secondes en strings pour les textes qui l'utiliserons
    private void UpdateStringValue()
    {
        string hours = "";
        string minute = "";
        
        if (m_timerHourValue < 10) hours = "0";
        if (m_timerMinuteValue < 10) minute = "0";

        hours += m_timerHourValue.ToString();
        minute += m_timerMinuteValue.ToString();
        
        m_valueString = $"{hours} : {minute}";
    }

    IEnumerator IncreaseTime()
    {
        yield return m_waitOneSeconde;
        UpdateTimerValue();
    }

    protected override string GetSingletonName()
    {
        return "TimerManager";
    }
}