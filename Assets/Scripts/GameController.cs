using System;
using CheapDialogSystem.Runtime.Assets;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    MENU,
    DIALOG,
    PLAYING,
    END
}

public enum StoryState
{
    INTRO,
    SEARCH_KEY,
    HAS_KEY,
    BOSS_FIGHT
}

public class GameController : MonoBehaviour
{
    public static readonly StoryState[] StoryStates = new StoryState[]
    {
        StoryState.INTRO,
        StoryState.SEARCH_KEY,
        StoryState.HAS_KEY,
        StoryState.BOSS_FIGHT
    };

    [SerializeField] private DialogContainer m_introDialog;
    [SerializeField] private PlayerInput m_input;

    [SerializeField] private HandController m_handController;
    [SerializeField] private DialogController m_dialogController;
    
    private GameState m_currentState;
    private StoryState m_currentStoryState;
    private int m_currentStoryIndex;

    public GameState CurrentState => m_currentState;
    public StoryState CurrentStoryState => m_currentStoryState;

    public event Action<GameState, GameState> GameStateChanged;
    public event Action<StoryState> StoryStateUpdated;

    private void Awake()
    {
        m_currentState = GameState.MENU;
        
        m_currentStoryIndex = 0;
        m_currentStoryState = GameController.StoryStates[m_currentStoryIndex];
    }

    private void Start()
    {
        this.StartDialog();
    }
    
    public void SwitchGameState(GameState p_target)
    {
        GameState l_old = m_currentState;
        m_currentState = p_target;
        this.GameStateChanged?.Invoke(m_currentState, l_old);
    }

    public void UpdateStory()
    {
        m_currentStoryIndex++;
        m_currentStoryState = GameController.StoryStates[m_currentStoryIndex];
        this.StoryStateUpdated?.Invoke(m_currentStoryState);
    }

    public void StartDialog()
    {
        this.SwitchGameState(GameState.DIALOG);
        m_handController.ShowHands(false);
        m_input.actions["CameraMove"].Disable();
        m_dialogController.QueueDialog(m_introDialog);
        
        m_input.actions["Fire"].started += this.NextDialog;
        m_dialogController.StartDialog(() =>
        {
            m_input.actions["CameraMove"].Enable();
            m_input.actions["Fire"].started -= this.NextDialog;
            this.SwitchGameState(GameState.PLAYING);
            m_handController.ShowHands(true);
        });
    }

    private void NextDialog(InputAction.CallbackContext p_ctx)
    {
        m_dialogController.ShowNext();
    }
}
