using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Minion : Movement_Units
{
	public Kazarm my_Kazarm = null;

	void Update ()
	{
		if (hp <= 0) {
			death ();
		}
		atack_Timer ();
		state_mashine ();
		if (regeneration) {
			hp_regeneration ();
		}
	}

	public override void death ()
	{
		//вынес в отдельный метод потому что еслибы реализовывал пул, тут бы и перехватывал юнитов)
		StartCoroutine ("deatch_progress");
	}

	void OnMouseDown ()
	{
		GameManager.Instance.active_unit = this;
		setSelected ();
	}

	//Времени на вшивание кривой сложности а так-же нейронного контроллера нет, потому простая машина состояний:
	public override void state_mashine ()
	{
		/*
		 * Поведение миньена:
		 * 1.Появился на карте
		 * 2.Проверяем наличие врагов
		 * 	2.а Врагов нет иду к фантану
		 * 	2.b Враги есть иду(до достаточной дистанции) и атакую ближайшего
		* итак, тут проверяем наличие врагов,
		* и не находимся ли мы сейчас у фантана:
		* */
		if (!this.isSelected ()) {
			if (GameManager.Instance.enemy_map_List.Count == 0) {
				if (!regeneration & target_object == null) {
					//значит движемся к фантану:
					target_object = GameManager.Instance.fantan_object;
					setDestination (GameManager.Instance.fantan_object.transform.position);
				}
			} else {
				target_object = null;
				//Если мы еще не в бою, то требуется выбрать противника:
				if (!battle) {
					if (active_enemy == null) {
						active_enemy = select_Enemy (GameManager.Instance.enemy_map_List);
					}
				}
				//Теперь необходио подобраться к противнику на расстояние удара:
				if (active_enemy != null) {
					actual_Distance = Vector3.Distance (active_enemy.transform.position, this.transform.position);
					//Debug.Log (actual_Distance);
					if (actual_Distance <= atack_distance) {
						battle = true;
						myNavAgent.stoppingDistance = atack_distance;
						if (atack_end) {
							//запускаем новый тамер для ноой атаки
							atack_timer_Bool = true;
							resset_atack_timer (atack_speed);
							//теперь наносим урон аппоненту
							active_enemy.set_Damage (atack_power);

						}
					} else {
						battle = false;
						if ((myNavAgent.pathEndPosition - myNavAgent.transform.position).magnitude == 0) {
							setDestination (active_enemy.transform.position);
						}
					}
				}
				//движемся к обьекту
				//setDestination (active_enemy.transform.position);
			}

		} else {
			//Тут прекращаем что либо делать и слушаем что нам говорят);
			/*
			 * Возможные команды: идти, атакавать.
			 * 
			 */
			switch (order) {
			case minion_order.atack:
				target_object = null;
				//Если мы еще не в бою, то требуется выбрать противника:
				if (!battle) {
					if (active_enemy == null) {
						active_enemy = select_Enemy (GameManager.Instance.enemy_map_List);
					}
				}
				//Теперь необходио подобраться к противнику на расстояние удара:
				if (active_enemy != null) {
					actual_Distance = Vector3.Distance (active_enemy.transform.position, this.transform.position);
					//Debug.Log (actual_Distance);
					if (actual_Distance <= atack_distance) {
						battle = true;
						myNavAgent.stoppingDistance = atack_distance;
						if (atack_end) {
							//запускаем новый тамер для ноой атаки
							atack_timer_Bool = true;
							resset_atack_timer (atack_speed);
							//теперь наносим урон аппоненту
							active_enemy.set_Damage (atack_power);
							
						}
					} else {
						battle = false;
						if ((myNavAgent.pathEndPosition - myNavAgent.transform.position).magnitude == 0) {
							setDestination (active_enemy.transform.position);
						}
					}
				}
				if (active_enemy == null) {
					order = minion_order.empty;
				}
				break;
			case minion_order.empty:
				break;
			}
		}
	}
}
