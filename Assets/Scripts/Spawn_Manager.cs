using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn_Manager : MonoBehaviour
{
	//Вообще конечно для мобильных платформ я бы переделал использование одних юнитов многократно,
	//ну то есть(был бы какой-то пул в который бы попадали мертвые юниты, и ьам они переводились бы в начальное состояние, после чего бы вторично использовались)
	//но у меня возможно просто не хватит времени), на данных выходных

	public static Spawn_Manager Instance = null;

	//Настройки первой волны:
	public int melee_minion_count = 4;
	public int distance_minion_count = 3;
	public int boss_count = 1;

	//Списки доступных префабов:
	public List<GameObject> melee_minion_prefab_List = new List<GameObject> ();
	public List<GameObject> distance_minion_prefab_List = new List<GameObject> ();
	public List<GameObject> boss_prefab_List = new List<GameObject> ();

	//Список спавн поинтов:
	public List<GameObject> spawn_points_list = new List<GameObject> ();

	void Start ()
	{
		Instance = this;
	}

	//Метод предоставляет спавн поинт из списка:
	GameObject getRandoomObject (List<GameObject> list)
	{
		int index = Random.Range (0, list.Count);
		return list [index];
	}

	//каждая волна увеличивает врагов кратно своему порядковому числу:
	public void start_Spawn (int count)
	{
		//Определяем количество Юнитов в новой волне:
		int new_melee_minion_count = melee_minion_count * count;
		int new_distance_minion_count = distance_minion_count * count;
		int new_boss_count = boss_count * count; 

		//Спавним пехоту:
		for (int i = 0; i < new_melee_minion_count; i++) {
			//Выбираем случайный префаб миньена пехотинца:
			GameObject selected_prefab = getRandoomObject (melee_minion_prefab_List);
			GameObject new_Melee_Minion = (GameObject)Instantiate (selected_prefab, getRandoomObject (spawn_points_list).transform.position, selected_prefab.transform.rotation);
			//Теперь добавляем его в список карты:
			GameManager.Instance.enemy_map_List.Add (new_Melee_Minion.GetComponent<Movement_Units> ());
		}

		//Спавним стрелков:
		for (int j = 0; j < new_distance_minion_count; j++) {
			//Выбираем случайный префаб миньена пехотинца:
			GameObject selected_prefab = getRandoomObject (distance_minion_prefab_List);
			GameObject new_Distance_Minion = (GameObject)Instantiate (selected_prefab, getRandoomObject (spawn_points_list).transform.position, selected_prefab.transform.rotation);
			//Теперь добавляем его в список карты:
			GameManager.Instance.enemy_map_List.Add (new_Distance_Minion.GetComponent<Movement_Units> ());
		}

		//Спавним сбосов:
		for (int k = 0; k < new_boss_count; k++) {
			//Выбираем случайный префаб миньена пехотинца:
			GameObject selected_prefab = getRandoomObject (boss_prefab_List);
			GameObject new_Bos = (GameObject)Instantiate (selected_prefab, getRandoomObject (spawn_points_list).transform.position, selected_prefab.transform.rotation);
			//Теперь добавляем его в список карты:
			GameManager.Instance.enemy_map_List.Add (new_Bos.GetComponent<Movement_Units> ());
		}

	}
}
