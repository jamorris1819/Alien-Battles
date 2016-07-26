using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum WeaponType { Pistol, Rifle, Plasma, Flamethrower, Laser }

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public float score;
    public float bullets;
    public bool canShoot;
    public bool vulnerable;
    public float vulnerableTimeout;
    public static int lives = 3;
    public Weapon currentWeapon;
    public float kickback;
    public float kickbackCamera;
    public float screenShake;
    public bool locked;
    public Vector2 inaccuracy;
    public Transform bulletCase;

    Animator animator;
    Rigidbody2D rbody;
    Vector3 velocity;
    LevelManager levelManager;
    bool reloading = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody2D>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        ChangeWeapon(WeaponType.Pistol);
    }

    public void ChangeWeapon(WeaponType weaponType)
    {
        Weapon weapon = levelManager.GetWeapon(weaponType);
        currentWeapon = weapon;
        animator.runtimeAnimatorController = currentWeapon.animator;
    }

    void FixedUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Z))
            ChangeWeapon(WeaponType.Pistol);
        if (Input.GetKeyDown(KeyCode.X))
            ChangeWeapon(WeaponType.Rifle);
        if (Input.GetKeyDown(KeyCode.C))
            ChangeWeapon(WeaponType.Plasma);
        if (Input.GetKeyDown(KeyCode.V))
            ChangeWeapon(WeaponType.Flamethrower);
        if (Input.GetKeyDown(KeyCode.B))
            ChangeWeapon(WeaponType.Laser);

        if (locked)
        {
            rbody.velocity = Vector2.zero;
            return;
        }

        // Aim the player in the correct direction.
        Vector3 position = levelManager.camera.WorldToScreenPoint(transform.position);
        Vector3 direction = Input.mousePosition - position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector2 velocity = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            velocity.y = 1;
        else if (Input.GetKey(KeyCode.S))
            velocity.y = -1;
        else
            velocity.y = 0;

        if (Input.GetKey(KeyCode.D))
            velocity.x = 1;
        else if (Input.GetKey(KeyCode.A))
            velocity.x = -1;
        else
            velocity.x = 0;

        

        velocity.Normalize();
        animator.SetBool("walking", velocity != Vector2.zero);
        rbody.velocity = velocity * speed;
        if (Input.GetMouseButton(0) && canShoot && !reloading)
        {
            animator.SetTrigger("shoot");
            Transform newBullet = Instantiate(currentWeapon.bullet);
            // Make sure the bullet doesn't appear in the centre of the player.
            float bulletAngle = angle + Random.Range(-currentWeapon.inaccuracy, currentWeapon.inaccuracy);
            Vector2 bulletDirection = Quaternion.AngleAxis(bulletAngle, Vector3.forward) * Vector2.right;
            Vector3 extension = bulletDirection.normalized;
            newBullet.position = transform.position + (currentWeapon.bulletDistance * extension);
            newBullet.rotation = Quaternion.AngleAxis(Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg, Vector3.forward);
            newBullet.GetComponent<Rigidbody2D>().velocity = bulletDirection.normalized * currentWeapon.bulletVelocity;

            if (currentWeapon.type == WeaponType.Plasma)
            {
                int spread = 15;
                Transform newBulletLeft = Instantiate(currentWeapon.bullet);
                Transform newBulletRight = Instantiate(currentWeapon.bullet);
                Quaternion bulletAngleLeft = Quaternion.AngleAxis(bulletAngle - spread, Vector3.forward);
                Quaternion bulletAngleRight = Quaternion.AngleAxis(bulletAngle + spread, Vector3.forward);
                Vector2 bulletDirectionLeft = bulletAngleLeft * Vector2.right;
                Vector2 bulletDirectionRight = bulletAngleRight * Vector2.right;
                Vector3 extensionLeft = bulletDirectionLeft.normalized;
                Vector3 extensionRight = bulletDirectionRight.normalized;
                newBulletLeft.position = transform.position + (currentWeapon.bulletDistance * extensionLeft);
                newBulletRight.position = transform.position + (currentWeapon.bulletDistance * extensionRight);
                newBulletLeft.rotation = bulletAngleLeft;
                newBulletRight.rotation = bulletAngleRight;
                newBulletLeft.GetComponent<Rigidbody2D>().velocity = bulletDirectionLeft.normalized * currentWeapon.bulletVelocity;
                newBulletRight.GetComponent<Rigidbody2D>().velocity = bulletDirectionRight.normalized * currentWeapon.bulletVelocity;
                bullets -= 2;
            }

            levelManager.Shoot();
            rbody.velocity -= (Vector2)direction.normalized * kickback;
            levelManager.cameraFollow.Kickback((Vector2)direction.normalized * -kickbackCamera);
            levelManager.cameraFollow.Shake(screenShake);
            // Drop case.
            Vector2 caseDrop = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            Transform casePiece = (Transform)Instantiate(bulletCase, transform.position, Quaternion.identity);
            casePiece.GetComponent<Rigidbody2D>().velocity = caseDrop;
            casePiece.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-10, 10);
            bullets--;
            canShoot = false;
            if (bullets <= 0)
                StartCoroutine(Reload());
            else
                StartCoroutine(ShootReset());
        }

        if (Input.GetKeyDown(KeyCode.R) && !reloading)
        {
            canShoot = false;
            StartCoroutine(Reload());
        }
    }

    public bool Damage(int amount)
    {
        if (!vulnerable)
            return false;
        health = Mathf.Clamp(health - amount, 0, maxHealth);
        vulnerable = false;
        if (health == 0)
        {
            lives--;
            if (lives != 0)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else
            {
                lives = 3;
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            StartCoroutine(Flash());
            StartCoroutine(VulnerableReset());
        }
        return true;
    }

    IEnumerator Reload()
    {
        reloading = true;
        bullets = 0;
        for (int i = 0; i < 35; i++)
        {
            bullets++;
            yield return new WaitForSeconds(1 / 34);
        }
        canShoot = true;
        reloading = false;
    }

    IEnumerator ShootReset()
    {
        yield return new WaitForSeconds(currentWeapon.timeout);
        canShoot = true;
    }

    IEnumerator VulnerableReset()
    {
        yield return new WaitForSeconds(vulnerableTimeout);
        vulnerable = true;
    }

    IEnumerator Flash()
    {
        Renderer r = GetComponent<Renderer>();

        for (int i = 0; i < 2; i++)
        {
            r.enabled = false;
            yield return new WaitForSeconds(0.1f);
            r.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Bullet"))
        {
            Bullet bullet = collider.transform.GetComponent<Bullet>();
            if(bullet.owner == BulletOwner.Enemy)
            {
                Damage((int)bullet.damage);
                Vector2 direction = bullet.GetComponent<Rigidbody2D>().velocity;
                //rbody.velocity -= (Vector2)direction.normalized * kickback;
                GameObject.Destroy(bullet.gameObject);
            }
        }
    }
}

public class Weapon
{
    public float timeout;
    public string name;
    public float bulletDistance;
    public RuntimeAnimatorController animator;
    public Transform bullet;
    public float bulletVelocity;
    public Sprite icon;
    public WeaponType type;
    public int inaccuracy;

    public Weapon(float delay, string name, float bulletDistance, RuntimeAnimatorController animator, Transform bullet, float bulletVelocity, Sprite icon, WeaponType type, int inaccuracy)
    {
        this.timeout = delay;
        this.name = name;
        this.bulletDistance = bulletDistance;
        this.animator = animator;
        this.bullet = bullet;
        this.bulletVelocity = bulletVelocity;
        this.icon = icon;
        this.type = type;
        this.inaccuracy = inaccuracy;
    }
}
