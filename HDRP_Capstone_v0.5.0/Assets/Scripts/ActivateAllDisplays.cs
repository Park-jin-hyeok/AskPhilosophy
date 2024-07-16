using UnityEngine;

public class ActivateAllDisplays : MonoBehaviour
{
    void Start()
    {
        Debug.Log("displays connected: " + Display.displays.Length);

        for (int i = 1; i < Display.displays.Length; i++)
        {
            //디스플레이 개수에 맞게 active
            Display.displays[i].Activate();
        }
    }

    void Update()
    {

    }
}