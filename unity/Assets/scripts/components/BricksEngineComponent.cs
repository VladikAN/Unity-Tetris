using UnityEngine;
using System;
using BricksGame;

public class BricksEngineComponent : MonoBehaviour
{
    public InfoBlockComponent ScoreInfoBlock;
    public InfoBlockComponent LevelInfoBlock;
    public BricksRenderComponent FieldRender;
    public BricksRenderComponent NextFigureRender;
    public float StartGameSpeed;
    public float GameSpeedModificator;
    public float InputSpeed;

    private int _score;
    private int _level;

    private Field _field;
    private Figure _currentFigure;
    private Figure _nextFigure;

    private float _inputTimeout;
    private float _gameSpeed;
    private float _nextTimeFrame;

    private bool _left;
    private bool _right;
    private bool _down;
    private bool _rotate;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    public void Start()
    {
        // Validate input parameters
        Validate();

        // Set initial state
        SetInitinalState();

        // Set next render frame
        UpdateTime();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public void Update()
    {
        // Update user input
        UpdateInput();

        // If field is collided with figure then game is over
        if (_field.CheckCollision(_currentFigure))
        {
            // Reset game
            SetInitinalState();
        }

        if (Time.time >= _nextTimeFrame)
        {
            // if figure is on the ground
            if (!_currentFigure.IsAllowedToMoveDown(_field))
            {
                // Merge figure to field
                _field.Merge(_currentFigure);

                // Remove full lines and update stats
                var deleted = _field.RemoveFullLines();
                UpdateStats(deleted);

                // Merge next figure to current and reset position
                _currentFigure.Set(_nextFigure.Points);
                _currentFigure.X = 3;
                _currentFigure.Y = 0;

                // Generate next shape
                _nextFigure.BuildNew(UnityEngine.Random.Range(0, 6), UnityEngine.Random.Range(0, 3));
            }
            else
            {
                // Figure is still fallen
                _currentFigure.MoveDownIfAllowed(_field);
            }

            UpdateTime();
        }

        // Render all shapes
        RenderAllShapes();
    }

    /// <summary>
    /// Validate all public parameters not null
    /// </summary>
    private void Validate()
    {
        if (ScoreInfoBlock == null)
        {
            throw new Exception("Score Info Block is not specified for this scene");
        }

        if (LevelInfoBlock == null)
        {
            throw new Exception("Level Info Block is not specified for this scene");
        }

        if (FieldRender == null)
        {
            throw new Exception("Field Render is not specified for this scene");
        }

        if (NextFigureRender == null)
        {
            throw new Exception("Next Figure Render is not specified for this scene");
        }

        if (StartGameSpeed == 0)
        {
            throw new Exception("Start Game Speed is not specified for this scene");
        }

        if (GameSpeedModificator == 0)
        {
            throw new Exception("Game Speed Modificator is not specified for this scene");
        }
    }

    /// <summary>
    /// Update all render components
    /// </summary>
    private void RenderAllShapes()
    {
        FieldRender.Render(_field.Map);
        FieldRender.Render(_currentFigure.Points, _currentFigure.X, _currentFigure.Y, false);
        NextFigureRender.Render(_nextFigure.Points);
    }

    /// <summary>
    /// Reset all parameters to initial
    /// </summary>
    private void SetInitinalState()
    {
        // Reset speed
        _inputTimeout = 0;
        _gameSpeed = StartGameSpeed;

        // Reset stats
        _score = 0;
        _level = 0;
        UpdateStats(0);

        // Reset field and figures
        if (_field == null)
        {
            _field = new Field();
        }
        _field.Reset();

        if (_currentFigure == null)
        {
            _currentFigure = new Figure();
        }
        _currentFigure.BuildNew(UnityEngine.Random.Range(0, 6), UnityEngine.Random.Range(0, 3));
        _currentFigure.X = 3;
        _currentFigure.Y = 0;

        if (_nextFigure == null)
        {
            _nextFigure = new Figure();
        }
        _nextFigure.BuildNew(UnityEngine.Random.Range(0, 6), UnityEngine.Random.Range(0, 3));

        // Render all shapes
        RenderAllShapes();
    }

    private void UpdateInput()
    {
        if (Input.touchCount > 0)
        {
            for (var i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
                {
                    var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.up);
                    if (hit.collider != null)
                    {
                        var button = hit.collider.GetComponent<ButtonComponent>();
                        if (button == null)
                        {
                            continue;
                        }

                        button.SetState(true);
                        _left |= button.Left;
                        _right |= button.Right;
                        _down |= button.Down;
                        _rotate |= button.Rotate;
                    }
                }
            }
        }
        else
        {
            var horizontal = Input.GetAxis("Horizontal");
            if (horizontal != 0)
            {
                _left |= horizontal < 0;
                _right = !_left;
            }

            var vertical = Input.GetAxis("Vertical");
            if (vertical != 0)
            {
                _down |= vertical < 0;
                _rotate = !_down;
            }
        }

        _inputTimeout -= Time.deltaTime;
        if (_inputTimeout >= 0)
        {
            _left = false;
            _right = false;
            _down = false;
            _rotate = false;
            return;
        }

        if (_left || _right || _down || _rotate)
        {
            _inputTimeout = InputSpeed;
        }

        if (_left)
        {
            _currentFigure.MoveLeftIfAllowed(_field);
            _left = false;
        }
        
        if (_right)
        {
            _currentFigure.MoveRightIfAllowed(_field);
            _right = false;
        }

        if (_down)
        {
            while (_currentFigure.IsAllowedToMoveDown(_field))
            {
                _currentFigure.MoveDownIfAllowed(_field);
            }
            _down = false;
        }

        if (_rotate)
        {
            _currentFigure.RotateIfAllowed(_field);
            _rotate = false;
        }
    }

    /// <summary>
    /// Update score/level accoding to deleted lines and display values
    /// </summary>
    private void UpdateStats(int deletedLines)
    {
        _score += 10 * deletedLines;
        _level = _score / 100 + 1;
        _gameSpeed = StartGameSpeed + GameSpeedModificator * (_level - 1);

        ScoreInfoBlock.PrintValue(_score);
        LevelInfoBlock.PrintValue(_level);
    }

    /// <summary>
    /// Set next time frame according to game speed
    /// </summary>
    private void UpdateTime()
    {
        _nextTimeFrame = Time.time + _gameSpeed;
    }
}