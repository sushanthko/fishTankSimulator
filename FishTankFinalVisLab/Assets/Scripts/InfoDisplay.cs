using UnityEngine;
using TMPro;

public class InfoDisplay : MonoBehaviour
{

	[SerializeField]
	private TMP_Text text;

	/// <summary>
	/// Sets the text of the InfoDisplay
	/// </summary>
	/// <param name="text">the text to display</param>
	public void SetText(string text)
	{
		this.text?.SetText(text);
	}
}
