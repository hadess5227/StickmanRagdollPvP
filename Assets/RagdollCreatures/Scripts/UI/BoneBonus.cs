using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoneBonus : MonoBehaviour
{
    List<AudioClip> auds = new List<AudioClip>();
    public Text boneTxt;
    public int mark = 0;
    float time = 0.0f;
    public float step = 0.07f;
    public float targetNum = 5;
    // Start is called before the first frame update
    void Start()
    {
        auds = new List<AudioClip>(Resources.LoadAll<AudioClip>("Music/BoneBreak"));
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time < step)
        {

        }
        else if(mark < targetNum)
        {
            mark++;
            time = 0.0f;

        }
        boneTxt.text = "+" + mark;
    }

    private void OnEnable()
    {
        int index = Random.RandomRange(0, auds.Count);
        mark = 0;
        time = 0.0f;

        if(auds.Count == 0)
        {
            auds = new List<AudioClip>(Resources.LoadAll<AudioClip>("Music/BoneBreak"));
        }
        boneTxt.GetComponent<AudioSource>().Play();
        transform.GetComponent<AudioSource>().clip = auds[index];
        transform.GetComponent<AudioSource>().Play();
        Invoke("disappear", 0.2f * targetNum);
    }

    void disappear()
    {
        gameObject.SetActive(false);
    }
}
