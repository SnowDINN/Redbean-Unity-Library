using UnityEngine;

namespace Redbean
{
	public class Log
	{
		[HideInCallstack]
		public static void Print(string tag, string message, Color color = default)
		{
			if (color == default)
				color = Color.white;
			
			Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[{tag}] {message}</color>");
		}
		
		[HideInCallstack]
		public static void Print(string message, Color color = default)
		{
			if (color == default)
				color = Color.white;
			
			Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[Log] {message}</color>");
		}
		
		[HideInCallstack]
		public static void System(string message, Color color = default)
		{
			Print("System", message, Color.cyan);
		}
		
		[HideInCallstack]
		public static void Notice(string message, Color color = default)
		{
			Print("Notice", message, Color.yellow);
		}
		
		[HideInCallstack]
		public static void Success(string tag, string message, Color color = default)
		{
			Print(tag, message, Color.green);
		}
		
		[HideInCallstack]
		public static void Fail(string tag, string message, Color color = default)
		{
			Print(tag, message, Color.red);
		}
	}
}