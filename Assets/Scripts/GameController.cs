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
    FIRST_BOSS,
    SEARCH_KEY,
    HAS_KEY,
    BOSS_FIGHT
}

public class GameController : MonoBehaviour
{
    public static readonly StoryState[] StoryStates = new StoryState[]
    {
        StoryState.INTRO,
        StoryState.FIRST_BOSS,
        StoryState.SEARCH_KEY,
        StoryState.HAS_KEY,
        StoryState.BOSS_FIGHT
    };

    [SerializeField] private DialogContainer m_introDialog;
    [SerializeField] private DialogContainer m_firstEncounterDialog;
    [SerializeField] private PlayerInput m_input;

    [SerializeField] private CharacterMovement m_character;
    [SerializeField] private AudioSource m_musicPlayer;
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
        this.StartIntroDialog();
    }
    
    public void SwitchGameState(GameState p_target)
    {
        GameState l_old = m_currentState;
        m_currentState = p_target;
        this.GameStateChanged?.Invoke(m_currentState, l_old);
    }

    private void UpdateStory()
    {
        m_currentStoryIndex++;
        m_currentStoryState = GameController.StoryStates[m_currentStoryIndex];
        this.StoryStateUpdated?.Invoke(m_currentStoryState);
    }

    private void StartIntroDialog()
    {
        this.StartDialog(m_introDialog, this.OnIntroDialogEnd); 
    }

    private void OnIntroDialogEnd()
    {
        this.UpdateStory();
        m_musicPlayer.Play();
    }

    public void OnFirstEncounterDialog(Transform p_target)
    {
        this.StartDialog(m_firstEncounterDialog, this.OnFirstEncounterDialogEnd);
    }
    
    private void OnFirstEncounterDialogEnd()
    {
        this.UpdateStory();
        m_handController.GiveGun();
    }

    public void StartDialog(DialogContainer p_dialog, Action p_endCallback)
    {
        this.SwitchGameState(GameState.DIALOG);
        this.LowerMusic();
        m_handController.ShowHands(false);
        m_input.actions["CameraMove"].Disable();
        m_dialogController.QueueDialog(p_dialog);
        m_input.actions["Fire"].started += this.NextDialog;
        
        m_dialogController.StartDialog(() =>
        {
            this.UpperMusic();
            m_input.actions["CameraMove"].Enable();
            m_input.actions["Fire"].started -= this.NextDialog;
            this.SwitchGameState(GameState.PLAYING);
            m_handController.ShowHands(true);
            
            p_endCallback?.Invoke();
        });
    }

    private void LowerMusic()
    {
        m_musicPlayer.volume = 0.2f;
    }
    
    private void UpperMusic()
    {
        m_musicPlayer.volume = 1.0f;
    }

    private void NextDialog(InputAction.CallbackContext p_ctx)
    {
        m_dialogController.ShowNext();
    }
}
