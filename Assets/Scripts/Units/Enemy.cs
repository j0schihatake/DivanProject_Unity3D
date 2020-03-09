using UnityEngine;
using System.Collections;

public class Enemy : Movement_Units
{
	public override void death ()
	{
		//вынес в отдельный метод потому что еслибы реализовывал пул, тут бы и перехватывал юнитов)
		StartCoroutine ("deatch_progress");
	}

	void Update ()
	{
		if (hp <= 0) {
			if (GameManager.Instance.enemy_map_List.Contains ((Movement_Units)this)) {
				death ();
				//начисляем опыт и деньги игроку:
				GameManager.Instance.player_money += gold_bonus;
			}
		}
		atack_Timer ();
		state_mashine ();
		if (regeneration) {
			hp_regeneration ();
		}
	}

	void OnMouseDown ()
	{
		if (GameManager.Instance.active_unit != null) {
			GameManager.Instance.active_unit.active_enemy = this;
		}
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
				//достчитали отключаем.
				atack_timer_Bool = false;
				atack_end = true;
			}
		}
	}

//-------------------------------------------------------Поведение:-----------------------------------------------------
	//Времени на вшивание кривой сложности а так-же нейронного контроллера нет, потому простая машина состояний:
	public override void state_mashine ()
	{
		/*
		 * Поведение миньена:
		 * 1.Появился на карте
		 * 2.Проверяем наличие врагов вне фонтана,
		 * 	2.а Врагов нет иду к валить диван
		 * 	2.b Враги есть иду и атакую ближайшего
		* итак, тут проверяем наличие врагов,
		* и не находимся ли мы сейчас у фантана:
		* */
		if (GameManager.Instance.player_units_in_Map_List.Count == 0) {
			//Отменяем активного противника:
			active_enemy = null;
			//значит движемся к дивану и даем ему тумаков:
			if (target_object == null) {
				target_object = GameManager.Instance.divan_object;
			}
			//Теперь подобно атаке на юнита:
			if (target_object != null) {
				actual_Distance = Vector3.Distance (target_object.transform.position, this.transform.position);
				//Debug.Log (actual_Distance);
				if (actual_Distance <= atack_distance) {
					battle = true;
					myNavAgent.stoppingDistance = atack_distance;
					//Итак мы достигли противника(если предыдущая атака закончена):
					if (atack_end) {
						//запускаем новый тамер для новой атаки
						atack_timer_Bool = true;
						resset_atack_timer (atack_speed);
						//теперь наносим урон аппоненту
						GameManager.Instance.divan_Damage (atack_power);
					}
				} else {
					battle = false;
					//Проверяем не движемся ли мы(зачем постоянно тыкать))
					if ((myNavAgent.pathEndPosition - myNavAgent.transform.position).magnitude == 0) {
						setDestination (target_object.transform.position);
					}
				}
			}
		} else {
			//Если мы еще не в бою, то требуется выбрать противника:
			target_object = null;
			if (!battle) {
				if (active_enemy == null) {
					active_enemy = select_Enemy (GameManager.Instance.player_units_in_Map_List);
				}
			}
			//Теперь необходимо подобраться к противнику на расстояние удара:
			if (active_enemy != null) {
				actual_Distance = Vector3.Distance (active_enemy.transform.position, this.transform.position);
				//Debug.Log (actual_Distance);
				if (actual_Distance <= atack_distance) {
					battle = true;
					myNavAgent.stoppingDistance = atack_distance;
					//Итак мы достигли противника(если предыдущая атака закончена):
					if (atack_end) {
						//запускаем новый тамер для новой атаки
						atack_timer_Bool = true;
						resset_atack_timer (atack_speed);
						//теперь наносим урон аппоненту
						active_enemy.set_Damage (atack_power);
					}
				} else {
					battle = false;
					setDestination (active_enemy.transform.position);
				}
			}
			//движемся к обьекту
			//setDestination (active_enemy.transform.position);
		}
	}
}
