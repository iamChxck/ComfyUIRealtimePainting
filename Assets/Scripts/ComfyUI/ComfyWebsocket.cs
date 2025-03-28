using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ComfyWebsocket : MonoBehaviour
{
    private string serverAddress = "127.0.0.1:8188";
    private string clientId = Guid.NewGuid().ToString();
    private ClientWebSocket ws;
    private CancellationTokenSource cancellationTokenSource;

    GeneratedImageLoader generatedImageLoader;

    public string promptID;

    async void Start()
    {
        generatedImageLoader = GetComponent<GeneratedImageLoader>();
        ws = new ClientWebSocket();
        cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await ws.ConnectAsync(new Uri($"ws://{serverAddress}/ws?clientId={clientId}"), cancellationTokenSource.Token);
            Debug.Log("WebSocket connected.");
            StartListening();
        }
        catch (Exception e)
        {
            Debug.LogError("WebSocket connection failed: " + e.Message);
        }
    }

    private async void StartListening()
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (ws.State == WebSocketState.Open)
            {
                var stringBuilder = new StringBuilder();
                WebSocketReceiveResult result;

                do
                {
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationTokenSource.Token);
                        Debug.Log("WebSocket closed by server.");
                        return;
                    }
                    stringBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                while (!result.EndOfMessage);

                string response = stringBuilder.ToString();
                Debug.Log("Received: " + response);

                if (response.Contains("\"queue_remaining\": 0"))
                {
                    generatedImageLoader.LoadLatestImage(promptID);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("WebSocket error: " + e.Message);
        }
    }

    private async void OnDestroy()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            try
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationTokenSource.Token);
                Debug.Log("WebSocket closed.");
            }
            catch (Exception e)
            {
                Debug.LogError("Error closing WebSocket: " + e.Message);
            }
        }
        cancellationTokenSource?.Cancel();
    }
}