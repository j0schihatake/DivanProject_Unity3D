using UnityEngine;
using System.Collections;

public class Kazarm : MonoBehaviour
{
	public GameObject minion_prefab = null;

	//Точка выхода из казармы:
	public GameObject spawn_point = null;	

	//Выбрал ли игрок казарму в данный момент:
	public bool kazarm_selected = false;

	//бонусы
	public float hp_Bonus = 0f;
	public float armor_Bonus = 0f;
	public float atack_Speed_Bonus = 0f;
	public int atack_Power_Bonus = 0;

	//У каждого апгрейда есть стоимость:
	public int spawn_minion_Coast = 100;
	public int hp_Bonus_Coast = 100;
	public int armor_Bonus_Coast = 50;
	public int atack_Speed_Bonus_Coast = 30;
	public int atack_Power_Bonus_Coast = 10;

	//Ссылки на обьекты кнопок(UI):
	public GameObject kazarmPanell = null;
	public GameObject spawn_Button = null;
	public GameObject hp_Bonus_Button = null;
	public GameObject armor_Bonus_Button = null;
	public GameObject atack_Speed_Bonus_Button = null;
	public GameObject atack_Power_Bonus_Button = null;

	void Update ()
	{
		//Проверяем если казарма выбрана игроком:
		if (kazarm_selected) {
			if (!kazarmPanell.activeSelf) {
				kazarmPanell.SetActive (true);
			}
			of_Button ();
			//Доступен ли найм миньена:
			if (GameManager.Instance.player_money >= spawn_minion_Coast) {
				spawn_Button.SetActive (true);
			}
			//Доступен ли бонус hp:
			if (GameManager.Instance.player_money >= hp_Bonus_Coast) {
				hp_Bonus_Button.SetActive (true);
			}
			//Доступен ли бонус брони:
			if (GameManager.Instance.player_money >= armor_Bonus_Coast) {
				armor_Bonus_Button.SetActive (true);
			}
			//Доступен ли бонус скорости атаки:
			if (GameManager.Instance.player_money >= atack_Speed_Bonus_Coast) {
				atack_Speed_Bonus_Button.SetActive (true);
			}
			//Доступен ли бонус силы атаки:
			if (GameManager.Instance.player_money >= atack_Power_Bonus_Coast) {
				atack_Power_Bonus_Button.SetActive (true);
			}
		}
	}

	private void of_Button ()
	{
		//kazarmPanell.SetActive(false);
		spawn_Button.SetActive (false);
		hp_Bonus_Button.SetActive (false);
		armor_Bonus_Button.SetActive (false);
		atack_Power_Bonus_Button.SetActive (false);
		atack_Speed_Bonus_Button.SetActive (false);
	}

	public void create_Unit ()
	{
		GameObject new_Minion = (GameObject)Instantiate (minion_prefab, spawn_point.transform.position, minion_prefab.transform.rotation);
		//Пичкаем нашего миньена бонусами:
		Movement_Units new_minion_script = new_Minion.GetComponent<Movement_Units> ();
		new_minion_script.max_hp += hp_Bonus;
		new_minion_script.armor += armor_Bonus;
		new_minion_script.atack_power += atack_Power_Bonus;
		new_minion_script.atack_speed += atack_Speed_Bonus;
		//Так как казармы вне фонтана то добавляем миньена на карту:
		GameManager.Instance.player_units_in_Map_List.Add (new_minion_script);
		GameManager.Instance.player_money -= spawn_minion_Coast;
		GameManager.Instance.updateMoney ();
	}

//-------------------------------------------Методы дергаемые с UI:--------------------------------------------------

	public void addHPBonus ()
	{
		this.hp_Bonus += 20;
		GameManager.Instance.player_money -= hp_Bonus_Coast;
		GameManager.Instance.updateMoney ();
	}

	public void addArmorBonus ()
	{
		this.armor_Bonus += 3;
		GameManager.Instance.player_money -= armor_Bonus_Coast;
		GameManager.Instance.updateMoney ();
	}

	public void addAtackSpeedBonus ()
	{
		this.atack_Speed_Bonus += 1;
		GameManager.Instance.player_money -= atack_Speed_Bonus_Coast;
		GameManager.Instance.updateMoney ();
	}

	public void addAtackPowerBonus ()
	{
		this.atack_Power_Bonus += 5;
		GameManager.Instance.player_money -= atack_Power_Bonus_Coast;
		GameManager.Instance.updateMoney ();
	}

}
