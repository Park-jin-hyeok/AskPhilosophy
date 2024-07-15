using UnityEngine;
using System.Collections;

public class DisplayScript : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        if (Display.displays.Length > 1)
        {
            foreach (var item in Display.displays)
            {
                item.Activate();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}