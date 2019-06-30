using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityScript
{
public class firstPersonController : MonoBehaviour
{
    [SerializeField] private AnimationCurve headBobing = new AnimationCurve
    (
        new Keyframe(0f, 0f, 0f, 0f),
        new Keyframe(0f, 0.5f, 0f, 5f),
        new Keyframe(0f, 0f, 0f, 0f),
        new Keyframe(1f, 2.5f, 0f, 0f),
        new Keyframe(0f, 0f, 0f, 0f),
        new Keyframe(0f, 0.5f, 0f, 5f)    
    );
    [SerializeField] [Header("Head Bobbing impact factor")] private float HeadBobbingFactor = 1; 
    [SerializeField] [Header("Speed Factor")] private float speed = 20;
    [SerializeField] [Header("Camera, body")] private GameObject Camera = default(GameObject);
    [SerializeField] private string HorizontalInput = "Horizontal";
    [SerializeField] private string VerticalInput = "Vertical";

    [SerializeField] [Header("Mouse X sensibility")] private float XSensibility = 1f;
    [SerializeField] [Header("Mouse Y sensibility")] private float Ysensibility = 1f;

    [SerializeField] [Header("Min Y value loocked")] private float MinLoocked = -90f;
    [SerializeField] [Header("Max Y value loocked")] private float MaxLoocked = -90f;

    private float HorizontalAxe = 0.0F;
    private float VerticalAxe = 0.0F;

    private Vector2 MouseMovement = default(Vector2);

    private Rigidbody RigidbodyCorps = null;

    private float CameraYOffSet;

    // Start is called before the first frame update
    void Start()
    {
        if (!RigidbodyCorps) RigidbodyCorps = gameObject.GetComponent<Rigidbody>();
        if (!RigidbodyCorps) RigidbodyCorps = gameObject.AddComponent<Rigidbody>();
        CameraYOffSet = Camera.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalAxe = Input.GetAxis(HorizontalInput);
        VerticalAxe = Input.GetAxis(VerticalInput);
        RigidbodyCorps.MovePosition(transform.position + (PLayerMovement() * speed * Time.deltaTime));
        HeadBobbing();
        RotatePLayer();
    }

    void HeadBobbing()
    {
                Camera.transform.localPosition = new Vector3 
                (
                    0,
                    CameraYOffSet + headBobing.Evaluate(Time.time * speed) * HeadBobbingFactor * (VerticalAxe + HorizontalAxe),
                    0
                ); 

    }

    Vector3 PLayerMovement()
    {
        float CameraYRot = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        return new Vector3
                        (
                            Mathf.Rad2Deg * Mathf.Sin(CameraYRot) * VerticalAxe + Mathf.Rad2Deg * Mathf.Sin(CameraYRot + Mathf.PI/2) * HorizontalAxe,
                            0,
                            Mathf.Rad2Deg * Mathf.Cos(CameraYRot) * VerticalAxe + Mathf.Rad2Deg * Mathf.Cos(CameraYRot + Mathf.PI/2) * HorizontalAxe
                        );
    }

    void RotatePLayer() 
    {
         MouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        RigidbodyCorps.MoveRotation(BodyRotation());
        Camera.transform.localRotation = CameraRotation();
    }

    Quaternion BodyRotation()
    {
        var rotation = transform.rotation.eulerAngles;
        return Quaternion.Euler(new Vector3(
            rotation.x,
            rotation.y + MouseMovement.x * XSensibility,
            rotation.z
        ));
    }


    Quaternion CameraRotation()
    {
        var rotation = Camera.transform.localRotation.eulerAngles;
        if ((rotation.x + 540) % 360 - 180 - MouseMovement.y * Ysensibility < MinLoocked)
            return Quaternion.Euler(MinLoocked, 0 , 0);
        else if ((rotation.x + 540) % 360 - 180 - MouseMovement.y * Ysensibility > MaxLoocked)
            return Quaternion.Euler(MaxLoocked, 0 , 0);
        return Quaternion.Euler(rotation.x - MouseMovement.y * Ysensibility, 0 , 0);
    }

}

};