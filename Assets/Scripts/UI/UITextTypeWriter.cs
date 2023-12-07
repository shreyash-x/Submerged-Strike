using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

// attach to UI Text component (with the full text already there)
namespace Narrative
{
	public class UITextTypeWriter : MonoBehaviour
	{

		[SerializeField] private Button continueButton;
		[SerializeField] private float delay = 0.05f;
		private TextMeshProUGUI txt;
		internal GameObject image;
		private string story;

		private void Awake()
		{
			continueButton.gameObject.SetActive(false);
			txt = GetComponent<TextMeshProUGUI>();
			txt.text = "";
		}

		public void SetNarrativeText(string text)
		{
			if (txt.text != "")
			{
				StartCoroutine(BackspaceAll(text));
			}
			else
			{
				story = text;
				StartCoroutine(PlayText());
			}
		}

		private IEnumerator PlayText()
		{
			foreach (char c in story)
			{
				if (c == '$')
				{
					txt.text += "_";
					yield return new WaitForSecondsRealtime(3 * delay);
					txt.text = txt.text.Substring(0, txt.text.Length - 1);
					yield return new WaitForSecondsRealtime(3 * delay);
				}
				else if (c == '#')
				{
					string curStr = txt.text;
					// backspace the entire string
					for (int i = 0; i < curStr.Length; i++)
					{
						txt.text = txt.text.Substring(0, txt.text.Length - 1);
						yield return new WaitForSecondsRealtime(0.002f);
					}
					yield return new WaitForSecondsRealtime(delay);
				}
				else if (c == '@')
				{
					var tmpText = txt.text;
					txt.text += "\n_";
					yield return new WaitForSecondsRealtime(0.01f);
					txt.text = tmpText + "\n";
				}
				else
				{
					txt.text += c;
					yield return new WaitForSecondsRealtime(delay);
				}
			}
			if (image != null)
			{
				// fade in gameobject
				var color = image.GetComponent<Unity.VectorGraphics.SVGImage>().color;
				color.a = 0;
				image.GetComponent<Unity.VectorGraphics.SVGImage>().color = color;
				image.SetActive(true);
				while (color.a < 1)
				{
					color.a += 0.01f;
					image.GetComponent<Unity.VectorGraphics.SVGImage>().color = color;
					yield return new WaitForSecondsRealtime(0.01f);
				}
			}
			continueButton.gameObject.SetActive(true);
		}

		private IEnumerator BackspaceAll(string nextString)
		{
			string curStr = txt.text;
			// backspace the entire string
			for (int i = curStr.Length - 1; i >= 0; i--)
			{
				txt.text = curStr.Substring(0, i);
				yield return new WaitForSecondsRealtime(0.002f);
			}
			yield return new WaitForSecondsRealtime(delay);
			story = nextString;
			StartCoroutine(PlayText());
		}

	}
}