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
        for (int i=0; i<pages.Count; i++)
        {
            pages[i].transform.rotation=Quaternion.identity;
        }
        pages[0].SetAsLastSibling();
        backButton.SetActive(false);

    }

    public void RotateForward()
    {
        if (rotate == true) { return; }
        index++;
        float angle = 180;
        ForwardButtonActions();
        pages[index].SetAsLastSibling();
        StartCoroutine(Rotate(angle, true));

    }

    public void ForwardButtonActions()
    {
        if (backButton.activeInHierarchy == false)
        {
            backButton.SetActive(true);
        }
        if (index == pages.Count - 1)
        {
            forwardButton.SetActive(false);
        }
    }

    public void RotateBack()
    {
        if (rotate == true) { return; }
        float angle = 0;
        pages[index].SetAsLastSibling();
        BackButtonActions();
        StartCoroutine(Rotate(angle, false));
    }

    public void BackButtonActions()
    {
        if (forwardButton.activeInHierarchy == false)
        {
            forwardButton.SetActive(true);
        }
        if (index - 1 == -1)
        {
            backButton.SetActive(false);
        }
    }

IEnumerator Rotate(float angle, bool forward)
{
    float value = 0f;
    bool swappedSide = false;

    Transform front = pages[index].Find("FrontSide");
    Transform back = pages[index].Find("BackSide");

    if (front != null) front.gameObject.SetActive(true);
    if (back != null) back.gameObject.SetActive(false);

    while (true)
    {
        rotate = true;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        value += Time.deltaTime * pageSpeed;
        pages[index].rotation = Quaternion.Slerp(pages[index].rotation, targetRotation, value);

        float currentY = pages[index].eulerAngles.y;

        if (!swappedSide)
        {
            if (forward && currentY > 90f && currentY < 270f)
            {
                if (front != null) front.gameObject.SetActive(false);
                if (back != null) back.gameObject.SetActive(true);
                swappedSide = true;
            }
            else if (!forward && currentY < 90f)
            {
                if (front != null) front.gameObject.SetActive(true);
                if (back != null) back.gameObject.SetActive(false);
                swappedSide = true;
            }
        }

        float angle1 = Quaternion.Angle(pages[index].rotation, targetRotation);
        if (angle1 < 0.1f)
        {
            pages[index].rotation = targetRotation;

            if (forward)
            {
                if (front != null) front.gameObject.SetActive(false);
                if (back != null) back.gameObject.SetActive(true);
            }
            else
            {
                if (front != null) front.gameObject.SetActive(true);
                if (back != null) back.gameObject.SetActive(false);
                index--;
            }

            rotate = false;
            break;
        }

        yield return null;
    }
}
}
