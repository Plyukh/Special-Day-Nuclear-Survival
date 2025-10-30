using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    public enum KeyboardType
    {
        Name
    }

    private TouchScreenKeyboard keyboard;

    public KeyboardType keyboardType;
    public Text keyboardText;

    private void Update()
    {
        if (keyboard != null)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Visible)
            {
                keyboardText.text = keyboard.text;
            }

            if(keyboard.status == TouchScreenKeyboard.Status.Done)
            {
                GameObject.FindWithTag("Player").GetComponent<Character>().characterName = keyboardText.text;
            }
        }
    }

    public void OpenKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open(keyboardText.text, TouchScreenKeyboardType.Default, false,false,false, true);
    }

    public void SetPlayerName()
    {
        GameObject.FindWithTag("Player").GetComponent<Character>().characterName = keyboardText.text;
    }
}
