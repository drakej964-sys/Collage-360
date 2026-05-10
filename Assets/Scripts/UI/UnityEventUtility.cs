using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor.Events;
#endif

public static class UnityEventUtility
{
    public static void RegisterListener(UnityEvent unityEvent, UnityAction unityAction)
    {
        if (unityEvent == null || unityAction == null)
            return;

#if UNITY_EDITOR
        UnityEventTools.RemovePersistentListener(unityEvent, unityAction);
        UnityEventTools.AddPersistentListener(unityEvent, unityAction);
#else
        unityEvent.RemoveListener(unityAction);
        unityEvent.AddListener(unityAction);
#endif
    }

    public static void UnregisterListener(UnityEvent unityEvent, UnityAction unityAction)
    {
        if (unityEvent == null || unityAction == null)
            return;

#if UNITY_EDITOR
        UnityEventTools.RemovePersistentListener(unityEvent, unityAction);
#else
        unityEvent.RemoveListener(unityAction);
#endif
    }

    public static void AddListener(UnityEvent unityEvent, UnityAction unityAction)
    {
        if (unityEvent == null || unityAction == null)
            return;

#if UNITY_EDITOR
        UnityEventTools.AddPersistentListener(unityEvent, unityAction);
#else
        unityEvent.AddListener(unityAction);
#endif
    }

    public static void RemoveListener(UnityEvent unityEvent, UnityAction unityAction)
    {
        if (unityEvent == null || unityAction == null)
            return;

#if UNITY_EDITOR
        UnityEventTools.RemovePersistentListener(unityEvent, unityAction);
#else
        unityEvent.RemoveListener(unityAction);
#endif
    }

    public static void RegisterListener(Button button, UnityAction unityAction)
    {
        if (button == null)
            return;

        RegisterListener(button.onClick, unityAction);
    }

    public static void UnregisterListener(Button button, UnityAction unityAction)
    {
        if (button == null)
            return;

        UnregisterListener(button.onClick, unityAction);
    }
}
