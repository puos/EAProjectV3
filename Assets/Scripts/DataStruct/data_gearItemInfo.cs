using System.Collections;

[System.Serializable]
public class data_gearItemInfo : EADataInfo {
	public string	id;
	public string	item_name;
	public int	wear_type;	//Weapon = 0 Helmet = 1 Armor = 2  NeckPlace = 3  Ring = 4  Etc = 5
	public int	gear_type;	//Sword = 1  Bow = 2   Rifle = 3   Pistol = 4  Etc = 5 
	public int	grade;
	public float	atk;
	public float	atk_range;
	public string	model_res;
	public string	res_icon;
	public string	projectile_id;
	public float	projectile_speed;
	public string	hit_effect_id;
	public string 	hit_effect_id2;

}
