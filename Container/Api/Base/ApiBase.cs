using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Discovery;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Redbean.Api
{
	public class ApiBase
	{
		private const int retryCount = 3;
		
		protected static async Task<T> GetRequestAsync<T>(string uri, object[] args, CancellationToken cancellationToken = default)
		{
			args ??= new[] { "" };
				
			var format = string.Format(uri, args.Where(_ => _ is string or int).ToArray());
			var response = "";
			
			var index = 0;
			while (index < retryCount)
			{
				if (cancellationToken.IsCancellationRequested)
					break;
				
				response = await GetApi(format, cancellationToken);
				if (!string.IsNullOrEmpty(response))
					break;
					
				index += 1;
			}

			return JsonConvert.DeserializeObject<T>(response);
		}
		
		protected static async Task<T> PostRequestAsync<T>(string uri, object args, CancellationToken cancellationToken = default)
		{
			args ??= new[] { "" };
			
			var content = new StringContent(JsonConvert.SerializeObject(args), Encoding.UTF8, "application/json");
			var response = "";
			
			var index = 0;
			while (index < retryCount)
			{
				if (cancellationToken.IsCancellationRequested)
					break;
				
				response = await PostApi(uri, content, cancellationToken);
				if (!string.IsNullOrEmpty(response))
					break;
					
				index += 1;
			}

			return JsonConvert.DeserializeObject<T>(response);
		}
		
		protected static async Task<T> DeleteRequestAsync<T>(string uri, object[] args, CancellationToken cancellationToken = default)
		{
			args ??= new[] { "" };
			
			var format = string.Format(uri, args.Where(_ => _ is string or int).ToArray());
			var response = "";
			
			var index = 0;
			while (index < retryCount)
			{
				if (cancellationToken.IsCancellationRequested)
					break;
				
				response = await DeleteApi(format, cancellationToken);
				if (!string.IsNullOrEmpty(response))
					break;
					
				index += 1;
			}
			
			return JsonConvert.DeserializeObject<T>(response);
		}
		
		private static async Task<string> GetApi(string uri, CancellationToken cancellationToken = default)
		{
			var stopwatch = Stopwatch.StartNew();
			var httpUri = uri.Split('?')[0].TrimStart('/');
			
			HttpResponseMessage request = null;
			try
			{
				request = await ApiSingleton.Http.GetAsync(uri, cancellationToken);
				if (request.IsSuccessStatusCode)
				{
					var response = await request.Content.ReadAsStringAsync();
					stopwatch.Stop();

					var responseParsing = JObject.Parse(response);
					if (responseParsing.TryGetValue("errorCode", out var errorCodeToken))
					{
						var errorCode = errorCodeToken.Value<int>();
						if (errorCode == 0)
							Log.Success("POST",
							            $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) Request success\n{response}");
						else
							Log.Fail("POST",
							         $"<{httpUri}> ({stopwatch.ElapsedMilliseconds}ms) ErrorCode : {errorCode}");
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
		
		private static async Task<string> PostApi(string uri, HttpContent content = default, CancellationToken cancellationToken = default)
		{
			var stopwatch = Stopwatch.StartNew();
			var httpUri = uri.Split('?')[0].TrimStart('/');
			
			HttpResponseMessage request = null;
			try
			{
				request = await ApiSingleton.Http.PostAsync(uri, content, cancellationToken);
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
		
		private static async Task<string> DeleteApi(string uri, CancellationToken cancellationToken = default)
		{
			var stopwatch = Stopwatch.StartNew();
			var httpUri = uri.Split('?')[0].TrimStart('/');
			
			HttpResponseMessage request = null;
			try
			{
				request = await ApiSingleton.Http.DeleteAsync(uri, cancellationToken);
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