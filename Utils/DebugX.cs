using UnityEngine;
using System.Runtime.CompilerServices;
using System.IO;

/// <summary>
/// DebugX.Log("examples..") 
/// DebugX.Log("examples..", this) <= can find obj including this script
/// </summary>
public static class DebugX
{
    public static void Log(
        string message,
        Object obj = null, // 로그 더블클릭 시 가르켜질 오브젝트
        [CallerFilePath] string filePath = "", // 파일 이름
        [CallerLineNumber] int line = 0, // 라인 번호
        [CallerMemberName] string member = "") // 호출한 메서드/프로퍼티 이름
    {
        string fileName = Path.GetFileName(filePath);
        Debug.Log($"[{fileName}:{line}/{member}] \n {message}", obj);
    }
    public static void LogWarning(
    string message,
    Object obj = null,
    [CallerFilePath] string filePath = "", 
    [CallerLineNumber] int line = 0,
    [CallerMemberName] string member = "") 
    {
        string fileName = Path.GetFileName(filePath);
        Debug.LogWarning($"[{fileName}:{line}/{member}] \n {message}", obj);
    }
    public static void LogError(
    string message,
    Object obj = null,
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int line = 0,
    [CallerMemberName] string member = "")
    {
        string fileName = Path.GetFileName(filePath);
        Debug.LogError($"[{fileName}:{line}/{member}] \n {message}", obj);
    }
}
