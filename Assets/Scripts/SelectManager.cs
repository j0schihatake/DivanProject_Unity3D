using UnityEngine;
using System.Collections;

public class SelectManager : MonoBehaviour
{
	public Vector3 disable_position = Vector3.zero;

	//Просто буду убирать из поля зрения игрока данный обьект
	public IEnumerator deactivate ()
	{
		yield return new WaitForSeconds (1f);
		this.transform.position = disable_position;
	}

	//Добавляем Юниты в коллайдере в спиcок выбранных:
	void OnTriggerEnter (Collider coll)
	{
		//Мы выделяем своих:
		//Debug.Log (coll.gameObject.tag);
		if (coll.gameObject.tag == "player_unit") {
			Movement_Units selected = coll.gameObject.GetComponent<Movement_Units> ();
			if (!GameManager.Instance.selected_unit_List.Contains (selected)) {
				GameManager.Instance.selected_unit_List.Add (selected);
				//Помечаем Unit как выбранный(управляемый игроком):
				GameManager.Instance.active_unit = selected;
				selected.setSelected ();
			}
		}
	}
}
