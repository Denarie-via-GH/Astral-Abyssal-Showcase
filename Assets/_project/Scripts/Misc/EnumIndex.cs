using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public enum SceneIndex
    {
        PERSISTENT = 0,
        MAIN_MENU = 1,
        INTRO_SCENE = 2,
        CONTROL_ROOM = 3,
        ORBITER = 4,
        EVENT_SCENE_CRATOR = 5,
        EVENT_SCENE_ASTEROIDFIELD = 6,
        EVENT_SCENE_TUNNEL = 7,
        EVENT_SCENE_FINAL = 8,
        OUTRO_SCENE = 9
    }
    public enum CameraFocusIndex
    {
        UTR_CAMERA = 0,
        MINIMAP = 1,
        RADAR = 2
    }
    public enum AmbientClipIndex
    {
        PRI_CRATOR = 0,
        SEC_FIRST_EYE = 1,
        PRI_ASTEROIDFIELD = 2,
        PRI_TUNNEL = 3,
        SEC_TUNNEL_EXIT = 4,
        SEC_DEVOURER_APPEAR = 5,
        PRI_BLACKHOLE = 6
    }
    public enum SFXClipIndex
    {
        INTERACT_VISUALIZER = 0,
        INTERACT_SLEEPSTATION = 1,
        INTERACT_SCAN = 2,
        DOCK_MODE = 3,
        BUTTON_1 = 4,
        DOOR_BUTTON_2 = 5,
        DOOR_SFX = 6,
        VENTILATION = 7,
        RECORD_LOOP = 8,
        SCAN_LOOP = 9,
        SAMPLE_LOOP = 10,
        TRANSFER_OBJECT = 11,
        SPACELAB_TRANSITION = 12,
        DISPENSE = 13,
        USE_BATHROOM = 14,
        EVENT_FIRST_EYE = 15,
        EVENT_CREATURE_INFILTRATED = 16,
        ORBITER_CRASH = 17,
        ORBITER_CRASH_SLIDE = 18,
        ENCOUNTER_SENTINELS = 19,
        FINAL_JUMPSCARE = 20,
        CRASH_EVENT = 21
    }
    public enum UIClipIndex
    {
        PRESS_BUTTON = 0,
        MOVE_BUTTON = 1,
        ROTATE_BUTTON = 2,
        DIALOGUE = 3,
        COMPLETE_OBJECTIVE = 4,
        START_OPERATION = 5,
        END_OPERATION = 6
    }
}
