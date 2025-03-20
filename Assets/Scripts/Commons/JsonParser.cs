using Newtonsoft.Json;
using UnityEngine;

public class JsonParser
{
    /// <summary>
    /// 주어진 JSON 문자열을 지정된 타입으로 역직렬화합니다.
    /// </summary>
    /// <typeparam name="T">역직렬화할 타입</typeparam>
    /// <param name="json">JSON 문자열</param>
    /// <returns>타입 T의 객체, 파싱 실패 시 기본값</returns>
    public static T Deserialize<T>(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON 역직렬화 오류: {ex.Message}\nJSON: {json}");
            return default;
        }
    }

    /// <summary>
    /// 주어진 JSON 문자열을 특정 타입으로 역직렬화합니다.
    /// </summary>
    /// <param name="json">JSON 문자열</param>
    /// <param name="type">역직렬화할 타입</param>
    /// <returns>객체, 파싱 실패 시 null</returns>
    public static object Deserialize(string json, System.Type type)
    {
        try
        {
            return JsonConvert.DeserializeObject(json, type);
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON 역직렬화 오류: {ex.Message}\nJSON: {json}");
            return null;
        }
    }

    /// <summary>
    /// 객체를 JSON 문자열로 직렬화합니다.
    /// </summary>
    /// <param name="obj">직렬화할 객체</param>
    /// <returns>JSON 문자열, 직렬화 실패 시 빈 문자열</returns>
    public static string Serialize(object obj)
    {
        try
        {
            return JsonConvert.SerializeObject(obj);
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON 직렬화 오류: {ex.Message}\n객체: {obj}");
            return string.Empty;
        }
    }
}
        
