using TMPro;
// UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialPad : MonoBehaviour {
    public TextMeshProUGUI displayText; // TextMeshProUGUI 오브젝트
    private GameObject currentClickedObject = null;
    private Vector3 originalPosition;

    public GameObject Client;

    int pressedNum = 0;
    void Start() {
        displayText.text = ""; // 게임 시작 시 텍스트를 초기화합니다.
    }

    void Update()
    {
        //마우스 클릭뿐만 아니라 터치값도 받아오기

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
                currentClickedObject = hit.collider.gameObject;
                originalPosition = currentClickedObject.transform.position;
                MoveObjectZ(currentClickedObject, 0.02f);
            }
        }

        if (Input.GetMouseButtonUp(0) && currentClickedObject != null) {
            MoveObjectZ(currentClickedObject, -0.02f);
            if (displayText.text == "ERROR") {
                if (currentClickedObject.name == "0") {
                    displayText.text = "";
                }
            }
            else {
                AddValueToText(currentClickedObject.name);
            }
            currentClickedObject = null;
        }
    }

    void MoveObjectZ(GameObject obj, float zChange) {
        Vector3 newPosition = obj.transform.position;
        newPosition.z += zChange;
        obj.transform.position = newPosition;
    }

    void AddValueToText(string value) {
        if (displayText.text.Length >= 4) {
            if (displayText.text == "1509") {
                displayText.text += value;
                if (displayText.text != "1509#") {
                    displayText.text = "ERROR";
                }
                else  {
                    Client.GetComponent<Client>().SendMessageToServer("0");
                    displayText.text = "";
                    //SceneManager.LoadScene(0);
                }
            }
            else if (displayText.text == "3456") {
                displayText.text += value;
                if (displayText.text != "3456#") {
                    displayText.text = "ERROR";
                }
                else {
                    Client.GetComponent<Client>().SendMessageToServer("1");
                    if (pressedNum == 1) {
                        SceneManager.LoadScene(0);
                    }
                    displayText.text = "";
                    pressedNum++;
                }
            }
            else if (displayText.text == "5656") {
                displayText.text += value;
                if (displayText.text != "5656#") {
                    displayText.text = "ERROR";
                }
                else {
                    Client.GetComponent<Client>().SendMessage("2");
                    SceneManager.LoadScene(0);
                }
            }
            else {
                displayText.text = "ERROR";
            }
        }
        else {
            displayText.text += value;
            Debug.Log(value);
        }
    }
}
