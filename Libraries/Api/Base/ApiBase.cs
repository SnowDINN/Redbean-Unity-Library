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
		protected static async Task<T> SendGetRequest<T>(string uri, params object[] args)
		{
			var format = string.Format(uri, args.Where(_ => _ is string or int).ToArray());
			var apiResponse = await GetApi(format);

			return JsonConvert.DeserializeObject<T>(apiResponse);
		}
		
		protected static async Task<T> SendPostRequest<T>(string uri, object obj)
		{
			var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
			var apiResponse = await PostApi(uri, content);

			return JsonConvert.DeserializeObject<T>(apiResponse);
		}
		
		protected static async Task<T> SendDeleteRequest<T>(string uri, params object[] args)
		{
			var format = string.Format(uri, args.Where(_ => _ is string or int).ToArray());
			var apiResponse = await DeleteApi(format);
			
			return JsonConvert.DeserializeObject<T>(apiResponse);
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

				throw;
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