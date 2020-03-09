using UnityEngine;
using System.Collections;

public class weapon : MonoBehaviour
{
	//Итак этот скрипт будет добавлен любому стрелковому/и нестрелковому оружию

	public float atackDistance = 30f;                          //актуальная дистанция для текущего оружия
	public float energyCoast = 40f;                            //стоимость выстрела в ходе
	public float skill = 1f;                                   //текущее владение этим оружием (зависит вероятность попадания)
	public int atackPower = 50;                                //урон
	public bool orderSchoot = false;                           //есть ли приказ стрелять:

	public int maxAmmo = 100;                            // количество дополнительных патронов 
	public int maxAmmoInClip = 30;                        // емкость магазина 
	public int gunAmmo = 0;                                // количество патронов в магазине 

	// режимы стрельбы 
	public bool SingleShoot = true;                       // одиночными 
	public bool BurstAuto = true;                        // очередями с отсечкой по 3 выстрела 
	public bool FullAuto = true;                        // непрерывними очередями 

	public AudioClip fireSound;                            // звук выстрела 
	public AudioClip reloadSound;                          // звук перезарядки

	//Для облегчения себе жизни просто отображаем требуемые анимации(название их состояний)
	public string idle = "idle";
	public string fire = "schoot";
	public string reload = "reload";

	AudioSource audio;                                    // переменная для хранения компонента AudioSource который мы добавим позже 
	Animation anim;                                        // переменная для хранения компонента Animation который мы добавим позже

	public int modes = 0;                                // переменная для определения режимов стрельбы 
	public bool canShoot = true;                        // триггер 
	public int currentMode;                                // текущий режим стрельбы 
	public int bulletsToGo;                                // переменная необходимая для реализации режима стрельбы очередями с отсечкой по 3 выстрела 

	public void Awake ()
	{
		audio = gameObject.AddComponent<AudioSource> ();    // добавляем компонент AudioSource 

		GunModes (SingleShoot, BurstAuto, FullAuto);        // используем метод GunModes для определения какие режимы стрельбы у нас доступны 

		int B = 0;                                        // временная переменная для определения текущего режима стрельбы 
		while (BitRead(modes, B) == 0) {
			B++;
		}            // если данный режим не поддерживается, то проверяем следующий режим 
		currentMode = B;                                // как найдем первый поддерживаемый режим назначаем его текущим 

		gunAmmo = maxAmmoInClip;                        // кладем в магазин патроны в колистве емкости магазина 
	}

	public void Update ()
	{
		/*
        //анимация бездействия воспроизводится все время 
        unit.animationState = Unit.unitState.idle;

        // при нажатии на левую кнопку мыши и если нам разрешено стрелять 
        if (Input.GetMouseButtonDown(0) & canShoot)
        {
        */
		if (orderSchoot == true & canShoot) {
			// выключаем триггер (то есть уже стрелять нельзя) 
			canShoot = false;
			orderSchoot = false;
			// если текущий режим стрельбы очередью с отсечкой по 3 выстрела, то переменной bulletsToGo 
			// присваиваем значение равное 2 (то есть 0, 1, 2 - три выстрела) 
			if (currentMode == 1) {
				bulletsToGo = 2;
			}
			// стреляем 
			Shoot ();
		}
		/*
        }
        */

		// переключение между режимами стрельбы с помощью Num1, Num2 и Num3 
		if (Input.GetKeyDown (KeyCode.Keypad1)) {
			ChangeGunMode (0);
		}
		if (Input.GetKeyDown (KeyCode.Keypad2)) {
			ChangeGunMode (1);
		}
		if (Input.GetKeyDown (KeyCode.Keypad3)) {
			ChangeGunMode (2);
		}
	}

	// Метод стрельбы 
	public void Shoot ()
	{
		// уменьшаем количество патронов в магазине 
		gunAmmo--;
		// воспроизводим звук выстрела 
		audio.PlayOneShot (fireSound);
		// воспроизводим анимацию выстрела 
		//anim.CrossFadeQueued(fire.name, 0.3f, QueueMode.PlayNow);
		//unit.animationState = Unit.unitState.schoot;
		// (услови №1) если патронов в магазине меньше 1... 
		if (gunAmmo < 1) {
			// выключаем триггер (то есть стрелять уже нельзя) 
			canShoot = false;
			// если количество дополнительных патронов больше 0... 
			if (maxAmmo > 0)
                // то перезаряжаем оружие 
				StartCoroutine (CoroutineReload ());
		} else {// если условие №1 не выполняется... 
			// то переходим к сопрограмме связанной со стрельбой 
			StartCoroutine (CoroutineShoot ());
		}
	}

	// index0 - стрельба одиночными 
	// index1 - стрельба очередью с отсечкой по 3 выстрела 
	// index2 - непрерывными очередями 
	public void GunModes (bool index0, bool index1, bool index2)
	{
		if (index0) {
			modes = BitOn (modes, 0);
		}
		if (index1) {
			modes = BitOn (modes, 1);
		}
		if (index2) {
			modes = BitOn (modes, 2);
		}
	}

	// index0 -  переменная для определения режимов стрельбы 
	// index1 - режим стрельбы (0, 1, 2) 
	public int BitOn (int index0, int index1)
	{
		return index0 | (1 << index1);
	}

	public int BitRead (int modes, int B)
	{
		return (modes & (1 << B)) >> B;
	}

	// index0 - режим стрельбы (0, 1, 2) 
	public int ChangeGunMode (int index0)
	{
		if (BitRead (modes, index0) == 1) {
			currentMode = index0;
		}
		return index0;
	}

	// сопрограмма стрельбы 
	public IEnumerator CoroutineShoot ()
	{
		// текущий режим стрельбы 
		switch (currentMode) {
		// стрельба одиночными 
		case 0:
                // небольшая задержка 
			yield return new WaitForSeconds (0.1f);
                // разрешаем стрелять 
			canShoot = true;
                // выходим с сопрограммы 
			yield break;
			break;

		// стрельба очередью с отсечкой по 3 выстрела 
		case 1:
                // небольшая задержка 
			yield return new WaitForSeconds (3f);
                // если еще не все выстрелы произвели... 
			if (bulletsToGo > 0) {
				// то уменьшаем нашу переменную 
				bulletsToGo--;
				// и еще раз стреляем 
				Shoot ();
			} else {// если все выстрелы уже произвели... 
				// то небольшая задержка 
				yield return new WaitForSeconds (0.1f);
				// разрешаем стрелять 
				canShoot = true;
				// выходим с сопрограммы 
				yield break;
			}
			break;

		// стрельба очередью 
		case 2:
                // небольшая задержка 
			yield return new WaitForSeconds (0.1f);
                // если зажата левая кнопка мыши 
			if (Input.GetButton ("Fire1")) {
				// стреляем 
				Shoot ();
			} else { // если не зажата левая кнопка мыши... 
				// включаем триггер (стрелять можно) 
				canShoot = true;
				// выходим с сопрограммы 
				yield break;
			}
			break;
		}
	}

	// сопрограмма перезарядки 
	public IEnumerator CoroutineReload ()
	{
		// воспроизводим анимацию перезарядки 
		//anim.PlayQueued(reload.name);
		//unit.animationState = Unit.unitState.reload;
		audio.PlayOneShot (reloadSound);
		// делаем задержку равную длине анимации перезарядки + 0.5 секунд 
		yield return new WaitForSeconds (3 + 0.5f);

		// вводим временную переменную (она служит для красоты перезарядки) 
		var ammo = 0;
		// если у нас были патроны в магазине то нашей временной переменной присваиваем значение оставшихся патронов 
		if (gunAmmo > 0) {
			ammo = gunAmmo;
			gunAmmo = 0;
		}
		// (условие №2)если дополнительных патронов меньше чем максимальная емкость магазина... 
		if (maxAmmoInClip > maxAmmo) {
			// (условие №3) если количество дополнительных патронов + оставшихся в магазине больше максимальной емкости магазина... 
			if (maxAmmo + ammo > maxAmmoInClip) {
				// то кладем в магазин патроны в каличестве максимального его объема 
				gunAmmo = maxAmmoInClip;
				// а дополнительные патроны считаем по формуле: дополнительные патроны = дополнительные патроны + оставшиеся патроны - объем магазина 
				maxAmmo = maxAmmo + ammo - maxAmmoInClip;
			} else {// если условие №3 не выполняется... 
				// то кладем в магазин патроны в количетсве равное дополнительные патроны + те что остались 
				gunAmmo = maxAmmo + ammo;
				// а дополнительные патроны приравниваем нулю 
				maxAmmo = 0;
			}
		} else {// если условие №2 не выполняется... 
			// то кладем в магазин патроны в каличестве максимального его объема 
			gunAmmo = maxAmmoInClip;
			// а дополнительные патроны считаем по формуле: дополнительные патроны = дополнительные патроны - объем магазина + оставшиеся 
			maxAmmo = maxAmmo - maxAmmoInClip + ammo;
		}
		// включаем триггер (стрелять можно) 
		canShoot = true;
	}
}
