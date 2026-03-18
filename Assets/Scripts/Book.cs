using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Book : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.1f;
    [SerializeField] List<Transform> pages;
    int index = -1;
    bool rotate = false;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject forwardButton;

    private void Start()
    {
        InitialState();
    }

    public void InitialState()
    {
        if (pages == null || pages.Count == 0)
        {
            Debug.LogError("Pages list is empty.");
            return;
        }

        index = -1;
        RefreshBookState();

        if (backButton != null)
            backButton.SetActive(false);

        if (forwardButton != null)
            forwardButton.SetActive(true);
    }

    public void RotateForward()
    {
        if (rotate) return;
        if (index >= pages.Count - 1) return;

        int flipPageIndex = index + 1;
        StartCoroutine(AnimateFlip(flipPageIndex, 180f, true));
    }

    public void RotateBack()
    {
        if (rotate) return;
        if (index < 0) return;

        int flipPageIndex = index;
        StartCoroutine(AnimateFlip(flipPageIndex, 0f, false));
    }

    IEnumerator AnimateFlip(int pageIndex, float targetY, bool forward)
    {
        rotate = true;

        Transform page = pages[pageIndex];

        page.SetAsLastSibling();

        Quaternion startRotation = page.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, targetY, 0);

        float time = 0f;
        bool swappedMidway = false;

        while (time < 1f)
        {
            time += Time.deltaTime * pageSpeed;
            page.localRotation = Quaternion.Slerp(startRotation, targetRotation, time);

            float y = page.localEulerAngles.y;

            if (forward && !swappedMidway && y > 90f && y < 270f)
            {
                SetPageFrontVisible(page, false);
                swappedMidway = true;
            }

            yield return null;
        }

        page.localRotation = targetRotation;

        if (forward)
            index++;
        else
            index--;

        RefreshBookState();
        UpdateButtons();

        rotate = false;
    }

    void RefreshBookState()
    {
        if (pages == null || pages.Count == 0) return;

        for (int i = 0; i < pages.Count; i++)
        {
            if (i <= index)
            {
                pages[i].localRotation = Quaternion.Euler(0, 180f, 0);
                SetPageFrontVisible(pages[i], false);
            }
            else
            {
                pages[i].localRotation = Quaternion.Euler(0, 0f, 0);
                SetPageFrontVisible(pages[i], true);
            }
        }

        int sibling = 0;

        for (int i = 0; i <= index; i++)
        {
            if (i >= 0 && i < pages.Count)
            {
                pages[i].SetSiblingIndex(sibling);
                sibling++;
            }
        }
        for (int i = pages.Count - 1; i > index; i--)
        {
            pages[i].SetSiblingIndex(sibling);
            sibling++;
        }
    }

    void UpdateButtons()
    {
        if (backButton != null)
            backButton.SetActive(index >= 0);

        if (forwardButton != null)
            forwardButton.SetActive(index < pages.Count - 1);
    }

    void SetPageFrontVisible(Transform page, bool showFront)
    {
        Transform front = page.Find("FrontSide");
        Transform back = page.Find("BackSide");

        if (front != null)
            front.gameObject.SetActive(showFront);

        if (back != null)
            back.gameObject.SetActive(!showFront);
    }
}