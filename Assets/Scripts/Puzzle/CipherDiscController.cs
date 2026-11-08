
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("Solution Angles")]
    [SerializeField] private float outerSolutionAngle = 225f;
    [SerializeField] private float middleSolutionAngle = 270f;
    [SerializeField] private float innerSolutionAngle = 225f;

    [Header("Paper Dispensing")]
    [SerializeField] private Transform cluePaper;
    [SerializeField] private float dispenseDistance = 2f;
    [SerializeField] private float dispenseDuration = 1.5f;

    [Header("Puzzle Completion")]
    [SerializeField] private UnityEvent onPuzzleSolved;

    private Transform[] rings;
    private GameObject[] highlights;

    private int selectedRing = 0;
    private bool isRotating;
    private bool isSolved;

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
        if (isSolved || isRotating ||
            index < 0 || index >= rings.Length)
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
        if (isSolved || isRotating ||
            rings[selectedRing] == null)
            return;
        // Play the cipher disc rotation sound.
        SoundEffectManager.Play("CipherDiscRotate");

        StartCoroutine(
            AnimateRotation(rings[selectedRing], degrees)
        );
    }

    private IEnumerator AnimateRotation(
        Transform ring,
        float degrees)
    {
        isRotating = true;

        float startAngle = ring.localEulerAngles.z;
        float targetAngle = startAngle + degrees;

        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / rotationDuration
            );

            // Ease-out rotation.
            t = 1f - Mathf.Pow(1f - t, 3f);

            float angle = Mathf.Lerp(
                startAngle,
                targetAngle,
                t
            );

            ring.localRotation =
                Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        ring.localRotation =
            Quaternion.Euler(0f, 0f, targetAngle);

        isRotating = false;

        CheckSolution();
    }

    private void CheckSolution()
    {
        if (isSolved)
            return;


        bool outerCorrect = IsAngleCorrect(
            outerRing.localEulerAngles.z,
            outerSolutionAngle
        );

        bool middleCorrect = IsAngleCorrect(
            middleRing.localEulerAngles.z,
            middleSolutionAngle
        );

        bool innerCorrect = IsAngleCorrect(
            innerRing.localEulerAngles.z,
            innerSolutionAngle
        );

        if (outerCorrect && middleCorrect && innerCorrect)
        {
            SolvePuzzle();
        }
    }

    private bool IsAngleCorrect(
        float currentAngle,
        float targetAngle)
    {
        return Mathf.Abs(
            Mathf.DeltaAngle(currentAngle, targetAngle)
        ) < 1f;
    }

    private void SolvePuzzle()
    {
        isSolved = true;

        // Hide all selection highlights.
        foreach (GameObject highlight in highlights)
        {
            if (highlight != null)
            {
                highlight.SetActive(false);
            }
        }

        Debug.Log("Cipher Disc Puzzle Solved!");

        // Dispense the paper containing the bridge location.
        if (cluePaper != null)
        {
            StartCoroutine(DispensePaper());
        }
        else
        {
            // Still trigger completion if no paper is assigned.
            onPuzzleSolved?.Invoke();
        }
    }

    private IEnumerator DispensePaper()
    {
        cluePaper.gameObject.SetActive(true);
        Vector3 startPosition = cluePaper.localPosition;
        Vector3 endPosition =
            startPosition + Vector3.down * dispenseDistance;

        float elapsed = 0f;

        while (elapsed < dispenseDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / dispenseDuration
            );

            // Ease-out movement.
            t = 1f - Mathf.Pow(1f - t, 3f);

            cluePaper.localPosition = Vector3.Lerp(
                startPosition,
                endPosition,
                t
            );

            yield return null;
        }

        cluePaper.localPosition = endPosition;

        Debug.Log("Clue paper fully dispensed.");

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.CompleteCipherPuzzle();
        }

        // Trigger any actions after the paper finishes dispensing.
        onPuzzleSolved?.Invoke();

        yield return new WaitForSeconds(2f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("RestaurantSeated2");
    }

    private void Update()
    {
        if (isSolved)
            return;

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
