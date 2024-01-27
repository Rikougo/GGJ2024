using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class MouseDeltaNormalizer : InputProcessor<Vector2>
{
    [Tooltip("Number to add to incoming values.")]
    public float valueShift = 0;

    public override Vector2 Process(Vector2 p_value, InputControl p_control)
    {
        return p_value / new Vector2(Screen.width, Screen.height);
    }
    
    #if UNITY_EDITOR
    static MouseDeltaNormalizer()
    {
        Initialize();
    }
    #endif
    
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<MouseDeltaNormalizer>();
    }
}