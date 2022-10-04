using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadingUI : MonoBehaviour
{
    float time = 0.0f;
    public GameObject UI1;
    // Start is called before the first frame update
    void Start()
    {
        float vRate = Screen.height / 1080.0f;
        float hRate = Screen.width / 1920.0f;

        UI1.transform.localScale = Vector3.one * 1.0f * vRate;
        UI1.GetComponent<RectTransform>().anchoredPosition = new Vector2(UI1.GetComponent<RectTransform>().anchoredPosition.x * hRate, UI1.GetComponent<RectTransform>().anchoredPosition.y * vRate);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time > 2.0f)
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}
