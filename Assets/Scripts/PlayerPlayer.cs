using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerPlayer : MonoBehaviour {
    private bool canMove = true, moving = false;
    private int speed = 5, buttonCooldown = 0, foodLossHealthGainTic = 5;
    public int damage = 5;
    public int pointsPerApple = 10, pointsPerCheese = 20, pointsPerSteak = 50;
    public float restartLevelDelay = 1f;
    public LayerMask blockingLayer;
    private Text foodText;
    private Text healthText;
    private Animator animator;
    private int food;
    private int health;
    private Vector3 pos;
    private BoxCollider2D boxCollider;
   

    // Use this for initialization
    protected void Start () {
        animator = GetComponent<Animator>();
        Camera.main.GetComponent<CompleteCameraController>().target = transform;
        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        food = GameManager.instance.playerFoodPoints;
        health = GameManager.instance.playerHealth;
        foodText.text = "Food: " + food;
        healthText.text = "Health: " + health;
        boxCollider = GetComponent<BoxCollider2D>();

    }
	
	// Update is called once per frame
	void Update () {
        buttonCooldown--;

        //if (!GameManager.instance.playersTurn) return;

        if (canMove) {
            pos = transform.position;
            TryMove();
        }

        if(moving) {
            if(transform.position == pos) {
                moving = false;
                canMove = true;

                TryMove();
            }
            transform.position = Vector3.MoveTowards(transform.position , pos , speed * Time.deltaTime);

            GameManager.instance.playersTurn = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Exit") {
            Invoke("Restart" , restartLevelDelay);
            enabled = false;
        } else if (collision.tag == "Apple") {
            food += pointsPerApple;
            foodText.text = "+" + pointsPerApple + " Food: " + food;
            //SoundManager.instance.RandomizeSfx(eatSound1 , eatSound2);
            collision.gameObject.SetActive(false);
        } else if (collision.tag == "Cheese") {
            food += pointsPerCheese;
            foodText.text = "+" + pointsPerCheese + " Food: " + food;
            //SoundManager.instance.RandomizeSfx(drinkSound1 , drinkSound2);
            collision.gameObject.SetActive(false);
        } else if(collision.tag == "Steak") {
            food += pointsPerSteak;
            foodText.text = "+" + pointsPerSteak + " Food: " + food;
            collision.gameObject.SetActive(false);
        }

    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseFood(int loss)
    {
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    public void LoseHealth(int loss)
    {
        health -= loss;
        healthText.text = "-" + loss + "Health: " + health;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0 || health <= 0) {
            animator.SetTrigger("GoobertSplat");
            GameManager.instance.GameOver();
            SoundManager.instance.musicSource.Stop();
            //SoundManager.instance.PlaySingle(gameOverSound);
        }
    }

    private bool CanMove(Vector3 dir)
    {
        Vector3 start = transform.position;
        Vector3 end = start + dir;
        boxCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(start , end , blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null) {
            return true;
        }

        Enemy enemy = hit.transform.GetComponent<Enemy>();
        if (enemy != null)
            animator.SetTrigger("GoobertAttack");
            enemy.LoseHealth(damage);
        return false;
    }

    private void DeductFoodIncreaseHealth()
    {
        foodLossHealthGainTic--;
        if (foodLossHealthGainTic <= 0) {
            foodLossHealthGainTic = 5;
            food--;
            health++;
            foodText.text = "Food: " + food;
            healthText.text = "Health: " + health;
        }
    }

    private void TryMove()
    {
        if (buttonCooldown <= 0) {

            buttonCooldown = 5;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                DeductFoodIncreaseHealth();
            if (Input.GetKey(KeyCode.W) && CanMove(Vector3.up)) {               
                canMove = false;
                moving = true;
                pos += Vector3.up;
               
            } else if (Input.GetKey(KeyCode.S) && CanMove(Vector3.down)) {
                canMove = false;
                moving = true;
                pos += Vector3.down;
                
            } else if (Input.GetKey(KeyCode.A) && CanMove(Vector3.left)) {
                canMove = false;
                moving = true;
                pos += Vector3.left;
            } else if (Input.GetKey(KeyCode.D) && CanMove(Vector3.right)) {
                canMove = false;
                moving = true;
                pos += Vector3.right;
            }
        }
    }
}
