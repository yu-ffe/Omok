using Commons.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    private Action beforeLoadAction;
    private Action afterLoadAction;

    /// <summary>
    /// 특정 씬을 비동기로 전환하며, 로딩 전에 실행할 작업 등록
    /// </summary>
    public void RegisterBeforeLoadAction(string targetSceneName, Action beforeAction)
    {
        beforeLoadAction = beforeAction;
    }
    
    public void RegisterAfterLoadAction(string sceneName, Action afterAction)
    {
        afterLoadAction = afterAction;
    }

    /// <summary>
    /// 씬을 비동기로 전환
    /// </summary>
    public async UniTaskVoid LoadSceneAsync(string sceneName)
    {
        Debug.Log($"[SceneTransitionManager] 씬 전환 요청: {sceneName}");

        beforeLoadAction?.Invoke();
        beforeLoadAction = null;

        await SceneManager.LoadSceneAsync(sceneName);

        Debug.Log($"[SceneTransitionManager] 씬 로드 완료: {sceneName}");

        afterLoadAction?.Invoke(); // ✅ 로딩 완료 후 실행
        afterLoadAction = null;
    }
    /// <summary>
    /// 즉시 씬 전환 (비동기 아님)
    /// </summary>
    public void LoadSceneImmediate(string sceneName)
    {
        beforeLoadAction?.Invoke();
        beforeLoadAction = null;

        SceneManager.LoadScene(sceneName);
    }}
