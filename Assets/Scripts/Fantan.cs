using UnityEngine;
using System.Collections;

public class Fantan : MonoBehaviour
{

	//Итак жизни дивана:
	private float hp = 100f;

	public void setDamage (float value)
	{
		hp -= value;
	}

	//У всех кто вошел включаем регенерацию:
	void OnTriggerEnter (Collider coll)
	{
		if (coll.gameObject.tag == "player_unit") {
			Movement_Units inputed_unit = coll.gameObject.GetComponent<Movement_Units> ();
			//Добавляем юнит в сисок "У фантана"
			if (!GameManager.Instance.player_unit_in_save_zone_List.Contains (inputed_unit)) {
				GameManager.Instance.player_unit_in_save_zone_List.Add (inputed_unit);
			}
			//Удаляем из списка карты:
			if (GameManager.Instance.player_units_in_Map_List.Contains (inputed_unit)) {
				GameManager.Instance.player_units_in_Map_List.Remove (inputed_unit);
			}
			inputed_unit.regeneration = true;
		}
	}

	//У всех кто вышел отключаем регенерацию:
	void OnTriggerExit (Collider coll)
	{
		if (coll.gameObject.tag == "player_unit") {
			Movement_Units inputed_unit = coll.gameObject.GetComponent<Movement_Units> ();
			//Добавляем юнит в сисок "У фантана"
			if (GameManager.Instance.player_unit_in_save_zone_List.Contains (inputed_unit)) {
				GameManager.Instance.player_unit_in_save_zone_List.Remove (inputed_unit);
			}
			//Удаляем из списка карты:
			if (!GameManager.Instance.player_units_in_Map_List.Contains (inputed_unit)) {
				GameManager.Instance.player_units_in_Map_List.Add (inputed_unit);
			}
			inputed_unit.regeneration = false;
		}
	}
	
}
