using UnityEngine;
using System;
using BricksGame;
using System.Linq;

public class BricksEngineComponent : MonoBehaviour
{
    public InfoBlockComponent ScoreInfoBlock;
    public InfoBlockComponent LevelInfoBlock;
    public BricksRenderComponent FieldRender;
    public BricksRenderComponent NextFigureRender;
    public GameObject PausedLabel;
    public float StartGameSpeed;
    public float GameSpeedModificator;
    public float InputSpeed;

    private bool _running;
    private int _score;

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
        // Set initial state
        SetInitinalState();

        _running = true;
        ChangeGameState(_running);

        // Set next render frame
        UpdateTime();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public void Update()
    {
        // Update user touch/keyboard input
        UpdateInput();

        // If field is collided with figure then game is over
        if (_field.CheckCollision(_currentFigure))
        {
            // Reset game
            SetInitinalState();
        }

        if (_running && Time.time >= _nextTimeFrame)
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
        FieldRender.Render(_field.Map);
        FieldRender.Render(_currentFigure.Points, _currentFigure.X, _currentFigure.Y, false);
        NextFigureRender.Render(_nextFigure.Points);
    }

    /// <summary>
    /// Validate all public parameters not null
    /// </summary>
    public void OnValidate()
    {
        if (ScoreInfoBlock == null) throw new Exception("Score Info Block is not specified for this scene");
        if (LevelInfoBlock == null) throw new Exception("Level Info Block is not specified for this scene");
        if (FieldRender == null) throw new Exception("Field Render is not specified for this scene");
        if (NextFigureRender == null) throw new Exception("Next Figure Render is not specified for this scene");
        if (PausedLabel == null) throw new Exception("Paused label us not specified for this scene");
        if (StartGameSpeed == 0) throw new Exception("Start Game Speed is not specified for this scene");
        if (GameSpeedModificator == 0) throw new Exception("Game Speed Modificator is not specified for this scene");
    }

    public void ResumeGame()
    {
        ChangeGameState(true);
    }

    public void PauseGame()
    {
        ChangeGameState(false);
    }

    private void ChangeGameState(bool? running = null)
    {
        _running = running ?? !_running;
        PausedLabel.SetActive(!_running);
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
    }

    /// <summary>
    /// Read user input and move figure according to that
    /// </summary>
    private void UpdateInput()
    {
        // Init _left, _right, _down and _rotate variables
        ReadInput();

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
        }
        
        if (_right)
        {
            _currentFigure.MoveRightIfAllowed(_field);
        }

        if (_down)
        {
            while (_currentFigure.IsAllowedToMoveDown(_field))
            {
                _currentFigure.MoveDownIfAllowed(_field);
            }

            // Update time will allow to move figure left and right after landing
            UpdateTime();
        }

        if (_rotate)
        {
            _currentFigure.RotateIfAllowed(_field);
        }
    }

    private void ReadInput()
    {
        if (Input.GetButtonUp("Pause"))
        {
            ChangeGameState();
        }

        if (_running)
        {
            // Input logic for keyboard
            var horizontal = Input.GetAxis("Horizontal");
            _left |= horizontal < 0;
            _right |= horizontal > 0;

            var vertical = Input.GetAxis("Vertical");
            _down |= vertical < 0;
            _rotate = vertical > 0;

            // Input logic for touch screen and mouse
            var positions = Input.touches.Where(x => x.phase == TouchPhase.Began || x.phase == TouchPhase.Stationary).Select(x => x.position).ToList();
            if (Input.GetMouseButtonUp(0)) positions.Add(Input.mousePosition);

            foreach (var position in positions)
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.up);
                if (hit.collider != null)
                {
                    var button = hit.collider.GetComponent<ButtonComponent>();
                    if (button == null) continue;

                    button.SetState(true);
                    _left |= button.Left;
                    _right |= button.Right;
                    _down |= button.Down;
                    _rotate |= button.Rotate;
                }
            }
        }
    }

    /// <summary>
    /// Update score/level accoding to deleted lines and display values
    /// </summary>
    private void UpdateStats(int deletedLines)
    {
        _score += 10 * deletedLines + 5 * (deletedLines != 0 ? deletedLines - 1 : 0);
        var level = _score / 100 + 1;
        _gameSpeed = StartGameSpeed + GameSpeedModificator * (level - 1);

        ScoreInfoBlock.PrintValue(_score);
        LevelInfoBlock.PrintValue(level);
    }

    /// <summary>
    /// Set next time frame according to game speed
    /// </summary>
    private void UpdateTime()
    {
        _nextTimeFrame = Time.time + _gameSpeed;
    }
}