using System;
using UnityEngine;

public class Player_Input : MonoBehaviour, IInput
{
    public Action<Vector2, UseSkill> OnMovementInput { get; set; }

    public Action<Vector3> OnMovementDirectionInput { get; set; }
    public Action OnAttackInput { get; set; }
    public Action<Vector3> OnAttackDirection { get; set; }
    public Action OnDodge { get; set; }
    public Action QSkill { get; set; }

    [SerializeField] Vector2 LastInput = Vector2.zero;
    [SerializeField] Vector2 AbsInput = Vector2.zero;

    Transform CameraTransform;
    public bool test = false;

    ISkill skill;

    private void Start()
    {
        //마우스 커서를 게임 중앙좌표에 고정시키고 커서를 숨김
        Cursor.lockState = CursorLockMode.Locked;
        CameraTransform = Camera.main.transform;
        skill = GetComponent<ISkill>();
    }


    private void Update()
    {
        Vector2 input = GetMovementInput();
        GetMovementDirection(input);
        GetAttackInput();
        GetAttackDirection();
        GetQSkill();
    }

    #region 이동
    Vector2 GetMovementInput()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        OnMovementInput?.Invoke(input, skill.isSkill());
        return input;
    }

    Vector3 GetDirection()
    {
        var cameraFowardDirection = CameraTransform.forward;
        Vector3 directionToMoveIn = Vector3.Scale(cameraFowardDirection, (Vector3.right + Vector3.forward));
        //Debug.DrawRay(Camera.main.transform.position, cameraFowardDirection * 10, Color.red);
        //Debug.DrawRay(Camera.main.transform.position, directionToMoveIn * 10, Color.blue);
        return directionToMoveIn;
    }

    void GetMovementDirection(Vector2 input)
    {
        AbsInput.x = Math.Abs(input.x);
        AbsInput.y = Math.Abs(input.y);
        Vector3 directionToMoveIn = GetDirection();
        if(input == Vector2.zero && LastInput == Vector2.zero)
        {

        }
        else if (input.y != 0&&( Math.Abs(LastInput.y) < AbsInput.y || AbsInput.y == 1))
        {
            //앞 뒤
            directionToMoveIn *= (input.y > 0) ? 1 : -1;

            //대각선 이동중인지
            if ((input.x != 0 && Math.Abs(LastInput.x) < AbsInput.x) || AbsInput.x == 1)
            {
                directionToMoveIn = directionToMoveIn + (input.x > 0 ? 1 : -1) * CameraTransform.right;
              
            }
        }
        else if ((Math.Abs(LastInput.y) > input.y || input.y == 0 )&& input.x != 0)
        {//좌 우
            directionToMoveIn = (input.x > 0 ? -1 : 1) * Vector3.Cross(directionToMoveIn, Vector3.up); 
        }
        else// 앞 뒤 감소중일 때
            directionToMoveIn *= (LastInput.y > 0) ? 1 : -1;
        LastInput = input;

        OnMovementDirectionInput?.Invoke(directionToMoveIn.normalized);

    }
    #endregion

    #region Skill
    void GetAttackInput()
    {
        if(Input.GetMouseButtonDown(0))
            OnAttackInput?.Invoke();
    }

    void GetAttackDirection()
    {
        if (Input.GetKeyDown(KeyCode.C))
            OnDodge?.Invoke();
    }

    void GetQSkill()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            QSkill?.Invoke();
    }



    #endregion

}
