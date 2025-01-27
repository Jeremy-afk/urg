using UnityEngine;

enum SingletonPolicy
{
    ReplaceByNew,
    KeepExisting
}

public class PersistantObject : MonoBehaviour
{
    [Header("Persistance")]
    [SerializeField] private bool makePersistant;

    [Header("Singleton Behaviour")]
    [Tooltip("Ignored if make singleton is false")]
    [SerializeField] private SingletonPolicy singletonPolicy;
    [SerializeField] private bool makeSingleton;

    public static PersistantObject Instance { get; private set; }

    private void Awake()
    {
        if (makeSingleton)
        {
            MakeSingleton();
        }

        if (makePersistant)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void MakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            switch (singletonPolicy)
            {
                case SingletonPolicy.ReplaceByNew:
                    Destroy(Instance.gameObject);
                    Instance = this;
                    break;
                case SingletonPolicy.KeepExisting:
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
