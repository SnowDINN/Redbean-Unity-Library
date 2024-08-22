using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Redbean.Api
{
	public enum ApiType
	{
		Get,
		Post,
		Delete,
	}
	
	public class ApiGenerator
	{
		public const string Namespace = "Redbean";
		
		public static async Task GetApiAsync(Type wrapper)
		{
			var uri = $"{AppSettings.ApiUri}/swagger/v1/swagger.json";

			var request = UnityWebRequest.Get(uri);
			await request.SendWebRequest();
			
			if (!string.IsNullOrEmpty(request.error))
			{
				Debug.LogError(request.error);
				return;
			}

			DeleteFiles($"{ApiSettings.ProtocolPath}");
			
			var api = JObject.Parse(request.downloadHandler.text);
			var apiEndpoints = api["paths"].ToObject<Dictionary<string, JObject>>();

			GenerateCSharpApiAsync(ApiType.Get, wrapper, apiEndpoints.Where(_ => _.Value.ContainsKey("get")).ToArray());
			GenerateCSharpApiAsync(ApiType.Post, wrapper, apiEndpoints.Where(_ => _.Value.ContainsKey("post")).ToArray());
		}

		
		/// <summary>
		/// API C# 스크립트 생성
		/// </summary>
		private static async void GenerateCSharpApiAsync(ApiType type, Type wrapper, IReadOnlyList<KeyValuePair<string, JObject>> apis)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("using System.Threading;");
			stringBuilder.AppendLine("using System.Threading.Tasks;");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine($"namespace {Namespace}.Api");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine($"\tpublic class Api{type}Request : {nameof(ApiBase)}");
			stringBuilder.AppendLine("\t{");

			var runtimeApis = apis.Where(_ => !_.Key.StartsWith("/Edit")).ToArray();
			var editApis = apis.Where(_ => _.Key.StartsWith("/Edit")).ToArray();
			
			stringBuilder.Append(GenerateCSharpMethod(type, wrapper, runtimeApis));
			stringBuilder.AppendLine();
			
			stringBuilder.AppendLine("#if UNITY_EDITOR");
			stringBuilder.Append(GenerateCSharpMethod(type, wrapper, editApis));
			stringBuilder.AppendLine("#endif");
			
			stringBuilder.AppendLine("\t}");
			stringBuilder.AppendLine("}");
			
			if (Directory.Exists(ApiSettings.ProtocolPath))
				Directory.CreateDirectory(ApiSettings.ProtocolPath);
				
			await File.WriteAllTextAsync($"{ApiSettings.ProtocolPath}/Api{type}Request.cs", $"{stringBuilder}");
		}

		private static string GenerateCSharpMethod(ApiType type, Type wrapper, IReadOnlyList<KeyValuePair<string, JObject>> apis)
		{
			var stringBuilder = new StringBuilder();
			
			for (var idx = 0; idx < apis.Count; idx++)
			{
				var parameter = "";
            				
				var jObject = apis[idx].Value[$"{type}".ToLower()].ToObject<JObject>();
				if (jObject.TryGetValue("parameters", out var parameters))
				{
					parameter = "?";
            					
					var parameterList = parameters.ToObject<List<Dictionary<string, object>>>();
					for (var i = 0; i < parameterList.Count; i++)
					{
						parameter += $"{parameterList[i]["name"]}={{{i}}}";
            
						if (i < parameterList.Count - 1)
							parameter += "&";
					}
				}
            				
				// Request Body 존재할 경우
				var requestType = "object[] args = default";
				if (jObject.TryGetValue("requestBody", out var requests))
				{
					requestType = requests.SelectToken("content.application/json.schema.$ref")
						.Value<string>()
						.Split('/')
						.Last();
            
					requestType += " args";
				}
            
				// Response Type 존재할 경우
				var responseType = "";
				if (jObject.TryGetValue("responses", out var responses))
				{
					var content = responses.SelectToken("200").ToObject<JObject>();
					if (content.TryGetValue("content", out var json))
					{
						responseType = json.SelectToken("application/json.schema.$ref")
							.Value<string>()
							.Split('/')
							.Last();
            
						responseType = $"<{responseType}>";
					}
				}
            			
				var requestUri = $"\"{apis[idx].Key}{parameter}\"";
				stringBuilder.AppendLine
					($"\t\tpublic static async Task<{wrapper.Name}{responseType}> {apis[idx].Key.Split('/').Last()}Request({requestType}, {nameof(CancellationToken)} cancellationToken = default) =>");
				stringBuilder.AppendLine
					($"\t\t\tawait {type}RequestAsync<{wrapper.Name}{responseType}>({requestUri}, args, cancellationToken);");
            				
				if (idx < apis.Count - 1)
					stringBuilder.AppendLine();
			}
			
			return $"{stringBuilder}";
		}

		private static void DeleteFiles(string path)
		{
			var directory = new DirectoryInfo(path);
			foreach (var file in directory.GetFiles())
				file.Delete();
		}
	}
}