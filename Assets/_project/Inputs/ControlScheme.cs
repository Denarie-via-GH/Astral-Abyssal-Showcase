//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.2
//     from Assets/_project/Inputs/ControlScheme.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @ControlScheme : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @ControlScheme()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""ControlScheme"",
    ""maps"": [
        {
            ""name"": ""UI"",
            ""id"": ""19564b70-6842-4fd8-9bd0-820ec4c4906f"",
            ""actions"": [
                {
                    ""name"": ""NextLine"",
                    ""type"": ""Button"",
                    ""id"": ""2457e7b0-ba42-4907-9125-128ecfbf2ed3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""64fe8be8-0245-44a1-8f32-43fe113bae65"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Exit"",
                    ""type"": ""Button"",
                    ""id"": ""1b9df983-61ec-4bba-a100-87f18daa79fb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ca8a4b1a-a4c0-4d1a-a7ce-dc4974d6f31b"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextLine"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cdaf0455-0a36-4cb2-ab4f-e465c1481b44"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextLine"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6c4ffbca-f9a6-4e03-b8cc-20144476408b"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a519fcac-9a51-4981-89c5-26cf251a257e"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Player"",
            ""id"": ""7354d86e-e7da-49c0-8101-d931814c47fe"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""9a807172-8f4f-4aa5-93c5-8e708fae2028"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Flashlight"",
                    ""type"": ""Button"",
                    ""id"": ""43038279-8bc5-4fa7-924c-cdd09d2ff0f5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""W/A/S/D"",
                    ""id"": ""02d3e2f1-b5d9-4cde-b457-6a50ed8af74c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""bc1c9d21-2e44-4bef-9fde-fee963d049f8"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5940b01f-3a86-4c67-bf74-5bac08dfd54f"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2401a8ca-d8e9-4996-b388-4c942c61a11d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3a0bbabb-3c9a-4c98-8e42-5a14c26d7bad"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c921282c-0231-48ea-996b-f965937363c3"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Flashlight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""ControlPanel"",
            ""id"": ""65e4813d-4080-40e8-990f-1a4e1621e0a6"",
            ""actions"": [
                {
                    ""name"": ""Tilt"",
                    ""type"": ""Value"",
                    ""id"": ""6b996cfa-7c77-4da4-9b71-1ca2758056b7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""AstralScan"",
                    ""type"": ""Button"",
                    ""id"": ""7065dc7c-5140-456c-a663-0c23a56524ee"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""eb2555de-5618-4d26-af26-30e0ac3a8952"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Map"",
                    ""type"": ""Button"",
                    ""id"": ""f0f678a4-72ff-4235-8368-15988f7bde80"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Radar"",
                    ""type"": ""Button"",
                    ""id"": ""9d1d7d21-da41-403a-b9ab-784971e6c00c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""3b545fb4-633f-4f7d-8fef-a0c9fbc56608"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tilt"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""e7648976-150e-4a87-8be6-60db23652552"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tilt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""69ba5594-76ae-4f9a-9da9-5dfe7e0b4202"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tilt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4f729a2e-2e63-4a41-814d-14b39f106dd6"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AstralScan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b508a4bb-e0dc-400c-8319-02490790ae46"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""24dc2f06-0661-4c42-9b80-ec5905ec3ee4"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Map"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e7145d7f-8503-404c-8414-ba9d0e2e519d"",
                    ""path"": ""<Keyboard>/n"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Radar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_NextLine = m_UI.FindAction("NextLine", throwIfNotFound: true);
        m_UI_Scroll = m_UI.FindAction("Scroll", throwIfNotFound: true);
        m_UI_Exit = m_UI.FindAction("Exit", throwIfNotFound: true);
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Flashlight = m_Player.FindAction("Flashlight", throwIfNotFound: true);
        // ControlPanel
        m_ControlPanel = asset.FindActionMap("ControlPanel", throwIfNotFound: true);
        m_ControlPanel_Tilt = m_ControlPanel.FindAction("Tilt", throwIfNotFound: true);
        m_ControlPanel_AstralScan = m_ControlPanel.FindAction("AstralScan", throwIfNotFound: true);
        m_ControlPanel_Move = m_ControlPanel.FindAction("Move", throwIfNotFound: true);
        m_ControlPanel_Map = m_ControlPanel.FindAction("Map", throwIfNotFound: true);
        m_ControlPanel_Radar = m_ControlPanel.FindAction("Radar", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_NextLine;
    private readonly InputAction m_UI_Scroll;
    private readonly InputAction m_UI_Exit;
    public struct UIActions
    {
        private @ControlScheme m_Wrapper;
        public UIActions(@ControlScheme wrapper) { m_Wrapper = wrapper; }
        public InputAction @NextLine => m_Wrapper.m_UI_NextLine;
        public InputAction @Scroll => m_Wrapper.m_UI_Scroll;
        public InputAction @Exit => m_Wrapper.m_UI_Exit;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @NextLine.started -= m_Wrapper.m_UIActionsCallbackInterface.OnNextLine;
                @NextLine.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnNextLine;
                @NextLine.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnNextLine;
                @Scroll.started -= m_Wrapper.m_UIActionsCallbackInterface.OnScroll;
                @Scroll.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnScroll;
                @Scroll.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnScroll;
                @Exit.started -= m_Wrapper.m_UIActionsCallbackInterface.OnExit;
                @Exit.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnExit;
                @Exit.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnExit;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @NextLine.started += instance.OnNextLine;
                @NextLine.performed += instance.OnNextLine;
                @NextLine.canceled += instance.OnNextLine;
                @Scroll.started += instance.OnScroll;
                @Scroll.performed += instance.OnScroll;
                @Scroll.canceled += instance.OnScroll;
                @Exit.started += instance.OnExit;
                @Exit.performed += instance.OnExit;
                @Exit.canceled += instance.OnExit;
            }
        }
    }
    public UIActions @UI => new UIActions(this);

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Flashlight;
    public struct PlayerActions
    {
        private @ControlScheme m_Wrapper;
        public PlayerActions(@ControlScheme wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Flashlight => m_Wrapper.m_Player_Flashlight;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Flashlight.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFlashlight;
                @Flashlight.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFlashlight;
                @Flashlight.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFlashlight;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Flashlight.started += instance.OnFlashlight;
                @Flashlight.performed += instance.OnFlashlight;
                @Flashlight.canceled += instance.OnFlashlight;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // ControlPanel
    private readonly InputActionMap m_ControlPanel;
    private IControlPanelActions m_ControlPanelActionsCallbackInterface;
    private readonly InputAction m_ControlPanel_Tilt;
    private readonly InputAction m_ControlPanel_AstralScan;
    private readonly InputAction m_ControlPanel_Move;
    private readonly InputAction m_ControlPanel_Map;
    private readonly InputAction m_ControlPanel_Radar;
    public struct ControlPanelActions
    {
        private @ControlScheme m_Wrapper;
        public ControlPanelActions(@ControlScheme wrapper) { m_Wrapper = wrapper; }
        public InputAction @Tilt => m_Wrapper.m_ControlPanel_Tilt;
        public InputAction @AstralScan => m_Wrapper.m_ControlPanel_AstralScan;
        public InputAction @Move => m_Wrapper.m_ControlPanel_Move;
        public InputAction @Map => m_Wrapper.m_ControlPanel_Map;
        public InputAction @Radar => m_Wrapper.m_ControlPanel_Radar;
        public InputActionMap Get() { return m_Wrapper.m_ControlPanel; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ControlPanelActions set) { return set.Get(); }
        public void SetCallbacks(IControlPanelActions instance)
        {
            if (m_Wrapper.m_ControlPanelActionsCallbackInterface != null)
            {
                @Tilt.started -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnTilt;
                @Tilt.performed -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnTilt;
                @Tilt.canceled -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnTilt;
                @AstralScan.started -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnAstralScan;
                @AstralScan.performed -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnAstralScan;
                @AstralScan.canceled -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnAstralScan;
                @Move.started -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnMove;
                @Map.started -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnMap;
                @Map.performed -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnMap;
                @Map.canceled -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnMap;
                @Radar.started -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnRadar;
                @Radar.performed -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnRadar;
                @Radar.canceled -= m_Wrapper.m_ControlPanelActionsCallbackInterface.OnRadar;
            }
            m_Wrapper.m_ControlPanelActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Tilt.started += instance.OnTilt;
                @Tilt.performed += instance.OnTilt;
                @Tilt.canceled += instance.OnTilt;
                @AstralScan.started += instance.OnAstralScan;
                @AstralScan.performed += instance.OnAstralScan;
                @AstralScan.canceled += instance.OnAstralScan;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Map.started += instance.OnMap;
                @Map.performed += instance.OnMap;
                @Map.canceled += instance.OnMap;
                @Radar.started += instance.OnRadar;
                @Radar.performed += instance.OnRadar;
                @Radar.canceled += instance.OnRadar;
            }
        }
    }
    public ControlPanelActions @ControlPanel => new ControlPanelActions(this);
    public interface IUIActions
    {
        void OnNextLine(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
        void OnExit(InputAction.CallbackContext context);
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnFlashlight(InputAction.CallbackContext context);
    }
    public interface IControlPanelActions
    {
        void OnTilt(InputAction.CallbackContext context);
        void OnAstralScan(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnMap(InputAction.CallbackContext context);
        void OnRadar(InputAction.CallbackContext context);
    }
}
