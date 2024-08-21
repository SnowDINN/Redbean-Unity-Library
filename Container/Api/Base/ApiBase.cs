using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Redbean.Api
{
	public class ApiBase
	{
		private const int retryCount = 3;
		
		protected static async Task<T> SendGetRequest<T>(string uri, params object[] args)
		{
			var format = string.Format(uri, args.Where(_ => _ is string or int).ToArray());
			var response = "";
			
			var index = 0;
			while (index < retryCount)
			{
				response = await GetApi(format);
				if (!string.IsNullOrEmpty(response))
					break;
					
				index += 1;
			}

			return JsonConvert.DeserializeObject<T>(response);
		}
		
		protected static async Task<T> SendPostRequest<T>(string uri, object obj)
		{
			var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
			var response = "";
			
			var index = 0;
			while (index < retryCount)
			{
				response = await PostApi(uri, content);
				if (!string.IsNullOrEmpty(response))
					break;
					
				index += 1;
			}

			return JsonConvert.DeserializeObject<T>(response);
		}
		
		protected static async Task<T> SendDeleteRequest<T>(string uri, params object[] args)
		{
			var format = string.Format(uri, args.Where(_ => _ is string or int).ToArray());
			var response = "";
			
			var index = 0;
			while (index < retryCount)
			{
				response = await DeleteApi(format);
				if (!string.IsNullOrEmpty(response))
					break;
					
				index += 1;
			}
			
			return JsonConvert.DeserializeObject<T>(response);
		}
		
		private static async Task<string> GetApi(string uri)
		{
			var stopwatch = Stopwatch.StartNew();
			var httpUri = uri.Split('?')[0].TrimStart('/');
			
			HttpResponseMessage request = null;
			try
			{
				request = await ApiContainer.Http.GetAsync(uri);
				if (request.IsSuccessStatusCode)
				{
					var response = await request.Content.ReadAsStringAsync();
					stopwatch.Stop();

					var responseParsing = JObject.Parse(response);
					if (responseParsing.TryGetValue("errorCode", out var errorCodeToken))
					{
						var errorCode = errorCodeToken.Value<int>();
						if (errorCode == 0)
							Log.Success("POST", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) Request success\n{response}");
						else
							Log.Fail("POST", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) ErrorCode : {errorCode}");
					}
					
					request.Dispose();
					return response;
				}
			}
			catch (HttpRequestException e)
			{
				Log.Fail("GET", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) {e.Message}");
				request?.Dispose();

				return string.Empty;
			}
			finally
			{
				stopwatch.Stop();
			}
			
			Log.Fail("GET", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) ({(int)request.StatusCode}) {request.ReasonPhrase}");
			request.Dispose();
			
			return string.Empty;
		}
		
		private static async Task<string> PostApi(string uri, HttpContent content = null)
		{
			var stopwatch = Stopwatch.StartNew();
			var httpUri = uri.Split('?')[0].TrimStart('/');
			
			HttpResponseMessage request = null;
			try
			{
				request = await ApiContainer.Http.PostAsync(uri, content);
				if (request.IsSuccessStatusCode)
				{
					var response = await request.Content.ReadAsStringAsync();
					stopwatch.Stop();

					var responseParsing = JObject.Parse(response);
					if (responseParsing.TryGetValue("errorCode", out var errorCodeToken))
					{
						var errorCode = errorCodeToken.Value<int>();
						if (errorCode == 0)
							Log.Success("POST", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) Request success\n{response}");
						else
							Log.Fail("POST", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) ErrorCode : {errorCode}");
					}
					
					request.Dispose();
					return response;
				}
			}
			catch (HttpRequestException e)
			{
				Log.Fail("POST", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) {e.Message}");
				request?.Dispose();
				
				throw;
			}
			finally
			{
				stopwatch.Stop();
			}

			Log.Fail("POST", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) ({(int)request.StatusCode}) {request.ReasonPhrase}");
			request.Dispose();
			
			return string.Empty;
		}
		
		private static async Task<string> DeleteApi(string uri)
		{
			var stopwatch = Stopwatch.StartNew();
			var httpUri = uri.Split('?')[0].TrimStart('/');
			
			HttpResponseMessage request = null;
			try
			{
				request = await ApiContainer.Http.DeleteAsync(uri);
				if (request.IsSuccessStatusCode)
				{
					var response = await request.Content.ReadAsStringAsync();
					stopwatch.Stop();

					var responseParsing = JObject.Parse(response);
					if (responseParsing.TryGetValue("errorCode", out var errorCodeToken))
					{
						var errorCode = errorCodeToken.Value<int>();
						if (errorCode == 0)
							Log.Success("POST", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) Request success\n{response}");
						else
							Log.Fail("POST", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) ErrorCode : {errorCode}");
					}
					
					request.Dispose();
					return response;
				}
			}
			catch (HttpRequestException e)
			{
				Log.Fail("DELETE", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) {e.Message}");
				request?.Dispose();

				throw;
			}
			finally
			{
				stopwatch.Stop();
			}
			
			Log.Fail("DELETE", $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) ({(int)request.StatusCode}) {request.ReasonPhrase}");
			request.Dispose();
			
			return string.Empty;
		}
	}
}