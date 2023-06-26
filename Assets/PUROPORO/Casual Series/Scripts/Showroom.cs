using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Showroom : MonoBehaviour
{
    public Transform showroomCamera;
    public RectTransform slidesContainer;
    public Text slideNumber;
    private int currentSlide = 0;
    public Vector2[] slides;

    void Start()
    {
        showroomCamera.position = new Vector3(slides[currentSlide].x, showroomCamera.position.y, showroomCamera.position.z);
        slidesContainer.localPosition = new Vector3(slides[currentSlide].y, slidesContainer.localPosition.y, slidesContainer.localPosition.z);

        slideNumber.text = currentSlide + 1 + "/" + slides.Length;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ChangeSlide(1);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeSlide(-1);
    }

    public void ChangeSlide(int i)
    {
        currentSlide += i;

        if (currentSlide < 0)
            currentSlide = slides.Length - 1;
        else if (currentSlide >= slides.Length)
            currentSlide = 0;

        showroomCamera.position = new Vector3(slides[currentSlide].x, showroomCamera.position.y, showroomCamera.position.z);
        slidesContainer.localPosition = new Vector3(slides[currentSlide].y, slidesContainer.localPosition.y, slidesContainer.localPosition.z);

        slideNumber.text = currentSlide + 1 + "/" + slides.Length;
    }
}
