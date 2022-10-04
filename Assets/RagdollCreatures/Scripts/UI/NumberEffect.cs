using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NumberEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public int num = 30;
    public Text num_txt;
    public float time = 0.0f;
    float audioTime = 0.0f;
    public float step = 0.07f;
    public int mark = 1;
    public string nextString = "";
    public bool sounded = false;
    void Start()
    {
        num_txt = GetComponent<Text>();
    }
    private void OnEnable()
    {
        time = 0.0f;
        sounded = false;
    }

    void PlaySound()
    {
        if(mark < num)
        {
            transform.GetComponent<AudioSource>().Play();
            sounded = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * 3;
        audioTime += Time.deltaTime*1.5f;

        if (time < step)
        {

        }
        else if (mark < num)
        {
            mark++;
            if(sounded == false)
                PlaySound();
            time = 0.0f;
        }

        if (audioTime < step)
        {

        }
        else if(mark<num)
        {
            audioTime = 0.0f;
        }
        num_txt.text = mark + nextString;
    }
}
