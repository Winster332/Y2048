namespace Birch.UI.Events.Keyboard;

public class KeyboardPress : IEventState
{
    public char Key { get; }
    public KeyboardPress(char key)
    {
        Key = key;
    }
}
