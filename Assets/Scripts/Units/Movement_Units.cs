using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Movement_Units : MonoBehaviour
{
	/*
	 * Общие свойства всех двигающихся юнитов(постараюсь завернуть их в этот класс обертку):
	 * Жизни;
	 * Броня (Armor) — сколько урона в процентном соотношении может быть поглощено, дробное число [0..1];
	 * Показатель атаки (Attack) — сколько урона наносится за один удар;
	 * Скорость атаки (Attack Speed) — ударов в секунду. Число может быть дробным, например, 1.5 удара в секунду;
	 * Скорость передвижения (Speed) — метров в секунду;
	 * Дальность атаки (Attack Range) — расстояние, на которое необходимо подойти, чтобы нанести урон;
	 * Количество золота (Gold) — количество золота, которое получает игрок за убийство юнита;
	 * Количество опыта (X получаемыое игроком за убийство).
	*/

	public float hp = 100;
	public float max_hp = 100;

	public bool regeneration = false;

	public float armor = 0;

	public int atack_power = 3;

	public float atack_speed = 1.5f;

	public float speed = 0f;

	public float atack_distance = 2f;

	public int gold_bonus = 50;

	public int exp = 10;

	private bool selected = false;

	public Movement_Units active_enemy = null;

	//Только для управляемых юнитов
	public minion_order order;
	public enum minion_order
	{
		empty,
		move,
		atack,
	}

	//Выделение Юнита эта часть его меши(ну я так сделал):
	public GameObject selected_visual = null;

	//Для проверки прибыли ли мы к обьекту буду использовать данную ссылку:
	public GameObject target_object = null;

	//Переменные для указания дистанции:
	public float actual_Distance = 0f;
	
	//Переменные для паузы между ударами
	public float atack_time = 0f;
	public bool atack_timer_Bool = false;
	public bool atack_end = true;
	public bool battle = false;

	//Для разграничения на врагов и друзей будем явно задавать тип нашего юнита:
	public unit_type type;
	public enum unit_type
	{
		player_melee_minion,
		player_distance_minion,
		player_main_character,
		//враги
		enemy_melee_minion,
		enemy_distance_minion,
		enemy_boss,
	}

	public UnityEngine.AI.NavMeshAgent myNavAgent = null;

	void Start ()
	{
		//Метод коненочно костыльный, большинство параметров я мог бы настроить прямо в NavMeshAgent(но задание есть задание)
		init ();
	}

	//Каждому юниту можно указать точку места назначения(он начнет к ней двигатся):
	public void setDestination (Vector3 destination)
	{
		//Итак если имеется позиция для дислокации, то двигаемся к ней
		myNavAgent.destination = destination;
		//myNavAgent.Move (destination);
	}

	//Иногда необходимо остановить движение экстренно:
	public void stop ()
	{
		myNavAgent.Stop ();
	}

	//Выношу первоначальную настройку Юнита в этот метод:
	public void init ()
	{
		//Получаем ссылку на NavMeshAgent:
		myNavAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}
	
	//пометить юнит как выбранный:
	public void setSelected ()
	{
		if (!GameManager.Instance.selected_unit_List.Contains (this)) {
			GameManager.Instance.selected_unit_List.Add (this);
		}
		//selected_visual.SetActive (true);
		selected = true;
	}
	public bool isSelected ()
	{
		return selected;
	}

	//Снять выделение с юнита:
	public void setDeselected ()
	{
		//selected_visual.SetActive (false);
		selected = false;
	}

	//Помечаем как мешень:
	public void setTarget (Movement_Units target)
	{
		active_enemy = target;
	}

	//убираем пометку:
	public void deselectTarget ()
	{
		active_enemy = null;
	}

	//Метод восстанавливает здоровье:
	public void hp_regeneration ()
	{
		if (hp < max_hp) {
			hp += 0.1f;
		}
		if (hp > max_hp) {
			hp = max_hp;
		}
	}

	//Метод получает на вход список и выводит наиближайший к нам обьект:
	public Movement_Units select_Enemy (List<Movement_Units> enemy_list)
	{
		//ВНИМАНИЕ! это место подлежит дальнейшей оптимизации. (думаю для тех количеств юнитов что у меня на уровне такое решение возможно)
		//Для выборки получаем первый элемент:
		Movement_Units select_enemy = enemy_list [0];
		float distance_to_enemy = Vector3.Distance (this.gameObject.transform.position, select_enemy.gameObject.transform.position);
		for (int i = 0; i < enemy_list.Count; i++) {
			float next_distance = Vector3.Distance (enemy_list [i].gameObject.transform.position, this.gameObject.transform.position);
			if (next_distance < distance_to_enemy) {
				distance_to_enemy = next_distance;
				select_enemy = enemy_list [i];
			}
		}
		return select_enemy;
	}

	public void set_Damage (int atack_power)
	{
		/*
		 * HP = сила атаки - % погашенный броней;
		 * */
		Debug.Log ("наношу урон: " + (atack_power - ((atack_power / 100) * this.armor)));
		this.hp -= atack_power - ((atack_power / 100) * this.armor);
	}

	//Таймер паузы между атаками(думаю явно будет не меньше чем длительность анимации!)
	public void atack_Timer ()
	{
		//Таймер атаки(по моему у меня дежавю):
		if (atack_timer_Bool) {
			if (atack_time > 0) {
				atack_time -= Time.deltaTime;
				atack_end = false;
			}
			if (atack_time < 0) {
				atack_time = 0f;
				//досчитали отключаем.
				atack_timer_Bool = false;
				atack_end = true;
			}
		}
	}

	//Сброс таймера:
	public void resset_atack_timer (float time)
	{
		atack_time = time;
	}


	//Интелект зашиваем вот сюда)
	public abstract void state_mashine ();

	public abstract void death ();

	//После смерти враг проиграет анимацию смерти и медленно опуститься вниз под землю, как в старых стратегиях)
	public IEnumerator deatch_progress ()
	{
		//Удаляем наш юнит из всех игровых списков:
		if (GameManager.Instance.enemy_map_List.Contains (this)) {
			GameManager.Instance.enemy_map_List.Remove (this);
		}
		if (GameManager.Instance.player_units_in_Map_List.Contains (this)) {
			GameManager.Instance.player_units_in_Map_List.Remove (this);
		}
		if (GameManager.Instance.player_unit_in_save_zone_List.Contains (this)) {
			GameManager.Instance.player_unit_in_save_zone_List.Remove (this);
		}

		Destroy (this.gameObject, 3f);
		yield return null;
	}
}