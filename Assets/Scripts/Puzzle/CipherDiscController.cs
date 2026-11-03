using System.Collections;
using UnityEngine;

public class CipherDiscController : MonoBehaviour
{
    [Header("Rotating Rings")]
    [SerializeField] private Transform outerRing;
    [SerializeField] private Transform middleRing;
    [SerializeField] private Transform innerRing;

    [Header("Selection Highlights")]
    [SerializeField] private GameObject outerHighlight;
    [SerializeField] private GameObject middleHighlight;
    [SerializeField] private GameObject innerHighlight;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationStep = 45f;
    [SerializeField] private float rotationDuration = 0.25f;

    private Transform[] rings;
    private GameObject[] highlights;

    private int selectedRing = 0;
    private bool isRotating;

    private void Start()
    {
        rings = new Transform[]
        {
            outerRing,
            middleRing,
            innerRing
        };

        highlights = new GameObject[]
        {
            outerHighlight,
            middleHighlight,
            innerHighlight
        };

        SelectRing(0);
    }

    public void SelectRing(int index)
    {
        if (index < 0 || index >= rings.Length)
            return;

        selectedRing = index;

        for (int i = 0; i < highlights.Length; i++)
        {
            if (highlights[i] != null)
            {
                highlights[i].SetActive(i == selectedRing);
            }
        }
    }

    public void RotateClockwise()
    {
        RotateSelectedRing(-rotationStep);
    }

    public void RotateCounterClockwise()
    {
        RotateSelectedRing(rotationStep);
    }

    private void RotateSelectedRing(float degrees)
    {
        if (isRotating || rings[selectedRing] == null)
            return;

        StartCoroutine(AnimateRotation(rings[selectedRing], degrees));
    }

    private IEnumerator AnimateRotation(Transform ring, float degrees)
    {
        isRotating = true;

        float startAngle = ring.localEulerAngles.z;
        float targetAngle = startAngle + degrees;

        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / rotationDuration);

            // Ease-out: starts quickly and slows toward the end.
            t = 1f - Mathf.Pow(1f - t, 3f);

            float angle = Mathf.Lerp(startAngle, targetAngle, t);

            ring.localRotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        ring.localRotation = Quaternion.Euler(0f, 0f, targetAngle);

        isRotating = false;
    }

    private void Update()
    {
        // Temporary keyboard controls.

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectRing(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectRing(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectRing(2);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            RotateCounterClockwise();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            RotateClockwise();
    }
}