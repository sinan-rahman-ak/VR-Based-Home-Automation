using UnityEngine;
using NativeWebSocket; // WebSocket library

public class LightToggle : MonoBehaviour
{
    public Light spotLight;  // Reference to Spot Light
    public Light pointLight; // Reference to Point Light
    private bool isLightOn = false;  // Start with lights OFF

    // WebSocket variables
    private WebSocket websocket;
    private string websocketUrl = "ws://192.168.1.6:80"; // Replace X.Y with ESP8266 IP

    void Start()
    {
        Debug.Log("Initializing LightToggle script...");

        // Ensure both lights are off initially
        if (spotLight != null)
        {
            spotLight.enabled = isLightOn;
        }
        else
        {
            Debug.LogError("Spot Light is not assigned!");
        }

        if (pointLight != null)
        {
            pointLight.enabled = isLightOn;
        }
        else
        {
            Debug.LogError("Point Light is not assigned!");
        }

        // Initialize WebSocket connection
        InitializeWebSocket();
    }

    private async void InitializeWebSocket()
    {
        Debug.Log("Attempting to connect to WebSocket...");
        websocket = new WebSocket(websocketUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("✅ WebSocket Connection Opened Successfully!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log($"📩 Message received from ESP8266: {message}");
        };

        websocket.OnClose += (e) =>
        {
            Debug.LogWarning("⚠️ WebSocket Connection Closed!");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError($"❌ WebSocket Error: {e}");
        };

        try
        {
            await websocket.Connect();
            Debug.Log("✅ WebSocket Connected Successfully!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ WebSocket Connection Failed: {ex.Message}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("🖐️ Player Hand detected! Toggling lights...");
            ToggleLights();
        }
    }

    private void ToggleLights()
    {
        isLightOn = !isLightOn;

        if (spotLight != null)
        {
            spotLight.enabled = isLightOn;
        }

        if (pointLight != null)
        {
            pointLight.enabled = isLightOn;
        }

        Debug.Log($"💡 Lights turned {(isLightOn ? "ON" : "OFF")}");
        SendWebSocketMessage(isLightOn ? "on" : "off");
    }

    private async void SendWebSocketMessage(string message)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(message);
            Debug.Log($"📤 Sent Message: '{message}' to WebSocket Server");
        }
        else
        {
            Debug.LogError("❌ WebSocket is not connected! Message not sent.");
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            Debug.Log("Closing WebSocket connection...");
            await websocket.Close();
        }
    }
}
