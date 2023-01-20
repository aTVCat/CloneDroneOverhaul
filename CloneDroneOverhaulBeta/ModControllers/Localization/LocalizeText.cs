using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class LocalizeText : MonoBehaviour
{
    public Text TextComponent;
    public string LocalizationID;

    private void Awake()
    {
        TextComponent = base.GetComponent<Text>();
    }
}