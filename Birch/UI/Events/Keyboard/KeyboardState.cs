namespace Birch.UI.Events.Keyboard;

public class KeyboardState : IEventState
{
    public KeyboardKeys KeyData { get; }

    public KeyboardState(KeyboardKeys keyData)
    {
        KeyData = keyData;
    }

    public virtual bool Alt => (KeyData & KeyboardKeys.Alt) == KeyboardKeys.Alt;
    public bool Control => (KeyData & KeyboardKeys.Control) == KeyboardKeys.Control;
    public virtual bool Shift => (KeyData & KeyboardKeys.Shift) == KeyboardKeys.Shift;
    public KeyboardKeys Modifiers => KeyData & KeyboardKeys.Modifiers;
    public int KeyValue => (int)(KeyData & KeyboardKeys.KeyCode);

    public KeyboardKeys KeyCode
    {
        get
        {
            var keyGenerated = KeyData & KeyboardKeys.KeyCode;

            // since Keys can be discontiguous, keeping Enum.IsDefined.
            if (!Enum.IsDefined(typeof(KeyboardKeys), (int)keyGenerated))
            {
                return KeyboardKeys.None;
            }

            return keyGenerated;
        }
    }
}
