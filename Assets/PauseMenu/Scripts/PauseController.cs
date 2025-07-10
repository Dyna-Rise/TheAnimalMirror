using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    GameState preGameState;

    Camera cam;
    PlayerFollow playerFollow;
    Vector3 defaultOffset;
    AudioListener audioListener;

    public GameObject pausePanel;
    public Slider[] pauseCameraSlider;
    public Toggle pauseSoundToggle;
    public TMP_Dropdown pauseTimeDropdown;

    float[] sliderValue = new float[3];
    bool isChanged;
    bool toggleIsOn = true;
    string dropdownText;

    bool inPause; // ポーズ中かどうかのフラグ

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneUnloaded += SceneUnloaded;

        cam = Camera.main;
        playerFollow = cam.gameObject.GetComponent<PlayerFollow>();
        defaultOffset = playerFollow.offset;
        audioListener = cam.gameObject.GetComponent<AudioListener>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!inPause)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    void Pause()
    {
        preGameState = GameController.gameState;
        GameController.gameState = GameState.pause;

        Time.timeScale = 0;
        pausePanel.SetActive(true);
        inPause = true;
    }

    void Resume()
    {
        GameController.gameState = preGameState;

        if (isChanged)
        {
            Vector3 newOffset = new Vector3(sliderValue[0], sliderValue[1], sliderValue[2]);
            playerFollow.offset = defaultOffset + newOffset;
            isChanged = false;
        }

        if (!toggleIsOn)
        {
            audioListener.enabled = false;
        }
        else
        {
            audioListener.enabled = true;
        }

        if (dropdownText == "2倍")
        {
            Time.timeScale = 2.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        pausePanel.SetActive(false);
        inPause = false;
    }

    void SceneUnloaded(Scene scene)
    {
        if (Time.timeScale != 1.0f)
        {
            Time.timeScale = 1.0f;
        }
    }

    public void SliderChangedX()
    {
        sliderValue[0] = pauseCameraSlider[0].value;
        isChanged = true;
    }
    public void SliderChangedY()
    {
        sliderValue[1] = pauseCameraSlider[1].value;
        isChanged = true;
    }
    public void SliderChangedZ()
    {
        sliderValue[2] = pauseCameraSlider[2].value;
        isChanged = true;
    }

    public void ToggleChanged()
    {
        toggleIsOn = pauseSoundToggle.isOn;
    }

    public void DropdownChanged()
    {
        dropdownText = pauseTimeDropdown.options[pauseTimeDropdown.value].text;
    }
}
