using UnityEngine;
using NativeWebSocket; // For WebSocket communication

public class FanController : MonoBehaviour
{
    public GameObject fanBlades;  // Reference to the fan blades GameObject
    public float rotationSpeed = 300f;  // Speed of rotation
    private bool isSpinning = false;  // To track if the fan is spinning

    // WebSocket variables
    private WebSocket websocket;
    private string websocketUrl = "ws://192.168.1.6:80"; // Replace with your NodeMCU's WebSocket URL

    async void Start()
    {
        // Ensure the fan blades object is assigned
        if (fanBlades == null)
        {
            Debug.LogError("[FanController] Fan blades are not assigned!");
        }

        // Initialize WebSocket connection
        await InitializeWebSocket();
    }

    void Update()
    {
        // Rotate the fan blades if they are spinning
        if (isSpinning && fanBlades != null)
        {
            fanBlades.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        }
    }

    private async System.Threading.Tasks.Task InitializeWebSocket()
    {
        websocket = new WebSocket(websocketUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("[WebSocket] Connection Opened");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log($"[WebSocket] Message received from server: {message}");
        };

        websocket.OnClose += (e) =>
        {
            Debug.LogWarning("[WebSocket] Connection Closed");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError($"[WebSocket] Error: {e}");
        };

        try
        {
            Debug.Log("[WebSocket] Attempting to connect...");
            await websocket.Connect();
            Debug.Log("[WebSocket] Connected successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[WebSocket] Connection failed: {ex.Message}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the interacting object is tagged as "PlayerHand"
        if (other.CompareTag("PlayerHand"))
        {
            ToggleFan();
        }
    }

    private void ToggleFan()
    {
        // Toggle the spinning state
        isSpinning = !isSpinning;

        // Log the fan's state for debugging
        Debug.Log(isSpinning ? "[FanController] Fan turned ON" : "[FanController] Fan turned OFF");

        // Send WebSocket message to control the NodeMCU
        SendWebSocketMessage(isSpinning ? "motor_on" : "motor_off");
    }

    private async void SendWebSocketMessage(string message)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(message);
            Debug.Log($"[WebSocket] Message Sent: '{message}' to WebSocket Server");
        }
        else
        {
            Debug.LogError("[WebSocket] Client is not connected. Message not sent.");
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            Debug.Log("[WebSocket] Closing connection...");
            await websocket.Close();
            Debug.Log("[WebSocket] Connection closed.");
        }
    }
}
