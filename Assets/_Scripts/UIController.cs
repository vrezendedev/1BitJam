using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{

    private UIDocument _UI;
    private PlayerMovement _playerMovement;
    private PlayerInteractivity _playerInteractivity;
    private LightManager _lightManager;
    public Sprite running;
    public Sprite recovering;
    public Sprite normal;

    void Awake()
    {
        _UI = this.gameObject.GetComponent<UIDocument>();
        _playerMovement = FindFirstObjectByType<PlayerMovement>().GetComponent<PlayerMovement>();
        _playerInteractivity = FindFirstObjectByType<PlayerInteractivity>().GetComponent<PlayerInteractivity>();
        _lightManager = FindFirstObjectByType<LightManager>().GetComponent<LightManager>();
    }

    void Update()
    {
        TrackInteractables();
        TrackRunning();
    }

    private void TrackInteractables()
    {
        var intr = _UI.rootVisualElement.Q<VisualElement>("Interact");
        Debug.Log(_playerInteractivity.LookingAt);
        if (_playerInteractivity.LookingAt.Length > 0)
        {
            switch (_playerInteractivity.LookingAt)
            {
                case "Pointer":
                    if (!_lightManager.isRotating)
                        intr.visible = true;
                    else
                        intr.visible = false;
                    break;
                case "Lever":
                    intr.visible = true;
                    break;
                default:
                    intr.visible = true;
                    break;
            }
        }
        else
        {
            intr.visible = false;
        }
    }

    private void TrackRunning()
    {
        var rng = _UI.rootVisualElement.Q<VisualElement>("Running");

        if (_playerMovement.running)
        {
            rng.style.backgroundImage = new StyleBackground(running);
        }
        else if (_playerMovement.recovering)
        {
            rng.style.backgroundImage = new StyleBackground(recovering);
        }
        else
        {
            rng.style.backgroundImage = new StyleBackground(normal);
        }

        var stm = _UI.rootVisualElement.Q<VisualElement>("StaminaMask");
        stm.style.width = 16;
        stm.style.height = Mathf.Lerp(64, 0, 1 - Mathf.InverseLerp(0, _playerMovement._initialStamina, _playerMovement.stamina));
    }
}
