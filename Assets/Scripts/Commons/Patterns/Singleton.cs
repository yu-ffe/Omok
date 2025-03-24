using System;
using UnityEngine;    


public abstract class Singleton<T> :MonoBehaviour where T : Singleton<T> 
{
    private static T instance;
    private static bool isShuttingDown = false;
    private static object lockObject = new object();
        
    /// <summary>
    /// 싱글톤 인스턴스에 접근 (다른 스크립트에서 사용)
    /// </summary>
    public static T Instance
    {
        get {
            if (isShuttingDown)
            {
                return null;
            }

            lock (lockObject)
            {
                if (!instance)
                {
                    //씬에서 찾기 (이미 존재)
                    instance = FindObjectOfType<T>();

                    if (!instance)
                    {
                        // 종료 중이라면 생성하지 않음 (이중 방어)
                        if (isShuttingDown)
                            return null;

                        GameObject singletonObject = new GameObject(typeof(T).Name + " (Singleton)");
                        instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return instance;
            }
        }
    }
    
    /// <summary>
    /// 오버라이드 가능한 초기화 (자식에서 필요 시 사용)
    /// </summary>
    protected virtual void Awake() {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this) {
            return;
            Destroy(gameObject);
        }
    }
    
    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }
    
    private void OnDestroy()
    {
        // 애플리케이션이 강제로 종료되지 않은 경우에도 제거 처리
        isShuttingDown = true;
    }

}
