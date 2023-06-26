using TMPro;
using UnityEngine;

public class StatRenderer : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text titleText;

    [SerializeField] 
    private TMP_Text valueText;

    public void SetValue(string title, string value)
    {
        titleText.text = title;
        valueText.text = value;
    }
}
