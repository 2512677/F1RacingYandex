namespace RGSK
{
    public interface IInputManager
    {
        bool isEnabled { get; set; }
        bool GetButton(int playerID, InputAction action);
        bool GetButtonDown(int playerID, InputAction action);
        bool GetButtonUp(int playerID, InputAction action);
        float GetAxis(int playerID, InputAction action);
        ITouchInputManager touchInputManager { get; }
    }
}