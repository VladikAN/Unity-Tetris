using System;
using UnityEngine;

public class ButtonComponent : MonoBehaviour
{
    public Sprite IdleSprite;
    public Sprite PressedSprite;
    public ButtonEvent Event;

    public float PressedTime;
    private float? _coolDown;

    /// <summary>
    /// Validate input
    /// </summary>
	public void OnValidate()
    {
		if (IdleSprite == null || PressedSprite == null) throw new Exception("Idle and Pressed sprites must be specified");
	}

    public void Update()
    {
        if (_coolDown.HasValue)
        {
            _coolDown -= Time.deltaTime;
            if (_coolDown <= 0)
            {
                SetState(false);
                _coolDown = null;
            }
        }
    }

    /// <summary>
    /// Change sprite state
    /// </summary>
    public void SetState(bool pressed)
    {
        var render = GetComponent<SpriteRenderer>();
        render.sprite = pressed ? PressedSprite : IdleSprite;
        
        if (pressed)
        {
            _coolDown = PressedTime;
        }
    }
}