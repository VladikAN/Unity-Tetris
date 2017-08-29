using UnityEngine;
using System;
using BricksGame;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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

    private bool _running = true;
    private int _score;

    private Field _field = new Field();
    private Figure _currentFigure = new Figure();
    private Figure _nextFigure = new Figure();

    private float _inputTimeout;
    private float _gameSpeed;
    private float _nextTimeFrame;
    private ButtonEvent _buttonEvent = ButtonEvent.None;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    public void Start()
    {
        // Set initial state
        var loaded = Load();
        if (!loaded)
        {
            _score = 0;
            ChangeGameState(true);
        }
        else
        {
            ChangeGameState(false);
        }

        SetInitinalState(false);

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
            SetInitinalState(true);
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
    /// Hanle application close/hide event
    /// </summary>
    /// <param name="paused"></param>
    public void OnApplicationPause(bool paused)
    {
        ChangeGameState(!paused);

        if (paused)
        {
            Save();
        }
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

    private void ChangeGameState(bool? running = null)
    {
        _running = running ?? !_running;
        _buttonEvent = ButtonEvent.None;
        PausedLabel.SetActive(!_running);
    }

    private void Save()
    {
        var bf = new BinaryFormatter();
        var file = File.Open(Application.persistentDataPath + "/FileName.dat", FileMode.Create);
        var data = new GameState
        {
            Score = _score,
            Field = _field.Map,
            Figure = _currentFigure.Points,
            FigureX = _currentFigure.X,
            FigureY = _currentFigure.Y,
            Next = _nextFigure.Points,
            NextX = _nextFigure.X,
            NextY = _nextFigure.Y
        };

        bf.Serialize(file, data);
        file.Close();
    }

    private bool Load()
    {
        if (File.Exists(Application.persistentDataPath + "/FileName.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/FileName.dat", FileMode.Open);
            var data = (GameState)bf.Deserialize(file);
            file.Close();

            _score = data.Score;
            _field = new Field(data.Field);
            _currentFigure = new Figure(data.Figure);
            _currentFigure.X = data.FigureX;
            _currentFigure.Y = data.FigureY;
            _nextFigure = new Figure(data.Next);
            _nextFigure.X = data.NextX;
            _nextFigure.Y = data.NextY;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Reset all parameters to initial
    /// </summary>
    private void SetInitinalState(bool reset)
    {
        // Reset speed
        _inputTimeout = 0;
        _gameSpeed = StartGameSpeed;

        // Reset stats
        UpdateStats(0);

        // Reset field and figures
        if (reset)
        {
            _field.Reset();
            _currentFigure.BuildNew(UnityEngine.Random.Range(0, 6), UnityEngine.Random.Range(0, 3));
            _currentFigure.X = 3;
            _currentFigure.Y = 0;
            _nextFigure.BuildNew(UnityEngine.Random.Range(0, 6), UnityEngine.Random.Range(0, 3));
        }
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
            _buttonEvent = ButtonEvent.None;
            return;
        }

        if (_buttonEvent != ButtonEvent.None)
        {
            _inputTimeout = InputSpeed;
        }

        if (_buttonEvent.HasFlag(ButtonEvent.PauseAndResume))
        {
            ChangeGameState();
        }

        if (_buttonEvent.HasFlag(ButtonEvent.SaveAndExit))
        {
            ChangeGameState(false);
            Save();
            Application.Quit();
        }

        if (_running)
        {
            if (_buttonEvent.HasFlag(ButtonEvent.Left))
            {
                _currentFigure.MoveLeftIfAllowed(_field);
            }

            if (_buttonEvent.HasFlag(ButtonEvent.Right))
            {
                _currentFigure.MoveRightIfAllowed(_field);
            }

            if (_buttonEvent.HasFlag(ButtonEvent.Down))
            {
                var downAllowed = _currentFigure.IsAllowedToMoveDown(_field);
                while (_currentFigure.IsAllowedToMoveDown(_field))
                {
                    _currentFigure.MoveDownIfAllowed(_field);
                }

                // Update time will allow to move figure left and right after landing
                if (downAllowed)
                {
                    UpdateTime();
                }
            }

            if (_buttonEvent.HasFlag(ButtonEvent.Rotate))
            {
                _currentFigure.RotateIfAllowed(_field);
            }
        }
    }

    private void ReadInput()
    {
        // Input logic for keyboard
        _buttonEvent |= Input.GetButtonUp("Pause") ? ButtonEvent.PauseAndResume : ButtonEvent.None;

        var horizontal = Input.GetAxis("Horizontal");
        _buttonEvent |= horizontal < 0 ? ButtonEvent.Left : ButtonEvent.None;
        _buttonEvent |= horizontal > 0 ? ButtonEvent.Right : ButtonEvent.None;

        var vertical = Input.GetAxis("Vertical");
        _buttonEvent |= vertical < 0 ? ButtonEvent.Down : ButtonEvent.None;
        _buttonEvent |= vertical > 0 ? ButtonEvent.Rotate : ButtonEvent.None;

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
                _buttonEvent |= button.Event;
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