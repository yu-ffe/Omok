using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace workspace.Ham6._03_Sctipts
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;
        private static bool isShuttingDown = false;
        private static object lockObject = new object();

        /// <summary>
        /// 싱글톤 인스턴스에 접근 (다른 스크립트에서 사용)
        /// </summary>
        public static T Instance
        {
            get
            {
                if (isShuttingDown)
                {
                    return null;
                }

                lock (lockObject)
                {
                    if (instance == null)
                    {
                        //씬에서 찾기 (이미 존재)
                        instance = FindObjectOfType<T>();

                        if (instance == null)
                        {
                            //없으면 새로 생성
                            GameObject singletonObject = new GameObject();
                            instance = singletonObject.AddComponent<T>();

                            //씬 전환시에도 유지
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
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else if (instance != this)
            {
                //중복 방지
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 앱 종료시 해제
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            isShuttingDown = true;
        }

        /// <summary>
        /// 수동 해제 시
        /// </summary>
        protected virtual void OnDestroy()
        {
            isShuttingDown = true;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        protected abstract  void OnSceneLoaded(Scene scene, LoadSceneMode mode);
    }
}