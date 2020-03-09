using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//Основной класс моего приложения(архитектура звездочка или снежинка)
public class GameManager: MonoBehaviour
{
	//денежные средства игрока(расходуются на апгрейды и покупку войск)
	public int player_money = 100;

	public float divan_hp = 100f;

	public int count_Spawn = 1;

	public static GameManager Instance = null;

	//Так как героя я удалять не стану, то буде иметь на него ссылку:
	public Movement_Units hero_unit = null; 

	//Список юнитов что сейчас выбраны игроком:
	public List<Movement_Units> selected_unit_List = new List<Movement_Units> ();
	public Movement_Units active_unit = null;

	//Список врагов на карте:
	public List<Movement_Units> enemy_map_List = new List<Movement_Units> ();

	//Список юнитов игрока вне фантана:
	public List<Movement_Units> player_units_in_Map_List = new List<Movement_Units> ();

	//Список юнитов игрока у фонтана:
	public List<Movement_Units> player_unit_in_save_zone_List = new List<Movement_Units> ();

	//Одновременно отображаем только один UI:
	public GameObject active_UI_Panel = null;

	//Ссылка на Фантан:
	public GameObject fantan_object = null;
	//Ссылка на диван:
	public GameObject divan_object = null;

	//Переключатель таймера:
	public Text level_timer = null;
	private bool timer_bool = true;
	private float times = 30f;

	//Ссылки на UI панели:
	public GameObject ui_minion_panel = null;
	public GameObject ui_player_panel = null;
	public GameObject ui_Kazarm_melee_panel = null;
	public GameObject ui_Kazarm_distance_panel = null;
	public Kazarm selected_kazarm = null;

	public Text money_Text = null;
	public Text divan_hp_Text = null;

	//Отображаем необходимый UI:
	public ui_type ui;
	public enum ui_type
	{
		empty,
		main_character,
		unit_distance,
		unit_melee,
		melee_kazarm,
		distance_kazarm,
	}

	void Start ()
	{
		Instance = this;
	}

	void Update ()
	{
		//так времени совсем нету уже), пусть проверка пока будет не через event-ы
		if (divan_hp <= 0) {
			Application.CancelQuit ();
		}
		timer ();
		schow_UI ();
	}

	private void timer ()
	{
		//Таймер глобалного времени:
		if (timer_bool) {
			if (times > 0) {
				times -= Time.deltaTime;
				//Ну и обновляем значение на UI:
				level_timer.text = times.ToString ();
			}
			if (times < 0) {
				times = 0f;
				level_timer.text = times.ToString ();
				//Запускаем спавн:
				Spawn_Manager.Instance.start_Spawn (count_Spawn);
				//достчитали отключаем.
				timer_bool = false;
				count_Spawn += 1;
			}
		}
		//Если на карте больше нет живых противников, то стартуем таймер и волну снова!
	}

	public void deselect_active_unit ()
	{
		if (selected_unit_List.Contains (active_unit)) {
			selected_unit_List.Remove (active_unit);
		}
		if (active_unit != null) {
			active_unit.setDeselected ();
		}
		active_unit = null;
	}

	//Выводим актуальный UI:
	private void schow_UI ()
	{
		switch (ui) {
		case ui_type.melee_kazarm:
			ui_Kazarm_melee_panel.SetActive (true);
			ui_Kazarm_distance_panel.SetActive (false);
			break;
		case ui_type.distance_kazarm:
			ui_Kazarm_distance_panel.SetActive (true);
			ui_Kazarm_melee_panel.SetActive (false);
			break;
		case ui_type.empty:
			disable_ui ();
			break;
		}
	}

	public void addMoney (int count)
	{
		this.player_money += count;
		money_Text.text = player_money.ToString ();
	}

	public void updateMoney ()
	{
		money_Text.text = player_money.ToString ();
	}

	public void divan_Damage (float count)
	{
		divan_hp -= count;
		divan_hp_Text.text = divan_hp.ToString ();
	}

	public void disable_ui ()
	{
		//ui_minion_panel.SetActive (false);
		//ui_player_panel.SetActive (false);
		ui_Kazarm_melee_panel.SetActive (false);
		ui_Kazarm_distance_panel.SetActive (false);
	}
}

/*
 * using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class FloatEvent : UnityEvent<float> {}
public class IntEvent : UnityEvent<int> {}
public class BoolEvent : UnityEvent<bool> {}
public class StringEvent : UnityEvent<string> {}
public class Event : UnityEvent {}

public class EventAggregator : MonoBehaviour {
	
	public static IntEvent OnDamageEvent = new IntEvent();
	

	void Start () 
	{
		OnDamageEvent.AddListener(Damage);
		OnDamageEvent.Invoke(50);
	}
	

	void Damage(int damage)
	{
		Debug.Log("damage = " + damage);
	}
	
	
}
*/