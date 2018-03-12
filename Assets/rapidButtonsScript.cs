using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rapidButtons;

public class rapidButtonsScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMNeedyModule needyModule;
    public List<Button> buttons = new List<Button>();
    public List<String> buttonText = new List<string>();
    public List<Color> buttonColors = new List<Color>();
    private Button[] selectedButtons;
    private Button[] correctButtons;
    public KMAudio Audio;
    bool logicTrue = true;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        needyModule = GetComponent<KMNeedyModule>();
        needyModule.OnNeedyActivation += OnNeedyActivation;
        needyModule.OnNeedyDeactivation+= OnNeedyDeactivation;
        needyModule.OnTimerExpired+= OnTimerExpired;
        foreach (Button button in buttons)
        {
            Button trueButton = button;
            trueButton.selectable.OnInteract += delegate () { buttonPress (trueButton); return false; };
        }
    }

    void Start()
    {
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    void buttonLogic()
    {
        Button bottomButton = selectedButtons.MaxBy((x) => -x.transform.localPosition.z);
        Button topButton = selectedButtons.MaxBy((x) => x.transform.localPosition.z);
        Button leftButton = selectedButtons.MaxBy((x) => -x.transform.localPosition.x);
        Button rightButton = selectedButtons.MaxBy((x) => x.transform.localPosition.x);

        if (selectedButtons.Where((x) => x.text.text.Equals(" ")).Count() == 1)
        {
            if (selectedButtons.Any((x) => x.buttonRend.material.color == buttonColors[2]))
            {
                correctButtons = selectedButtons.Where((x) => x.buttonRend.material.color == buttonColors[2]).ToArray();
                Debug.LogFormat("[Rapid Buttons #{0}] You must push a green button.", moduleId);
                return;
            }
            else if (selectedButtons.Any((x) => x.buttonRend.material.color == buttonColors[1]))
            {
                correctButtons = selectedButtons.Where((x) => x.buttonRend.material.color == buttonColors[1]).ToArray();
                Debug.LogFormat("[Rapid Buttons #{0}] You must push a red button.", moduleId);
                return;
            }
            else if (selectedButtons.Any((x) => x.text.text == "O"))
            {
                correctButtons = selectedButtons.Where((x) => x.text.text == "O").ToArray();
                Debug.LogFormat("[Rapid Buttons #{0}] You must push a button containing a 'O label'.", moduleId);
                return;
            }
            else if (bottomButton.buttonRend.material.color == buttonColors[9])
            {
                correctButtons = new Button[] { bottomButton };
                Debug.LogFormat("[Rapid Buttons #{0}] You must push the button nearest the bottom.", moduleId);
                return;
            }
        }
        if (Bomb.GetOnIndicators().Count() > 0)
        {
            if (selectedButtons.Any((x) => x.buttonRend.material.color == buttonColors[0]))
            {
                correctButtons = selectedButtons.Where((x) => x.buttonRend.material.color == buttonColors[0]).ToArray();
                Debug.LogFormat("[Rapid Buttons #{0}] You must push a blue button.", moduleId);
                return;
            }
            else if (selectedButtons.Any((x) => x.buttonRend.material.color == buttonColors[4]))
            {
                correctButtons = selectedButtons.Where((x) => x.buttonRend.material.color == buttonColors[4]).ToArray();
                Debug.LogFormat("[Rapid Buttons #{0}] You must push a yellow button.", moduleId);
                return;
            }
            else if (selectedButtons.Any((x) => x.text.text == "•"))
            {
                correctButtons = selectedButtons.Where((x) => x.text.text == "•").ToArray();
                Debug.LogFormat("[Rapid Buttons #{0}] You must push a button containing a '• label'.", moduleId);
                return;
            }
            else if (topButton.buttonRend.material.color == buttonColors[6])
            {
                correctButtons = new Button[] { topButton };
                Debug.LogFormat("[Rapid Buttons #{0}] You must push the button nearest the top.", moduleId);
                return;
            }
        }
        if (logicTrue == true)
        {
            if (selectedButtons.Any((x) => x.buttonRend.material.color == buttonColors[5]))
            {
                correctButtons = selectedButtons.Where((x) => x.buttonRend.material.color == buttonColors[5]).ToArray();
                Debug.LogFormat("[Rapid Buttons #{0}] You must push an orange button.", moduleId);
                return;
            }
            else if (selectedButtons.Any((x) => x.buttonRend.material.color == buttonColors[3]))
            {
                correctButtons = selectedButtons.Where((x) => x.buttonRend.material.color == buttonColors[3]).ToArray();
                Debug.LogFormat("[Rapid Buttons #{0}] You must push a purple button.", moduleId);
                return;
            }
            else if (selectedButtons.Any((x) => x.text.text == "+"))
            {
                correctButtons = selectedButtons.Where((x) => x.text.text == "+").ToArray();
                Debug.LogFormat("[Rapid Buttons #{0}] You must push a button containing a '+ label'.", moduleId);
                return;
            }
            else if (leftButton.buttonRend.material.color == buttonColors[7])
            {
                correctButtons = new Button[] { leftButton };
                Debug.LogFormat("[Rapid Buttons #{0}] You must push the button nearest the left.", moduleId);
                return;
            }
            else
            {
                correctButtons = new Button[] { rightButton };
                Debug.LogFormat("[Rapid Buttons #{0}] You must push the button nearest the right.", moduleId);
                return;
            }
        }
    }

    void OnNeedyActivation()
    {
        selectedButtons = buttons.RandomPick(3,true).ToArray();
        foreach (Button button in selectedButtons)
        {
            button.gameObject.SetActive(true);
            button.buttonRend.material.color = buttonColors.RandomPick();
            button.text.text = buttonText.RandomPick();
        }
        Debug.LogFormat("[Rapid Buttons #{0}] Your three buttons are {1} with '{2} label', {3} with '{4} label' and {5} with '{6} label'.", moduleId, selectedButtons[0].buttonRend.material.color, selectedButtons[0].text.text.Replace(" ", "no"), selectedButtons[1].buttonRend.material.color, selectedButtons[1].text.text.Replace(" ", "no"), selectedButtons[2].buttonRend.material.color, selectedButtons[2].text.text.Replace(" ", "no"));
        buttonLogic();
    }

    void OnNeedyDeactivation()
    {
        GetComponent<KMNeedyModule>().HandlePass();
        Start();
    }

    void OnTimerExpired()
    {
        GetComponent<KMNeedyModule>().HandleStrike();
        Start();
    }

    public void buttonPress(Button pressedButton)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        pressedButton.selectable.AddInteractionPunch(.5f);
        if (correctButtons.Contains(pressedButton))
        {
            Debug.LogFormat("[Rapid Buttons #{0}] You pushed the correct button.", moduleId);
            OnNeedyDeactivation();
        }
        else
        {
            Debug.LogFormat("[Rapid Buttons #{0}] Strike! You pushed the {1} button with '{2}-label'. That was incorrect.", moduleId, pressedButton.buttonRend.material.color, pressedButton.text.text.Replace(" ", "no"));
            GetComponent<KMNeedyModule>().HandleStrike();
            OnNeedyDeactivation();
        }
    }
}
