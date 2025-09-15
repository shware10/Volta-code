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
        Object obj = null, // �α� ����Ŭ�� �� �������� ������Ʈ
        [CallerFilePath] string filePath = "", // ���� �̸�
        [CallerLineNumber] int line = 0, // ���� ��ȣ
        [CallerMemberName] string member = "") // ȣ���� �޼���/������Ƽ �̸�
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
