using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] public GameObject buildMenu;
    [SerializeField] private GameObject sleepPanel;
    [SerializeField] private GameObject stormMessage;
    [SerializeField] private GameObject crystalMessage;
    [SerializeField] private GameObject buildButton;
    [SerializeField] private GameObject stormBar;
    [SerializeField] GameObject baseHealthBar;
    [SerializeField] private GameObject initialFadeEffect;

    [SerializeField] private float moveSpeed = 2f;    // Speed at which the message moves up
    [SerializeField] private float fadeDuration = 2f; // Duration over which the message fades

    private CanvasGroup canvasGroup; // For fading out UI elements
    private Vector3 initialPosition;
    private CanvasGroup canvasGroup2; // For fading out UI elements
    private Vector3 initialPosition2;

    void Start()
    {
        //StartCoroutine(FadeEffect());
        // Get or add a CanvasGroup component
        canvasGroup = stormMessage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = stormMessage.AddComponent<CanvasGroup>();
        }
        // Initial position of the message
        initialPosition = stormMessage.transform.position;

        canvasGroup2 = crystalMessage.GetComponent<CanvasGroup>();
        if (canvasGroup2 == null)
        {
            canvasGroup2 = crystalMessage.AddComponent<CanvasGroup>();
        }
        // Initial position of the message
        initialPosition2 = crystalMessage.transform.position;
    }

    public void ToggleBuildMenu()
    {
        buildMenu.SetActive(!buildMenu.activeSelf);
    }

    public void ToggleSleepPanel()
    {
        sleepPanel.SetActive(!sleepPanel.activeSelf);
    }

    public void ToggleStormMessage()
    {
        stormMessage.SetActive(true);
        StartCoroutine(MoveAndFade());
    }
    public void ToggleCrystalMessage()
    {
        crystalMessage.SetActive(true);
        StartCoroutine(MoveAndFade2());
    }

    IEnumerator MoveAndFade()
    {
        float elapsedTime = 0f;

        Vector3 startPosition = initialPosition;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        while (elapsedTime < fadeDuration)
        {
            stormMessage.transform.position = new Vector3(
                startPosition.x, startPosition.y + elapsedTime * moveSpeed, startPosition.z);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure it's completely faded out and moved
        if (canvasGroup2 != null)
        {
            canvasGroup2.alpha = 0f;
        }
        crystalMessage.transform.position = initialPosition2;
        crystalMessage.SetActive(false);
    }
    IEnumerator MoveAndFade2()
    {
        float elapsedTime = 0f;

        Vector3 startPosition = initialPosition2;

        if (canvasGroup2 != null)
        {
            canvasGroup2.alpha = 1f;
        }

        while (elapsedTime < fadeDuration)
        {
            crystalMessage.transform.position = new Vector3(
                startPosition.x, startPosition.y + elapsedTime * moveSpeed, startPosition.z);

            if (canvasGroup2 != null)
            {
                canvasGroup2.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure it's completely faded out and moved
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        stormMessage.transform.position = initialPosition;
        stormMessage.SetActive(false);
    }

    public void SetBuildButton(bool value)
    {
        buildButton.GetComponent<Button>().interactable = value;
    }

    public void DeactivateStormBar()
    {
        stormBar.SetActive(false);
    }

    public IEnumerator ActivateStormBar(int maxStormHealth)
    {
        //yield return new WaitForSeconds(0.2f);
        Slider slider = stormBar.GetComponent<Slider>();
        BarScript barScript = stormBar.GetComponent<BarScript>();
        barScript.SetMaxValue(maxStormHealth);
        barScript.SetValue(0);
        stormBar.SetActive(true);
        for (int i = 0; i < maxStormHealth; i++)
        {
            barScript.SetValue((int)slider.value + 5);
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void SetStormBar(int value)
    {
        Slider slider = stormBar.GetComponent<Slider>();
        BarScript barScript = stormBar.GetComponent<BarScript>();
        barScript.SetValue(value);
    }

    public void ToggleBaseBar()
    {
        baseHealthBar.SetActive(!baseHealthBar.activeSelf);
    }

    public void SetBaseBar(int value)
    {
        Slider slider = baseHealthBar.GetComponent<Slider>();
        BarScript barScript = baseHealthBar.GetComponent<BarScript>();
        barScript.SetValue(value);
    }

    IEnumerator FadeEffect()
    {
        initialFadeEffect.SetActive(true);
        yield return null;
    }
}
