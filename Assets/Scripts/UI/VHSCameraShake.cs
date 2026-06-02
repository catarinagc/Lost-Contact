using UnityEngine;

public class VHSCameraShake : MonoBehaviour
{
    [Header("Normal VHS Shake")]
    [SerializeField] private float normalHorizontalShake = 0.004f;
    [SerializeField] private float normalVerticalShake = 0.003f;
    [SerializeField] private float normalSpeed = 8f;

    [Header("Running VHS Shake")]
    [SerializeField] private float runningHorizontalShake = 0.04f;
    [SerializeField] private float runningVerticalShake = 0.06f;
    [SerializeField] private float runningSpeed = 18f;

    [Header("Running Head Bob")]
    [SerializeField] private float bobAmount = 0.08f;
    [SerializeField] private float bobSpeed = 14f;

    [Header("Running Detection")]
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    [Header("Smooth Transition")]
    [SerializeField] private float transitionSpeed = 10f;

    private Vector3 baseLocalPosition;

    private float currentHorizontalShake;
    private float currentVerticalShake;
    private float currentSpeed;
    private float currentBobAmount;

    void Start()
    {
        baseLocalPosition = transform.localPosition;
    }

    void OnDisable()
    {
        transform.localPosition = baseLocalPosition;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
        bool isRunning = Input.GetKey(runKey) && isMoving;

        float targetHorizontalShake = isRunning ? runningHorizontalShake : normalHorizontalShake;
        float targetVerticalShake = isRunning ? runningVerticalShake : normalVerticalShake;
        float targetSpeed = isRunning ? runningSpeed : normalSpeed;
        float targetBobAmount = isRunning ? bobAmount : 0f;

        currentHorizontalShake = Mathf.Lerp(currentHorizontalShake, targetHorizontalShake, Time.deltaTime * transitionSpeed);
        currentVerticalShake = Mathf.Lerp(currentVerticalShake, targetVerticalShake, Time.deltaTime * transitionSpeed);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * transitionSpeed);
        currentBobAmount = Mathf.Lerp(currentBobAmount, targetBobAmount, Time.deltaTime * transitionSpeed);

        float noiseX = Mathf.PerlinNoise(Time.time * currentSpeed, 0f) - 0.5f;
        float noiseY = Mathf.PerlinNoise(0f, Time.time * currentSpeed) - 0.5f;

        float horizontalOffset = noiseX * currentHorizontalShake;
        float verticalOffset = noiseY * currentVerticalShake;
        float bob = Mathf.Sin(Time.time * bobSpeed) * currentBobAmount;

        Vector3 finalOffset = new Vector3(
            horizontalOffset,
            verticalOffset + bob,
            0f
        );

        transform.localPosition = baseLocalPosition + finalOffset;
    }
}
