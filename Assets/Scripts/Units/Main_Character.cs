using UnityEngine;
using System.Collections;

public class Main_Character : Movement_Units
{

	public override void death ()
	{
		//Итак герой у нас особенный:
		
	}

	public override void state_mashine ()
	{
		switch (order) {
		case minion_order.atack:
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

	void Update ()
	{
		state_mashine ();
		if (regeneration) {
			hp_regeneration ();
		}
	}

	void OnMouseDown ()
	{
		GameManager.Instance.active_unit = this;
		setSelected ();
	}
}
