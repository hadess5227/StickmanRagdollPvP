using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class UIFloatingName : MonoBehaviour
{
    public GameObject arrowObj;
    public GameObject myPlayer;
    public Text playerName;
    public Canvas uiCanvas;
    public float height;
    public float zoomScale;
    // Start is called before the first frame update
    void Start()
    {
        uiCanvas = GameObject.Find("UI").GetComponent<Canvas>();
        RagdollCreature ragdollCreature = myPlayer.transform.root.GetComponent<RagdollCreature>();

        if (RoomManager.Instance && ragdollCreature.aiCont == false)
        {
            if (myPlayer && myPlayer.transform.root.GetComponent<PhotonView>())
            {
                string name = myPlayer.transform.root.GetComponent<PhotonView>().Owner.NickName;
                arrowObj.SetActive(myPlayer.transform.root.GetComponent<PhotonView>().IsMine);
                SetName(name);
                UpdatePosition();
            }
        }
        else if(RoomManager.Instance == null)
        {
            if (ragdollCreature && ragdollCreature.centerObj && ragdollCreature.aiCont)
            {
                string name = ragdollCreature.centerObj.GetComponent<AIController>().AIName;
                SetName(name);
            }
            else
            {
                string name = PlayerPrefs.GetString("PlayerName");
                SetName(name);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myPlayer == null)
        {
            Destroy(gameObject);
        }
        else
            UpdatePosition();
    }

    void SetName(string name)
    {
        playerName.text = name.ToString();
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
