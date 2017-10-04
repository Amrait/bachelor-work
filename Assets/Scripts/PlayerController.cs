using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerController : MonoBehaviour {
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lookSensitivity = 5f;
    [SerializeField] private float thrusterForce = 1500f;
    [SerializeField] private float thrusterFuelBurnSpeed = 1f;
    [SerializeField] private float thrusterFuelRegenSpeed = 0.3f;
    [SerializeField] private float thrusterFuelAmount = 1f;
    
    public float getThrusterFuelAmount(){
        return thrusterFuelAmount;
    }
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    [SerializeField] private LayerMask environmentMask;

    [Header("Spring settings:")]
    [SerializeField] private float jointSpring = 20f;
    [SerializeField] private float jointMaxForce = 40f;


    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();

        SetJointSettings(jointSpring);
    }
    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        //Встановлення точки притягнення для того, аби правильно притягуватись до об'єктів
        RaycastHit _springHit;
        if (Physics.Raycast (transform.position, Vector3.down, out _springHit, 100f, environmentMask)){
            joint.targetPosition = new Vector3(0f,-_springHit.point.y,0f);
        }
        else{
            joint.targetPosition = new Vector3(0f,-_springHit.point.y,0f);
        }
        //Обрахунок швидкості як 3D вектору
        float _xMov = Input.GetAxisRaw("Horizontal");
        float _zMov = Input.GetAxisRaw("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;
        //Фінальний вектор руху з нормалізацією, що робить рух сталим без прискорення
        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed;
        //Застосування пересування
        motor.Move(_velocity);

        //Прорахунок поворотів самого гравця
        float _yRot = Input.GetAxisRaw("Mouse X");
        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;
        //Застосування поворотів гравця
        motor.Rotate(_rotation);

        //Прорахунок поворотів камери
        float _xRot = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRot * lookSensitivity;
        //Застосування поворотів камери
        motor.RotateCamera(_cameraRotationX);
        //Застосування сили польоту
        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton("Jump") && thrusterFuelAmount>0)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;
            if (thrusterFuelAmount > 0.01f) {
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSettings(0f);
            }

        } else
        {
            SetJointSettings(jointSpring);
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
        }
        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount,0f,1f);
        motor.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }
}

