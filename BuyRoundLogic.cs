using System.Collections.Generic;
using input;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuyRoundLogic : MonoBehaviour
{
    public static BuyRoundLogic current;
    private bool _buyingPhase;
    private bool _invested;
    public Dictionary<string, List<int>> upgrades { get; private set; }
    public int BuySelection { get; private set; }
    private InputController inputController;

    private void Awake()
    {
        inputController = new InputController();
    }

    void Start()
    {
        current = this;
        upgrades = new Dictionary<string, List<int>>();
        // List format is { upgradeLevel, upgradeBonus, upgradeCost }
        upgrades["armor"] = new List<int>() {1, 25, 25};
        upgrades["damage"] = new List<int>() {1, 10, 25};
        upgrades["attackspeed"] = new List<int>() {1, 25, 25};
        GameEvents.current.OnBuyRound += HandleBuyRound;
    }

    private void UpdateBuySelection(int selection)
    {
        BuySelection = selection;
        GameEvents.current.UpdateUI();
    }

    private void OnEnable()
    {
        inputController.Player.Navigate.Enable();
        inputController.Player.Buy.Enable();
    }

    private void OnDisable()
    {
        inputController.Player.Navigate.performed -= Navigated;
        inputController.Player.Buy.performed -= BuySelected;
    }

    private void BuySelected(InputAction.CallbackContext obj)
    {
        BuySelected(BuySelection);
    }

    private void BuySelected(int buySelection)
    {
        switch (buySelection)
        {
            case 1:
                UpgradePurchase("armor");
                break;
            case 2:
                UpgradePurchase("attackspeed");

                break;
            case 3:
                UpgradePurchase("damage");
                break;
            case 4:
                InvestFunds();
                break;
            case 5:
                FreedomPurchase(1);
                break;
            case 6:
                FreedomPurchase(2);
                break;
        }
    }

    private void Navigated(InputAction.CallbackContext obj)
    {
        if (!GameState.current.Paused)
        {
            var newSelection = BuySelection - (int) obj.ReadValue<Vector2>().y;
            if (newSelection < 1)
            {
                newSelection = 6;
            }
            else if (newSelection > 6)
            {
                newSelection = 1;
            }

            UpdateBuySelection(newSelection);
        }
    }

    private void HandleBuyRound(bool buyBool)
    {
        if (!buyBool && _buyingPhase && !_invested)
        {
            InvestFunds();
        }
        else
        {
            _invested = false;
        }

        _buyingPhase = buyBool;
        if (buyBool)
        {
            inputController.Player.Navigate.performed += Navigated;
            inputController.Player.Buy.performed += BuySelected;
        }
        else
        {
            inputController.Player.Navigate.performed -= Navigated;
            inputController.Player.Buy.performed -= BuySelected;
        }

        UpdateBuySelection(1);
    }

    private void Update()
    {
        //TODO Handle keyboard using new input.
        if (_buyingPhase)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                BuySelected(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                BuySelected(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                BuySelected(3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                BuySelected(4);
            }

            if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
            {
                BuySelected(5);
            }

            if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
            {
                BuySelected(6);
            }
        }
    }


    private void FreedomPurchase(int tier)
    {
        if (tier == 1)
        {
            // Commoner

            // Display Victory Text

            // Menu with Restart or Quit
            FreedomPurchase(300, tier);
        }

        if (tier == 2)
        {
            // VIP

            // Display Victory Text

            // Menu with Restart or Quit
            FreedomPurchase(5000, tier);
        }
    }

    private void FreedomPurchase(int gold, int tier)
    {
        if (GameState.current.CurrentGold >= gold)
        {
            inputController.Player.Buy.performed -= BuySelected;
            GameEvents.current.Victory(tier);
            StartCoroutine(KillPlayer());
        }
    }

    private IEnumerator<WaitForSeconds> KillPlayer()
    {
        //This is done separately so that the button doesn't auto press on load.
        yield return new WaitForSeconds(.1f);
        GameEvents.current.PlayerKilled();
    }

    private void InvestFunds()
    {
        _invested = true;
        int investmentAmount = GameState.current.CurrentGold;
        GameEvents.current.Investment(investmentAmount);
    }

    private void UpgradePurchase(string upgradeType)
    {
        // Armor
        if (GameState.current.CurrentGold >= upgrades[upgradeType][2])
        {
            GameEvents.current.Upgrade(upgradeType, upgrades[upgradeType]);
            upgrades[upgradeType][0] += 1;
            if (upgradeType == "armor")
            {
                upgrades[upgradeType][1] *= 2;
            }

            upgrades[upgradeType][2] *= 2;
        }
    }
}