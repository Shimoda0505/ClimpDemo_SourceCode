using UnityEngine;
using UnityEngine.InputSystem;



/// <summary>
/// InputSystem���\�b�h
/// </summary>
public  struct S_InputData 
{

    //�X�e�B�b�N-----------------------------------------------------------------------------------------------------------------------

    //Value
    /// <summary>
    /// R�X�e�B�b�N(Value)
    /// </summary>
    public Vector2 StickRightValue(float deadZone)
    {

        //���͂�����Βl�X�V
        if (StickRightBool(deadZone)) { return Gamepad.current.rightStick.ReadValue(); }
        return new Vector2(0, 0);
    }

    /// <summary>
    /// L�X�e�B�b�N(Value)
    public Vector2 StickLeftValue(float deadZone)
    {

        //���͂�����Βl�X�V
        if (StickLeftBool(deadZone)) { return Gamepad.current.leftStick.ReadValue(); }
        return new Vector2(0, 0);
    }


    //Bool
    /// <summary>
    /// R�X�e�B�b�N(Bool)
    /// </summary>
    public bool StickRightBool(float deadZone)
    {
        //���͗ʂ����l�ȏ�Ȃ�
        if (Mathf.Abs(Gamepad.current.rightStick.ReadValue().x) + Mathf.Abs(Gamepad.current.rightStick.ReadValue().y) >= deadZone) { return true; }

        return false;
    }

    /// <summary>
    /// L�X�e�B�b�N(Bool)
    /// </summary>
    public bool StickLeftBool(float deadZone)
    {
        //���͗ʂ����l�ȏ�Ȃ�
        if (Mathf.Abs(Gamepad.current.leftStick.ReadValue().x) + Mathf.Abs(Gamepad.current.leftStick.ReadValue().y) >= deadZone) { return true; }

        return false;
    }


    //���͒�
    /// <summary>
    /// R�X�e�B�b�N(���͒�)
    /// </summary>
    public bool StickRight()
    {
        if (Gamepad.current.rightStickButton.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// L�X�e�B�b�N(���͒�)
    /// </summary>
    public bool StickLeft()
    {
        if (Gamepad.current.leftStickButton.isPressed) { return true; }

        return false;
    }


    //���͉�
    /// <summary>
    /// R�X�e�B�b�N(���͉�)
    /// </summary>
    public bool StickRightDown()
    {
        if (Gamepad.current.rightStickButton.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// L�X�e�B�b�N(���͉�)
    /// </summary>
    public bool StickLeftDown()
    {
        if (Gamepad.current.leftStickButton.wasPressedThisFrame) { return true; }

        return false;
    }


    //���͗�
    /// <summary>
    /// R�X�e�B�b�N(���͗�)
    /// </summary>
    public bool StickRightUp()
    {
        if (Gamepad.current.rightStickButton.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// L�X�e�B�b�N(���͗�)
    /// </summary>
    public bool StickLeftUp()
    {
        if (Gamepad.current.leftStickButton.wasReleasedThisFrame) { return true; }

        return false;
    }



    //�{�^��----------------------------------------------------------------------------------------------------------------------------

    //���͒�
    /// <summary>
    /// Y�{�^��(���͒�)
    /// </summary>
    public bool ButtonNorth()
    {
        if (Gamepad.current.buttonNorth.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// A�{�^��(���͒�)
    /// </summary>
    public bool ButtonSouth()
    {
        if (Gamepad.current.buttonSouth.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// B�{�^��(���͒�)
    /// </summary>
    public bool ButtonEast()
    {
        if (Gamepad.current.buttonEast.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// X�{�^��(���͒�)
    /// </summary>
    public bool ButtonWest()
    {
        if (Gamepad.current.buttonWest.isPressed) { return true; }

        return false;
    }


    //���͉�
    /// <summary>
    /// Y�{�^��(���͉�)
    /// </summary>
    public bool ButtonNorthDown()
    {
        if (Gamepad.current.buttonNorth.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// A�{�^��(���͉�)
    /// </summary>
    public bool ButtonSouthDown()
    {
        if (Gamepad.current.buttonSouth.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// B�{�^��(���͉�)
    /// </summary>
    public bool ButtonEastDown()
    {
        if (Gamepad.current.buttonEast.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// X�{�^��(���͉�)
    /// </summary>
    public bool ButtonWestDown()
    {
        if (Gamepad.current.buttonWest.wasPressedThisFrame) { return true; }

        return false;
    }


    //���͗�
    /// <summary>
    /// Y�{�^��(���͗�)
    /// </summary>
    public bool ButtonNorthUp()
    {
        if (Gamepad.current.buttonNorth.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// X�{�^��(���͗�)
    /// </summary>
    public bool ButtonSouthUp()
    {
        if (Gamepad.current.buttonSouth.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// B�{�^��(���͗�)
    /// </summary>
    public bool ButtonEastUp()
    {
        if (Gamepad.current.buttonEast.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// A�{�^��(���͗�)
    /// </summary>
    public bool ButtonWestUp()
    {
        if (Gamepad.current.buttonWest.wasReleasedThisFrame) { return true; }

        return false;
    }



    //�\��----------------------------------------------------------------------------------------------------------------------------

    //Value
    /// <summary>
    /// R�X�e�B�b�N(Value)
    /// </summary>
    public Vector2 DpadValue(float deadZone)
    {
        //���͂̏����l
        Vector2 value = new Vector2(0, 0);

        //���͗ʂ����l�ȏ�Ȃ�
        if (Mathf.Abs(Gamepad.current.dpad.ReadValue().x) + Mathf.Abs(Gamepad.current.dpad.ReadValue().y) >= deadZone)
        {
            //�l�X�V
            value = Gamepad.current.dpad.ReadValue();
        }

        return value;
    }


    //Bool
    /// <summary>
    /// R�X�e�B�b�N(Bool)
    /// </summary>
    public bool DpadBool(float deadZone)
    {
        //���͗ʂ����l�ȏ�Ȃ�
        if (Mathf.Abs(Gamepad.current.dpad.ReadValue().x) + Mathf.Abs(Gamepad.current.dpad.ReadValue().y) >= deadZone) { return true; }

        return false;
    }


    //���͒�
    /// <summary>
    /// Right�\��(���͒�)
    /// </summary>
    public bool DpadRight()
    {
        if (Gamepad.current.dpad.right.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Left�\��(���͒�)
    /// </summary>
    public bool DpadLeft()
    {
        if (Gamepad.current.dpad.left.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Up�\��(���͒�)
    /// </summary>
    public bool DpadUp()
    {
        if (Gamepad.current.dpad.up.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Down�\��(���͒�)
    /// </summary>
    public bool DpadDown()
    {
        if (Gamepad.current.dpad.down.isPressed) { return true; }

        return false;
    }


    //���͉�
    /// <summary>
    /// Right�\��(���͉�)
    /// </summary>
    public bool DpadRightDown()
    {
        if (Gamepad.current.dpad.right.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Left�\��(���͉�)
    /// </summary>
    public bool DpadLeftDown()
    {
        if (Gamepad.current.dpad.left.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Up�\��(���͉�)
    /// </summary>
    public bool DpadUpDown()
    {
        if (Gamepad.current.dpad.up.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Down�\��(���͉�)
    /// </summary>
    public bool DpadDownDown()
    {
        if (Gamepad.current.dpad.down.wasPressedThisFrame) { return true; }

        return false;
    }


    //���͗�
    /// <summary>
    /// Right�\��(���͗�)
    /// </summary>
    public bool DpadRightUp()
    {
        if (Gamepad.current.dpad.right.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Left�\��(���͗�)
    /// </summary>
    public bool DpadLeftUp()
    {
        if (Gamepad.current.dpad.left.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Up�\��(���͗�)
    /// </summary>
    public bool DpadUpUp()
    {
        if (Gamepad.current.dpad.up.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Down�\��(���͗�)
    /// </summary>
    public bool DpadDownUp()
    {
        if (Gamepad.current.dpad.down.wasReleasedThisFrame) { return true; }

        return false;
    }



    //�V�����_�[(��)----------------------------------------------------------------------------------------------------------------------------

    //���͗�
    /// <summary>
    /// R�V�����_�[(���͗�)
    /// </summary>
    public float ShoulderRightValue(float deadZone)
    {
        //���͂̏����l
        float value = 0f;

        if (Gamepad.current.rightShoulder.ReadValue() >= deadZone)
        {
            //�l�X�V
            value = Gamepad.current.rightShoulder.ReadValue();
        }

        return value;
    }

    /// <summary>
    /// L�V�����_�[(���͗�)
    /// </summary>
    public float ShoulderLeftValue(float deadZone)
    {
        //���͂̏����l
        float value = 0f;

        if (Gamepad.current.leftShoulder.ReadValue() >= deadZone)
        {
            //�l�X�V
            value = Gamepad.current.leftShoulder.ReadValue();
        }

        return value;
    }


    //���͒�
    /// <summary>
    /// R�V�����_�[(���͒�)
    /// </summary>
    public bool ShoulderRight()
    {
        if (Gamepad.current.rightShoulder.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// L�V�����_�[(���͒�)
    /// </summary>
    public bool ShoulderLeft()
    {
        if (Gamepad.current.leftShoulder.isPressed) { return true; }

        return false;
    }


    //���͉�
    /// <summary>
    /// R�V�����_�[(���͉�)
    /// </summary>
    public bool ShoulderRightDown()
    {
        if (Gamepad.current.rightShoulder.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// L�V�����_�[(���͉�)
    /// </summary>
    public bool ShoulderLeftDown()
    {
        if (Gamepad.current.leftShoulder.wasPressedThisFrame) { return true; }

        return false;
    }


    //���͗�
    /// <summary>
    /// R�V�����_�[(���͗�)
    /// </summary>
    public bool ShoulderRightUp()
    {
        if (Gamepad.current.rightShoulder.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// L�V�����_�[(���͗�)
    /// </summary>
    public bool ShoulderLeftUp()
    {
        if (Gamepad.current.leftShoulder.wasReleasedThisFrame) { return true; }

        return false;
    }



    //�g���K�[(��)----------------------------------------------------------------------------------------------------------------------------

    //���͗�
    /// <summary>
    /// R�g���K�[(���͗�)
    /// </summary>
    public float TriggerRightValue(float deadZone)
    {
        //���͂̏����l
        float value = 0f;

        if (Gamepad.current.leftTrigger.ReadValue() >= deadZone)
        {
            //�l�X�V
            value = Gamepad.current.rightShoulder.ReadValue();
        }

        return value;
    }

    /// <summary>
    /// L�g���K�[(���͗�)
    /// </summary>
    public float TriggerLeftValue(float deadZone)
    {
        //���͂̏����l
        float value = 0f;

        if (Gamepad.current.leftTrigger.ReadValue() >= deadZone)
        {
            //�l�X�V
            value = Gamepad.current.leftShoulder.ReadValue();
        }

        return value;
    }


    //���͒�
    /// <summary>
    /// R�g���K�[(���͒�)
    /// </summary>
    public bool TriggerRight()
    {
        if (Gamepad.current.leftTrigger.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// L�g���K�[(���͒�)
    /// </summary>
    public bool TriggerLeft()
    {
        if (Gamepad.current.leftTrigger.isPressed) { return true; }

        return false;
    }


    //���͉�
    /// <summary>
    /// R�g���K�[(���͉�)
    /// </summary>
    public bool TriggerRightDown()
    {
        if (Gamepad.current.leftTrigger.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// L�g���K�[(���͉�)
    /// </summary>
    public bool TriggerLeftDown()
    {
        if (Gamepad.current.leftTrigger.wasPressedThisFrame) { return true; }

        return false;
    }


    //���͗�
    /// <summary>
    /// R�g���K�[(���͗�)
    /// </summary>
    public bool TriggerRightUp()
    {
        if (Gamepad.current.leftTrigger.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// L�g���K�[(���͗�)
    /// </summary>
    public bool TriggerLeftUp()
    {
        if (Gamepad.current.leftTrigger.wasReleasedThisFrame) { return true; }

        return false;
    }



    //�����{�^��----------------------------------------------------------------------------------------------------------------------------

    //���͒�
    /// <summary>
    /// Start(���͒�)
    /// </summary>
    public bool ButtonStart()
    {
        if (Gamepad.current.startButton.isPressed) { return true; }

        return false;
    }

    /// <summary>
    /// Select(���͒�)
    /// </summary>
    public bool ButtonSelect()
    {
        if (Gamepad.current.selectButton.isPressed) { return true; }

        return false;
    }


    //���͉�
    /// <summary>
    /// Start(���͉�)
    /// </summary>
    public bool ButtonStartDown()
    {
        if (Gamepad.current.startButton.wasPressedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Select(���͉�)
    /// </summary>
    public bool ButtonSelectDown()
    {
        if (Gamepad.current.selectButton.wasPressedThisFrame) { return true; }

        return false;
    }


    //���͗�
    /// <summary>
    /// Start(���͗�)
    /// </summary>
    public bool ButtonStartUp()
    {
        if (Gamepad.current.startButton.wasReleasedThisFrame) { return true; }

        return false;
    }

    /// <summary>
    /// Select(���͗�)
    /// </summary>
    public bool ButtonSelectUp()
    {
        if (Gamepad.current.selectButton.wasReleasedThisFrame) { return true; }

        return false;
    }



    //���̑�----------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// �R���g���[���[�U��
    /// </summary>
    public void Vibration(float LV, float RV) { Gamepad.current.SetMotorSpeeds(LV, RV); }

    /// <summary>
    /// �S�Ẵ{�^������
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

