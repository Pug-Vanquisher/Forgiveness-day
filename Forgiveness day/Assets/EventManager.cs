using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("EventManager");
                _instance = obj.AddComponent<EventManager>();
            }
            return _instance;
        }
    }

    // Словарь для хранения событий
    private Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();

    // Метод для подписки на события
    public void Subscribe(string eventName, UnityAction listener)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = new UnityEvent();
        }
        eventDictionary[eventName].AddListener(listener);
    }

    // Метод для отписки от событий
    public void Unsubscribe(string eventName, UnityAction listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName].RemoveListener(listener);
        }
    }

    // Метод для вызова событий
    public void TriggerEvent(string eventName)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName].Invoke();
        }
    }
}
