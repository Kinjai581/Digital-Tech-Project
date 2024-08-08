namespace Test;

public class Timer
{
    private readonly Texture2D _texture;

    private readonly Vector2 _position;

    private readonly SpriteFont _font;

    private readonly Vector2 _textposition;

    private string _text;

    private readonly float _timeLength;

    private bool _active;

    public bool Repeat {get; set;}

    public Timer(Texture2D texture, SpriteFont font, Vector2 position, float length){
        _texture = texture;
        _font = font;
        _position = position;
    } 
}