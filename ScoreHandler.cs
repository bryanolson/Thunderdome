using System;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ScoreHandler : MonoBehaviour
    {
        private TextMeshProUGUI _countText;
        private bool buyVisible;
        private bool tutBool = false;
        private bool vicBool;

        void Start()
        {
            _countText = GetComponent<TextMeshProUGUI>();
            GameEvents.current.OnUpdateUI += UpdateScore;
            GameEvents.current.OnInvestment += HideBuy;
            GameEvents.current.OnBuyRound += ShowBuy;
            GameEvents.current.OnLevelVictory += HandleVictory;
        }

        private void HandleVictory()
        {
            // Debug.Log("Handle Victory");
            vicBool = true;
        }

        private void ShowBuy(bool inBuyRound)
        {
            vicBool = false;
            buyVisible = inBuyRound;
        }

        private void HideBuy(int unused)
        {
            ShowBuy(false);
        }

        private void Update()
        {
            UpdateScore();
        }

        void UpdateScore()
        {
            if (name == "LevelVictory")
            {
                if (vicBool)
                {
                    _countText.text = "Level Complete! Moving To Upgrade Center";
                }
                else
                {
                    _countText.text = "";
                }
            }

            if (name == "Score")
            {
                // TODO: Update symbols/text/GameState attribute
                _countText.text = $"Level: {GameState.current.CurrentLevel}\n" +
                                  $"Killed: {GameState.current.EnemiesKilled} of {GameState.current.LevelEnemies}.";
            }

            if (name == "HealthArmor")
            {
                // TODO: Update symbols/text/GameState attribute
                // Unicode heart: {"\u2665".ToString()}
                _countText.text =
                    $"Health: {GameState.current.CurrentHealth} | Armor: {GameState.current.CurrentArmor}";
            }

            if (name == "AmmoBoost")
            {
                // TODO: Update symbols/text/GameState attribute
                _countText.text = $"Ammo: {GameState.current.CurrentAmmo}";
            }

            if (name == "BuyingMenu")
            {
                if (buyVisible)
                {
                    // TODO: Update symbols/text/GameState attribute
                    _countText.text = "Upgrade Your Vehicle:</style>\n\n" +
                                      $"{StyleStart(1)}1) Increase Armor:\n\tLevel:{BuyRoundLogic.current.upgrades["armor"][0]} ${BuyRoundLogic.current.upgrades["armor"][2]} (+{BuyRoundLogic.current.upgrades["armor"][1]} Armor){StyleEnd(1)}\n\n" +
                                      $"{StyleStart(2)}2) Increase Projectile Speed:\n\tLevel:{BuyRoundLogic.current.upgrades["attackspeed"][0]} ${BuyRoundLogic.current.upgrades["attackspeed"][2]} (+{BuyRoundLogic.current.upgrades["attackspeed"][1]} Attack Speed){StyleEnd(2)}\n\n" +
                                      $"{StyleStart(3)}3) Increase Damage:\n\tLevel:{BuyRoundLogic.current.upgrades["damage"][0]} ${BuyRoundLogic.current.upgrades["damage"][2]} (+{BuyRoundLogic.current.upgrades["damage"][1]} Damage){StyleEnd(3)}\n\n";
                }
                else
                {
                    _countText.text = "";
                }
            }

            if (name == "InvestFreedomMenu")
            {
                if (buyVisible)
                {
                    // TODO: Update symbols/text/GameState attribute
                    _countText.text = "Invest Funds (This Will End Buying Phase):\n\n" +
                                      $"{StyleStart(4)}4) Invest Remaining Funds (10% ROI):\n\tInvest ${GameState.current.CurrentGold}, Receive ${(int) (GameState.current.CurrentGold * 1.1)}{StyleEnd(4)}\n\n\n\n" +
                                      "Victory Through Freedom:\n\n" +
                                      $"{StyleStart(5)}8) Commoner: $300{StyleEnd(5)}\n\n" +
                                      $"{StyleStart(6)}9) VIP: $5,000{StyleEnd(6)}";
                }
                else
                {
                    _countText.text = "";
                }
            }

            if (name == "Upgrades")
            {
                {
                    // TODO: Update symbols/text/GameState attribute
                    _countText.text = $"$:{GameState.current.CurrentGold}\n" +
                                      $"Armor Level:{BuyRoundLogic.current.upgrades["armor"][0] - 1}\n" +
                                      $"Projectile Speed Level:{BuyRoundLogic.current.upgrades["attackspeed"][0] - 1}\n" +
                                      $"Damage Level:{BuyRoundLogic.current.upgrades["damage"][0] - 1}";
                }
            }
        }

        private String StyleStart(int selection)
        {
            return StyleIfSelected(selection, "<style=\"C3\">");
        }

        private String StyleEnd(int selection)
        {
            return StyleIfSelected(selection, "</style>");
        }

        private String StyleIfSelected(int selection, String text)
        {
            if (selection == BuyRoundLogic.current.BuySelection)
            {
                return text;
            }

            return "";
        }
    }
}