using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SetupScreen : MonoBehaviour
{
    public InputField IDEntryField;
    public void startButton()
    {
        if (int.TryParse(IDEntryField.text, out StateMachineGame.participantID))
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            IDEntryField.image.color = Color.red;
        }
    }
}
