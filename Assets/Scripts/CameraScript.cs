using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
//Создаю универсальную камеру:
	public float leftrightspeed = 14;
	public float upbackspeed = 14;
	private float xSpeed = 14;
	private Quaternion rotate;
	private Vector3 movedirection;
	private Vector3 rotation;
//Покачто это скопище всех существующих скриптов MouseLook:
	private float xorbitSpeed = 250;
	private float yorbitSpeed = 120.0f;
	private float yMinLimit = -20f;
	private float yMaxLimit = 80f;
	private Vector3 angles;
	private float x;
	private float y;
	public enum RotationAxes
	{
		MouseXAndY = 0,
		MouseX = 1,
		MouseY = 2
	}
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	public float minimumX = -360F;
	public float maximumX = 360F;
	public float minimumY = -60F;
	public float maximumY = 60F;
	float rotationY = 0F;

//Режим следования за обьектом:
	public Transform FolowObject;
	public float distance = 10f;
	public float height = 5f;
	private float heightDamping = 2f;
	private float rotationDamping = 3f;

	public Camera spacesCamera = null;

	public GameObject selectBox = null;
	public BoxCollider boxCollider = null;
	public GameObject main_object = null;
	public Transform cam_transform = null;
	public Transform start_cam_transform = null;

	public Movement_Units unit = null;

	public Vector3 click_position = Vector3.zero;
	public Vector3 center_position = Vector3.zero;
	public Vector3 current_position = Vector3.zero;
	public Vector3 collider_size = Vector3.zero;

	//Ограничиваем движение камеры кнопками:
	public float max_z_coords = 10f;
	public float min_z_coords = 10f;
	public float max_x_coords = 10f;
	public float min_x_coords = 10f;
	
	private Vector3 result = Vector3.zero;
	
//Переключением типа меняем поведение:
	public TypesCamera cameraTypes;
	public enum TypesCamera
	{
		followtounit,
		free,
	}

	void Start ()
	{
		angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		if (GetComponent<Rigidbody> ()) {
			GetComponent<Rigidbody> ().freezeRotation = true;
		}
		cam_transform = this.gameObject.transform;
		//ладно, сделаю проще, сохраню начальное положение камеры:
		start_cam_transform.position = this.transform.position;
		start_cam_transform.rotation = this.transform.rotation;
	}
	void FixedUpdate ()
	{
		WorldFreeCameraMetod ();
	}

	void Update ()
	{

		deselect_all_units ();
		StartCoroutine ("getPositionClick");
	}

	//Поведение камеры:
	private void WorldFreeCameraMetod ()
	{
		switch (cameraTypes) {
		case TypesCamera.free:
			//Формирую вектор перемещения(не стал вращать карту)
			movedirection = Vector3.zero + (Vector3.right * (Input.GetAxis ("Horizontal") * leftrightspeed)) 
				+ (Vector3.forward * (Input.GetAxis ("Vertical") * upbackspeed));
			this.gameObject.transform.position = (transform.position + movedirection * Time.deltaTime);
			//теперь задаем границы карты по X:
			if (cam_transform.position.x > max_x_coords) {
				cam_transform.position = new Vector3 (max_x_coords, cam_transform.position.y, cam_transform.position.z);
			}
			if (cam_transform.position.x < min_x_coords) {
				cam_transform.position = new Vector3 (min_x_coords, cam_transform.position.y, cam_transform.position.z);
			}
			//теперь задаем границы карты по Y:
			if (cam_transform.position.z > max_z_coords) {
				cam_transform.position = new Vector3 (cam_transform.position.x, cam_transform.position.y, max_z_coords);
			}
			if (cam_transform.position.z < min_z_coords) {
				cam_transform.position = new Vector3 (cam_transform.position.x, cam_transform.position.y, min_z_coords);
			}
			break;
		case TypesCamera.followtounit:
			if (FolowObject != null) {
				float wantedRotationAngle = FolowObject.transform.eulerAngles.y;	
				float wantedHeight = FolowObject.transform.position.y + height;	
				float currentRotationAngle = transform.eulerAngles.y;
				float currentHeight = transform.position.y;
				currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
				currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
				Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
				transform.position = FolowObject.transform.position;
				transform.position -= currentRotation * Vector3.forward * distance;
				this.transform.position = (new Vector3 (transform.position.x, currentHeight, transform.position.z));
				transform.LookAt (FolowObject.transform);
			}
			break;
		}
	}

	//Метод возвращает позицию клика: 
	public IEnumerator getPositionClick ()
	{
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100)) {
				//Если игрок кликает на другого юнита игрока:
				if (hit.collider.gameObject.tag == "player_unit") {
					if (hit.collider.gameObject != GameManager.Instance.active_unit) {
						//Debug.Log ("Ты кликнул на другого юнита");
						//Проверяем если выбрана не группа юнитов а один единственный, снимаем выдление с текущего и выделяем нового:
						if (GameManager.Instance.selected_unit_List.Count == 1) {
							GameManager.Instance.deselect_active_unit ();
						}
						//Теперь отмечаем новый юнит как помеченный:
						GameManager.Instance.active_unit = hit.collider.gameObject.GetComponent<Movement_Units> ();
						GameManager.Instance.active_unit.setSelected ();
					}
				} else 
				//Игрок выделил казарму:
				if (hit.collider.gameObject.tag == "kazarm_melee") {
					hit.collider.gameObject.GetComponent<Kazarm> ().kazarm_selected = true;
					Debug.Log ("Игрок выбрал казарму пехотинцев");
					deselect_all_units ();
					//Отображаем интерфейс управления казармой:
					GameManager.Instance.ui = GameManager.ui_type.melee_kazarm;
				} else if (hit.collider.gameObject.tag == "kazarm_distance") {
					hit.collider.gameObject.GetComponent<Kazarm> ().kazarm_selected = true;
					Debug.Log ("Игрок выбрал казарму стрелков");
					deselect_all_units ();
					//Отображаем интерфейс управления казармой:
					GameManager.Instance.ui = GameManager.ui_type.distance_kazarm;
				} else if (hit.collider.gameObject.tag == "enemy") {
					//итак игрок кликнул на врага, указываем всем подконтрольным унитом данного врага как цель:
					for (int i = 0; i < GameManager.Instance.selected_unit_List.Count; i++) {
						GameManager.Instance.selected_unit_List [i].active_enemy = hit.collider.gameObject.GetComponent<Movement_Units> ();
						//ну и переводим их в атаку
						GameManager.Instance.selected_unit_List [i].order = Movement_Units.minion_order.atack;
					}
				} else {
					if (GameManager.Instance.active_unit != null) {
						//Передаем нашему подконтрольному юниту кординаты для дислокации:
						if (GameManager.Instance.selected_unit_List.Count > 0) {
							for (int i = 0; i < GameManager.Instance.selected_unit_List.Count; i++) {
								Movement_Units select_unit = GameManager.Instance.selected_unit_List [i];
								select_unit.setDestination (hit.point);
								select_unit.order = Movement_Units.minion_order.empty;
							}
						}	
					}
					Debug.DrawLine (ray.origin, hit.point);
					result = hit.point;
					result.y = 0f;
				}

			}
		}
		click_position = result;
		yield return new WaitForSeconds (2f);
		//Итак стартую выделение:
		selectBoxSone (click_position);
		yield return null;
	}

	//Реализую выделение(пока без визуализации):
	public void selectBoxSone (Vector3 start_position)
	{
		//Итак игрок шелкнул, и удерживает левую кнопку:
		if (Input.GetMouseButton (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100)) {
				//hit.point это и есть текущая позиция курсора.
				current_position = hit.point;
				Debug.DrawLine (ray.origin, hit.point, Color.red);
			}
			//теперь ищем центр, куда будем устанавливать бокс выделения:
			center_position.x = ((start_position.x + current_position.x) / 2);
			center_position.y = 0;
			center_position.z = ((start_position.z + current_position.z) / 2);

			//Перемещаем выделение:
			selectBox.transform.position = center_position;

			//теперь масштабируем наш бокс выделения:
			//нам нужны x и z(y константа), значения это разница между центром и стартовой позицией помодулю
			//Debug.Log ("x = " + Mathf.Abs (current_position.x - start_position.x));
			//Debug.Log ("z = " + Mathf.Abs (current_position.z - start_position.z));

			collider_size.x = Mathf.Abs (current_position.x - start_position.x);
			collider_size.y = 10f;
			collider_size.z = Mathf.Abs (current_position.z - start_position.z);

			boxCollider.size = collider_size;
			//дактивируем рамку выбора:
			selectBox.GetComponent<SelectManager> ().StartCoroutine ("deactivate");
		}
	}

	//при нажатии на правую кнопку мыши снимаем выделение с юнитов:
	public void deselect_all_units ()
	{
		//Отключаем активные здания:
		if (Input.GetMouseButtonDown (1)) {
			//Если мы были в казарме:
			if (GameManager.Instance.ui == GameManager.ui_type.distance_kazarm || GameManager.Instance.ui == GameManager.ui_type.melee_kazarm) {
				GameManager.Instance.disable_ui ();
				GameManager.Instance.ui = GameManager.ui_type.empty;
			}
		}
		if (GameManager.Instance.selected_unit_List.Count > 1) {
			if (Input.GetMouseButtonDown (1)) {
				if (GameManager.Instance.active_unit != null) {
					GameManager.Instance.active_unit = null;
				}
				for (int i = 0; i < GameManager.Instance.selected_unit_List.Count; i++) {
					Movement_Units next = GameManager.Instance.selected_unit_List [i];
					next.setDeselected ();
				}
				GameManager.Instance.selected_unit_List.Clear ();
			}
		} else {
			if (Input.GetMouseButtonDown (1)) {
				GameManager.Instance.deselect_active_unit ();
			}
		}
	}

	public void camera_type ()
	{
		switch (cameraTypes) {
		case TypesCamera.followtounit:
			cameraTypes = TypesCamera.free;
			this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x, 37f, this.gameObject.transform.position.z);
			this.gameObject.transform.rotation = start_cam_transform.rotation;
			break;
		case TypesCamera.free:
			cameraTypes = TypesCamera.followtounit;
			break;
		}

	}
}
