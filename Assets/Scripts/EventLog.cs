using UnityEngine;
using UnityEngine.UI;

public class EventLog : MonoBehaviour
{
    static private GameObject playerPoint;
    static private RectTransform point;
    static private Text text;
    static private RawImage rawImage;

    static private Vector3 lastTextPosition;
    static private Vector3 playerPosition;

    private void Awake()
    {
        point = transform.GetChild(0).GetComponent<RectTransform>();
        text = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        playerPoint = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (text != null && text.gameObject.activeInHierarchy)
        {
            lastTextPosition = Camera.main.WorldToScreenPoint(playerPosition);
            point.transform.position = lastTextPosition;
            text.rectTransform.position += new Vector3(0, 0.1f);
            text.color = Color.Lerp(text.color, new Color(text.color.r,text.color.g,text.color.b, 0), 1 * Time.deltaTime);

            if(text.color.a <= 0.01f)
            {
                text.gameObject.SetActive(false);
            }
        }
    }

    public static void Print(string value, Color32 textColor)
    {
        text.gameObject.SetActive(false);
        text.gameObject.SetActive(true);
        playerPosition = playerPoint.transform.position;
        text.rectTransform.anchoredPosition = new Vector3(0, 0);

        text.color = textColor;

        text.text = value;
    }
}