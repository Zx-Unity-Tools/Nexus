using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zx
{
    public class Nexus
    {
        #region Calling Code

        public static async Task<TResult> Post<TData, TResult>(string url, TData data, int delay = 0)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL cannot be null or empty.", nameof(url));
            }
            Debug.Log("API Request: " + url);
            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            string jsonContent = JsonConvert.SerializeObject(data);
            StringContent httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                await Task.Delay(delay * 1000);
                return await PostRequestServer<TResult>(url, client, httpContent);
            }
            catch (HttpRequestException ex)
            {
                Debug.LogError($"API request failed: {ex.Message}");
                throw;
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Task canceled.");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error: {ex.Message}");
                throw;
            }
            finally
            {
                httpContent.Dispose();
                client.Dispose();
            }
        }

        public static async Task<TResult> Get<TResult>(string url, int delay = 0)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL cannot be null or empty.", nameof(url));
            }
            Debug.Log("API Request: " + url);
            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            try
            {
                await Task.Delay(delay * 1000);
                return await GetRequestServer<TResult>(url, client);
            }
            catch (HttpRequestException ex)
            {
                Debug.LogError($"API request failed: {ex.Message}");
                throw;
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Task canceled.");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error: {ex.Message}");
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }

  #endregion
        #region Private Methods

        private static async Task<TResult> PostRequestServer<TResult>(string url, HttpClient client, StringContent httpContent)
        {
            using HttpResponseMessage response = await client.PostAsync(url, httpContent);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed with status code: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(responseBody);
        }
        private static async Task<TResult> GetRequestServer<TResult>(string url, HttpClient client)
        {
            using HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed with status code: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(responseBody);
        }

  #endregion
    }
}
