using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour
{
    private TextMeshProUGUI _countText;
    private TextMeshProUGUI _creditsText;
    private TextMeshProUGUI _victoryDefeatText;
    private Text _textBoxTutorial;
    private Text _textBoxCredits;
    private bool _tutBool = true;
    private bool _creditBool;
    private bool _menuBool = true;
    private bool _victoryBool;

    // Start is called before the first frame update
    void Start()
    {
        _textBoxTutorial = GameObject.Find("TutorialButtonText").GetComponent<Text>();
        _textBoxCredits = GameObject.Find("CreditButtonText").GetComponent<Text>();
        _countText = GetComponent<TextMeshProUGUI>();
        _creditsText = GameObject.Find("Credits").GetComponent<TextMeshProUGUI>();
        _victoryDefeatText = GameObject.Find("VictoryDefeat").GetComponent<TextMeshProUGUI>();

        GameEvents.current.OnTutorial += UpdateTutorialBool;
        GameEvents.current.OnCredits += UpdateCreditBool;
        GameEvents.current.OnTogglePause += HandleTogglePause;
        GameEvents.current.OnPlayerKilled += HandleDefeat;
        GameEvents.current.OnVictory += HandleVictory;
    }

    private void Update()
    {
        UpdateTutorial();
    }

    private void HandleTogglePause(bool paused)
    {
        _menuBool = paused;
    }

    void UpdateTutorialBool(bool tutorialBool)
    {
        _tutBool = tutorialBool;
    }

    void UpdateCreditBool(bool tutorialBool)
    {
        _creditBool = tutorialBool;
    }

    private void HandleDefeat()
    {
        if (!_victoryBool)
        {
            _victoryDefeatText.text = "You have been defeated! Press Restart to play again.";
        }
        // Show credits, disable buttons, disable tutorial
    }

    private void HandleVictory(int i)
    {
        _victoryBool = true;
        string rank = i == 1 ? "Commoner" : "VIP";
        _victoryDefeatText.text = $"Victorious! You survive the Thunderdome with the rank of {rank}.";
    }

    void UpdateTutorial()
    {
        if (_tutBool)
        {
            _textBoxTutorial.text = "Hide Tutorial";
        }

        if (!_tutBool)
        {
            _textBoxTutorial.text = "Show Tutorial";
        }

        if (name == "Tutorial")
        {
            if (_tutBool && _menuBool)
            {
                _countText.text = "Controls:\n" +
                                  "Move - WASD / Left Stick\n" +
                                  "Aim Turret - Arrows / Right Stick / Mouse\n" +
                                  "Shoot - L Mouse / Space / R Trigger\n" +
                                  "Reset Camera - Down Arrow / R Mouse\n" +
                                  "Speed Boost - Left Shift / Left Trigger\n" +
                                  "Buy Menu - Use Numbers or D-Pad";
            }
            else
            {
                _countText.text = "";
            }
        }

        if (name == "GameObjectives")
        {
            if (_tutBool && _menuBool)
            {
                _countText.text = "Game Objectives:\n" +
                                  "Survive each round of combat in the Thunderdome and be the last gladiator standing.\n" +
                                  "Defeat enemies and gather any collectibles they leave behind.\n" +
                                  "Survive by protecting health with armor.\n" +
                                  "Obtain money for defeating enemies and surviving rounds.\n" +
                                  "Purchase upgrades or invest money between rounds.\n" +
                                  "Ultimately, a gladiator can buy their freedom from the Thunderdome!";
            }
            else
            {
                _countText.text = "";
            }
        }

        /////////////////////////////////////////////////////////////// CREDITS
        if (_creditBool)
        {
            _textBoxCredits.text = "Hide Credits";
        }

        if (!_creditBool)
        {
            _textBoxCredits.text = "Show Credits";
        }

        if (name == "Credits")
        {
            if (_creditBool && _menuBool)
            {
                _creditsText.text = "Created With Love By Thunderdome Entertainment:\n" +
                                    "Steven Abbott\n" +
                                    "David Baca\n" +
                                    "Daniel Klingman\n" +
                                    "Bryan Olson\n" +
                                    "Ayush Petigara";
            }
            else
            {
                _creditsText.text = "";
            }
        }
    }
}