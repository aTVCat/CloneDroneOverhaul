using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedTextField : MonoBehaviour
{
	public string LocalizationID;

	public bool DontTranslateText;

	public bool DontTranslateOnStart;

	public bool DisableWrappingForLogographicLanguages = true;

	public int MaxCharactersPerJapaneseLineBreak = -1;
}