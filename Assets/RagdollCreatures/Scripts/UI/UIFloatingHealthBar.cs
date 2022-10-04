using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIFloatingHealthBar : MonoBehaviour
{
    public GameObject myPlayer;
    public Canvas uiCanvas;
    public float height;
    public float zoomScale;
    // Start is called before the first frame update
    void Start()
    {
        uiCanvas = GameObject.Find("UI").GetComponent<Canvas>();

        string name = PlayerPrefs.GetString("PlayerName");
        
        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (myPlayer == null)
            Destroy(gameObject);
        else
            UpdatePosition();
    }


    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = currentHealth / maxHealth;
    }

    Vector2 WorldToCanvas(Canvas canvas, Vector3 world_position, Camera camera, out bool left, out bool right, out bool top, out bool bottom)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        left = right = top = bottom = false;
        Vector3 viewport_position = camera.WorldToViewportPoint(world_position);

        /*
        if(viewport_position.x < 0.1f)
        {
            viewport_position.x = 0.1f;
        }
        else if(viewport_position.x > 0.9f)
        {
            viewport_position.x = 0.9f;
        }

        if(viewport_position.y < 0.2f)
        {
            viewport_position.y = 0.2f;
        }
        else if(viewport_position.y > 0.8f)
        {
            viewport_position.y = 0.8f;
        }
        */
        var canvas_rect = canvas.GetComponent<RectTransform>();

        return new Vector2((viewport_position.x * canvas_rect.sizeDelta.x) - (canvas_rect.sizeDelta.x * 0.5f),
                           (viewport_position.y * canvas_rect.sizeDelta.y) - (canvas_rect.sizeDelta.y * 0.5f));
    }

    void UpdatePosition()
    {
        Vector3 aRot = Vector3.zero;
        bool left, right, top, bottom;
        float screenX = Screen.width;
        float screenY = Screen.height;
        float _height = height * zoomScale;
        Vector2 newPos = WorldToCanvas(uiCanvas, myPlayer.transform.position + new Vector3(0.0f, _height, 0.0f), Camera.main, out left, out right, out bottom, out top);

        gameObject.GetComponent<RectTransform>().anchoredPosition = newPos;
    }
}
