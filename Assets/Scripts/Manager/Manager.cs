using UnityEngine;

public class Manager<T> : MonoBehaviour
    where T : Manager<T>
{
    public static T Instance;

    protected virtual void Awake()
    {
        Instance = (T)this;
    }
}