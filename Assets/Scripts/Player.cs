using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerPotion = 20;
    public float restartLevelDelay = 1f;
    private Text foodText;
    private Text healthText;
    private Animator animator;
    private int food;
    private int health;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    // Use this for initialization
    protected override void Start () {
        animator = GetComponent<Animator>();
        Camera.main.GetComponent<CompleteCameraController>().target = transform;
        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        food = GameManager.instance.playerFoodPoints;
        health = GameManager.instance.playerHealth;
        foodText.text = "Food: " + food;
        base.Start();
	}

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    protected override void AttemptMove <T> (int xDir , int yDir)
    {
        food--;
        foodText.text = "Food: " + food;
        base.AttemptMove <T> (xDir , yDir);
        RaycastHit2D hit;
        if (Move(xDir , yDir , out hit))
            SoundManager.instance.RandomizeSfx(moveSound1 , moveSound2);

        CheckIfGameOver();

        //GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Exit") {
            Invoke("Restart" , restartLevelDelay);
            enabled = false;
        }
        else if(collision.tag == "Food") {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1 , eatSound2);
            collision.gameObject.SetActive(false);
        }
        else if(collision.tag == "Potion") {
            health += pointsPerPotion;
            healthText.text = "+" + pointsPerPotion + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1 , drinkSound2);
            collision.gameObject.SetActive(false);
        }

    }
    protected override void OnCantMove <T> (T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0) {
            GameManager.instance.GameOver();
            SoundManager.instance.musicSource.Stop();
            SoundManager.instance.PlaySingle(gameOverSound);
        }
    }

    // Update is called once per frame
    void Update () {

        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal , vertical);
	}


}
