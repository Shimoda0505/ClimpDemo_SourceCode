using UnityEngine;
using UnityEngine.InputSystem;



/// <summary>
/// InputSystemメソッド
/// </summary>
public  struct S_InputData 
{

    //スティック-----------------------------------------------------------------------------------------------------------------------

    //Value
    /// <summary>
    /// Rスティック(Value)
    /// </summary>
    public Vector2 StickRightValue(float deadZone)
    {

        //入力があれば値更新
        if (StickRightBool(deadZone)) { return Gamepad.current.rightStick.ReadValue(); }
        return new Vector2(0, 0);
    }

    /// <summary>
    /// Lスティック(Value)
    public Vector2 StickLeftValue(float deadZone)
    {

        //入力があれば値更新
        if (StickLeftBool(deadZone)) { return Gamepad.current.leftStick.ReadValue(); }
        return new Vector2(0, 0);
    }


    //Bool
    /// <summary>
    /// Rスティック(Bool)
    /// </summary>
    public bool StickRightBool(float deadZone)
    {
        //入力量が一定値以上なら
        if (Mathf.Abs(Gamepad.current.rightStick.ReadValue().x) + Mathf.Abs(Gamepad.current.rightStick.ReadValue().y) >= deadZone) { return true; }

        return false;
    }

    /// <summary>
    /// Lスティック(Bool)
    /// </summary>
    public bool StickLeftBool(float deadZone)
    {
        //入力量が一定値以上なら
        if (Mathf.Abs(Gamepad.current.leftStick.ReadValue().x) + Mathf.Abs(Gamepad.current.leftStick.ReadValue().y) >= deadZone) { return true; }

        return false;
    }


    //入力中
    /// <summary>
    /// Rスティック(入力中)
    /// </summary>
    public bool StickRight()
    {
        if (Gamepad.current.rightStickButton.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Lスティック(入力中)
    /// </summary>
    public bool StickLeft()
    {
        if (Gamepad.current.leftStickButton.isPressed) { return true; }

        return false;
    }


    //入力押
    /// <summary>
    /// Rスティック(入力押)
    /// </summary>
    public bool StickRightDown()
    {
        if (Gamepad.current.rightStickButton.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Lスティック(入力押)
    /// </summary>
    public bool StickLeftDown()
    {
        if (Gamepad.current.leftStickButton.wasPressedThisFrame) { return true; }

        return false;
    }


    //入力離
    /// <summary>
    /// Rスティック(入力離)
    /// </summary>
    public bool StickRightUp()
    {
        if (Gamepad.current.rightStickButton.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Lスティック(入力離)
    /// </summary>
    public bool StickLeftUp()
    {
        if (Gamepad.current.leftStickButton.wasReleasedThisFrame) { return true; }

        return false;
    }



    //ボタン----------------------------------------------------------------------------------------------------------------------------

    //入力中
    /// <summary>
    /// Yボタン(入力中)
    /// </summary>
    public bool ButtonNorth()
    {
        if (Gamepad.current.buttonNorth.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Aボタン(入力中)
    /// </summary>
    public bool ButtonSouth()
    {
        if (Gamepad.current.buttonSouth.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Bボタン(入力中)
    /// </summary>
    public bool ButtonEast()
    {
        if (Gamepad.current.buttonEast.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Xボタン(入力中)
    /// </summary>
    public bool ButtonWest()
    {
        if (Gamepad.current.buttonWest.isPressed) { return true; }

        return false;
    }


    //入力押
    /// <summary>
    /// Yボタン(入力押)
    /// </summary>
    public bool ButtonNorthDown()
    {
        if (Gamepad.current.buttonNorth.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Aボタン(入力押)
    /// </summary>
    public bool ButtonSouthDown()
    {
        if (Gamepad.current.buttonSouth.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Bボタン(入力押)
    /// </summary>
    public bool ButtonEastDown()
    {
        if (Gamepad.current.buttonEast.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Xボタン(入力押)
    /// </summary>
    public bool ButtonWestDown()
    {
        if (Gamepad.current.buttonWest.wasPressedThisFrame) { return true; }

        return false;
    }


    //入力離
    /// <summary>
    /// Yボタン(入力離)
    /// </summary>
    public bool ButtonNorthUp()
    {
        if (Gamepad.current.buttonNorth.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Xボタン(入力離)
    /// </summary>
    public bool ButtonSouthUp()
    {
        if (Gamepad.current.buttonSouth.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Bボタン(入力離)
    /// </summary>
    public bool ButtonEastUp()
    {
        if (Gamepad.current.buttonEast.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Aボタン(入力離)
    /// </summary>
    public bool ButtonWestUp()
    {
        if (Gamepad.current.buttonWest.wasReleasedThisFrame) { return true; }

        return false;
    }



    //十字----------------------------------------------------------------------------------------------------------------------------

    //Value
    /// <summary>
    /// Rスティック(Value)
    /// </summary>
    public Vector2 DpadValue(float deadZone)
    {
        //入力の初期値
        Vector2 value = new Vector2(0, 0);

        //入力量が一定値以上なら
        if (Mathf.Abs(Gamepad.current.dpad.ReadValue().x) + Mathf.Abs(Gamepad.current.dpad.ReadValue().y) >= deadZone)
        {
            //値更新
            value = Gamepad.current.dpad.ReadValue();
        }

        return value;
    }


    //Bool
    /// <summary>
    /// Rスティック(Bool)
    /// </summary>
    public bool DpadBool(float deadZone)
    {
        //入力量が一定値以上なら
        if (Mathf.Abs(Gamepad.current.dpad.ReadValue().x) + Mathf.Abs(Gamepad.current.dpad.ReadValue().y) >= deadZone) { return true; }

        return false;
    }


    //入力中
    /// <summary>
    /// Right十字(入力中)
    /// </summary>
    public bool DpadRight()
    {
        if (Gamepad.current.dpad.right.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Left十字(入力中)
    /// </summary>
    public bool DpadLeft()
    {
        if (Gamepad.current.dpad.left.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Up十字(入力中)
    /// </summary>
    public bool DpadUp()
    {
        if (Gamepad.current.dpad.up.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Down十字(入力中)
    /// </summary>
    public bool DpadDown()
    {
        if (Gamepad.current.dpad.down.isPressed) { return true; }

        return false;
    }


    //入力押
    /// <summary>
    /// Right十字(入力押)
    /// </summary>
    public bool DpadRightDown()
    {
        if (Gamepad.current.dpad.right.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Left十字(入力押)
    /// </summary>
    public bool DpadLeftDown()
    {
        if (Gamepad.current.dpad.left.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Up十字(入力押)
    /// </summary>
    public bool DpadUpDown()
    {
        if (Gamepad.current.dpad.up.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Down十字(入力押)
    /// </summary>
    public bool DpadDownDown()
    {
        if (Gamepad.current.dpad.down.wasPressedThisFrame) { return true; }

        return false;
    }


    //入力離
    /// <summary>
    /// Right十字(入力離)
    /// </summary>
    public bool DpadRightUp()
    {
        if (Gamepad.current.dpad.right.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Left十字(入力離)
    /// </summary>
    public bool DpadLeftUp()
    {
        if (Gamepad.current.dpad.left.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Up十字(入力離)
    /// </summary>
    public bool DpadUpUp()
    {
        if (Gamepad.current.dpad.up.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Down十字(入力離)
    /// </summary>
    public bool DpadDownUp()
    {
        if (Gamepad.current.dpad.down.wasReleasedThisFrame) { return true; }

        return false;
    }



    //ショルダー(上)----------------------------------------------------------------------------------------------------------------------------

    //入力量
    /// <summary>
    /// Rショルダー(入力量)
    /// </summary>
    public float ShoulderRightValue(float deadZone)
    {
        //入力の初期値
        float value = 0f;

        if (Gamepad.current.rightShoulder.ReadValue() >= deadZone)
        {
            //値更新
            value = Gamepad.current.rightShoulder.ReadValue();
        }

        return value;
    }

    /// <summary>
    /// Lショルダー(入力量)
    /// </summary>
    public float ShoulderLeftValue(float deadZone)
    {
        //入力の初期値
        float value = 0f;

        if (Gamepad.current.leftShoulder.ReadValue() >= deadZone)
        {
            //値更新
            value = Gamepad.current.leftShoulder.ReadValue();
        }

        return value;
    }


    //入力中
    /// <summary>
    /// Rショルダー(入力中)
    /// </summary>
    public bool ShoulderRight()
    {
        if (Gamepad.current.rightShoulder.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Lショルダー(入力中)
    /// </summary>
    public bool ShoulderLeft()
    {
        if (Gamepad.current.leftShoulder.isPressed) { return true; }

        return false;
    }


    //入力押
    /// <summary>
    /// Rショルダー(入力押)
    /// </summary>
    public bool ShoulderRightDown()
    {
        if (Gamepad.current.rightShoulder.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Lショルダー(入力押)
    /// </summary>
    public bool ShoulderLeftDown()
    {
        if (Gamepad.current.leftShoulder.wasPressedThisFrame) { return true; }

        return false;
    }


    //入力離
    /// <summary>
    /// Rショルダー(入力離)
    /// </summary>
    public bool ShoulderRightUp()
    {
        if (Gamepad.current.rightShoulder.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Lショルダー(入力離)
    /// </summary>
    public bool ShoulderLeftUp()
    {
        if (Gamepad.current.leftShoulder.wasReleasedThisFrame) { return true; }

        return false;
    }



    //トリガー(下)----------------------------------------------------------------------------------------------------------------------------

    //入力量
    /// <summary>
    /// Rトリガー(入力量)
    /// </summary>
    public float TriggerRightValue(float deadZone)
    {
        //入力の初期値
        float value = 0f;

        if (Gamepad.current.leftTrigger.ReadValue() >= deadZone)
        {
            //値更新
            value = Gamepad.current.rightShoulder.ReadValue();
        }

        return value;
    }

    /// <summary>
    /// Lトリガー(入力量)
    /// </summary>
    public float TriggerLeftValue(float deadZone)
    {
        //入力の初期値
        float value = 0f;

        if (Gamepad.current.leftTrigger.ReadValue() >= deadZone)
        {
            //値更新
            value = Gamepad.current.leftShoulder.ReadValue();
        }

        return value;
    }


    //入力中
    /// <summary>
    /// Rトリガー(入力中)
    /// </summary>
    public bool TriggerRight()
    {
        if (Gamepad.current.leftTrigger.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Lトリガー(入力中)
    /// </summary>
    public bool TriggerLeft()
    {
        if (Gamepad.current.leftTrigger.isPressed) { return true; }

        return false;
    }


    //入力押
    /// <summary>
    /// Rトリガー(入力押)
    /// </summary>
    public bool TriggerRightDown()
    {
        if (Gamepad.current.leftTrigger.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Lトリガー(入力押)
    /// </summary>
    public bool TriggerLeftDown()
    {
        if (Gamepad.current.leftTrigger.wasPressedThisFrame) { return true; }

        return false;
    }


    //入力離
    /// <summary>
    /// Rトリガー(入力離)
    /// </summary>
    public bool TriggerRightUp()
    {
        if (Gamepad.current.leftTrigger.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Lトリガー(入力離)
    /// </summary>
    public bool TriggerLeftUp()
    {
        if (Gamepad.current.leftTrigger.wasReleasedThisFrame) { return true; }

        return false;
    }



    //中央ボタン----------------------------------------------------------------------------------------------------------------------------

    //入力中
    /// <summary>
    /// Start(入力中)
    /// </summary>
    public bool ButtonStart()
    {
        if (Gamepad.current.startButton.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Select(入力中)
    /// </summary>
    public bool ButtonSelect()
    {
        if (Gamepad.current.selectButton.isPressed) { return true; }

        return false;
    }


    //入力押
    /// <summary>
    /// Start(入力押)
    /// </summary>
    public bool ButtonStartDown()
    {
        if (Gamepad.current.startButton.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Select(入力押)
    /// </summary>
    public bool ButtonSelectDown()
    {
        if (Gamepad.current.selectButton.wasPressedThisFrame) { return true; }

        return false;
    }


    //入力離
    /// <summary>
    /// Start(入力離)
    /// </summary>
    public bool ButtonStartUp()
    {
        if (Gamepad.current.startButton.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Select(入力離)
    /// </summary>
    public bool ButtonSelectUp()
    {
        if (Gamepad.current.selectButton.wasReleasedThisFrame) { return true; }

        return false;
    }



    //その他----------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// コントローラー振動
    /// </summary>
    public void Vibration(float LV, float RV) { Gamepad.current.SetMotorSpeeds(LV, RV); }

    /// <summary>
    /// 全てのボタン入力
    /// </summary>
    /// <returns></returns>
    public bool AnyButton()
    {
        if (Gamepad.current.startButton.wasPressedThisFrame || Gamepad.current.selectButton.wasPressedThisFrame || Gamepad.current.leftTrigger.wasPressedThisFrame || Gamepad.current.rightTrigger.wasPressedThisFrame ||
            Gamepad.current.buttonNorth.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame || Gamepad.current.buttonWest.wasPressedThisFrame ||
            Gamepad.current.rightStickButton.wasPressedThisFrame || Gamepad.current.leftStickButton.wasPressedThisFrame || Gamepad.current.leftShoulder.wasPressedThisFrame || Gamepad.current.rightShoulder.wasPressedThisFrame)
        {
            return true;
        }

        return false;
    }

}

