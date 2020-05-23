using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [Header("Screen/Graphics")]
    public Dropdown RESOLUTION_OPTION;
    public Dropdown SCREENTYPE_OPTION;
    public Slider RENDERSCALE_OPTION;
    public Text RENDERSCALE_TEXT_OPTION;

    [Space(5)]
    [Header("Sound")]
    public Slider GLOBALVOLUME_OPTION;

    [Space(5)]
    public UniversalRenderPipelineAsset URPAsset;


    private bool HaveSave = false;
    private Resolution[] RESOLUTIONS;
    private List<string> FullScreenMode_data = new List<string>();

    void Start()
    {
        RESOLUTIONS = Screen.resolutions;
        FullScreenMode_data.Add(FullScreenMode.ExclusiveFullScreen.ToString());
        FullScreenMode_data.Add(FullScreenMode.FullScreenWindow.ToString());
        FullScreenMode_data.Add(FullScreenMode.MaximizedWindow.ToString());
        FullScreenMode_data.Add(FullScreenMode.Windowed.ToString());

        LoadOptions();    
    }

    void Update()
    {
        
    }

    private void LoadOptions()
    {
        List<string> options = new List<string>();

        int CurrentIndex = 0;

        for (int i = 0; i < RESOLUTIONS.Length; i++)
        {
            options.Add(RESOLUTIONS[i].width + "x" + RESOLUTIONS[i].height + "H" + RESOLUTIONS[i].refreshRate);

            if (Screen.currentResolution.width == RESOLUTIONS[i].width && Screen.currentResolution.height == RESOLUTIONS[i].height)
            {
                CurrentIndex = i;
            }
        }

        SCREENTYPE_OPTION.AddOptions(FullScreenMode_data);
        RESOLUTION_OPTION.AddOptions(options);

        if (PlayerPrefs.HasKey("HaveSave"))//Have save and ready to load
        {
            RESOLUTION_OPTION.value = PlayerPrefs.GetInt("RESOLUTION_OPTION");
            RESOLUTION_OPTION.RefreshShownValue();
            SCREENTYPE_OPTION.value = PlayerPrefs.GetInt("SCREENTYPE_OPTION");
            SCREENTYPE_OPTION.RefreshShownValue();

            GLOBALVOLUME_OPTION.value = PlayerPrefs.GetFloat("GLOBALVOLUME_OPTION");

            RENDERSCALE_OPTION.value = PlayerPrefs.GetFloat("RENDERSCALE_OPTION");

            ApplyAll();
        }
        else //Dont have options save
        {
            SCREENTYPE_OPTION.value = (int)Screen.fullScreenMode;
            SCREENTYPE_OPTION.RefreshShownValue();

            RESOLUTION_OPTION.value = CurrentIndex;
            RESOLUTION_OPTION.RefreshShownValue();

            GLOBALVOLUME_OPTION.value = 1;
            RENDERSCALE_OPTION.value = 1;

            ApplyAll();
        }
    }

    public void SAVE_APPLY()
    {
        PlayerPrefs.SetInt("HaveSave", 1);
        PlayerPrefs.SetInt("RESOLUTION_OPTION", RESOLUTION_OPTION.value);
        PlayerPrefs.SetInt("SCREENTYPE_OPTION", SCREENTYPE_OPTION.value);
        PlayerPrefs.SetFloat("GLOBALVOLUME_OPTION", GLOBALVOLUME_OPTION.value);
        PlayerPrefs.SetFloat("RENDERSCALE_OPTION", RENDERSCALE_OPTION.value);

        PlayerPrefs.Save();

        ApplyAll();
    }

    void ApplyAll()
    {
        if (Screen.width != RESOLUTIONS[RESOLUTION_OPTION.value].width && Screen.height != RESOLUTIONS[RESOLUTION_OPTION.value].height)
        {
            switch (SCREENTYPE_OPTION.value)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    Screen.SetResolution(RESOLUTIONS[RESOLUTION_OPTION.value].width, RESOLUTIONS[RESOLUTION_OPTION.value].height, FullScreenMode.ExclusiveFullScreen);
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    Screen.SetResolution(RESOLUTIONS[RESOLUTION_OPTION.value].width, RESOLUTIONS[RESOLUTION_OPTION.value].height, FullScreenMode.FullScreenWindow);
                    break;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                    Screen.SetResolution(RESOLUTIONS[RESOLUTION_OPTION.value].width, RESOLUTIONS[RESOLUTION_OPTION.value].height, FullScreenMode.MaximizedWindow);
                    break;
                case 3:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    Screen.SetResolution(RESOLUTIONS[RESOLUTION_OPTION.value].width, RESOLUTIONS[RESOLUTION_OPTION.value].height, FullScreenMode.Windowed);
                    break;
            }
        }

        AudioListener.volume = GLOBALVOLUME_OPTION.value;
        URPAsset.renderScale = RENDERSCALE_OPTION.value;
        RENDERSCALE_TEXT_OPTION.text = RENDERSCALE_OPTION.value.ToString();
    }

    public void RENDERSCALE_OPTION_ValueChange()
    {
        URPAsset.renderScale = RENDERSCALE_OPTION.value;
        RENDERSCALE_TEXT_OPTION.text = RENDERSCALE_OPTION.value.ToString();
    }
}


[System.Serializable]
public class OptionSerialaizer
{
    public int RESOLUTION_VALUE;
}