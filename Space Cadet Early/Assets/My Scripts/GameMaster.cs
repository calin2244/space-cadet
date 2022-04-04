using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;
    private static int score = 0;
    Player player;
    [SerializeField] private TextMeshPro scoreText;

    void Awake()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameMaster>();
        }
        player = FindObjectOfType<Player>();
    }

    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
    }

    public void KillEnemy(EnemyFollow _enemy)
    {
        Destroy(_enemy.gameObject);
        score += 15;
        scoreText.text =score.ToString();
    }

    public void KillDisaster(AsteroidMovement _asteroid)
    {
        Destroy(_asteroid.gameObject);
        score += 5;
        scoreText.text = score.ToString();
    }

    public void KillAllEnemies()
    {
        Player player;
        player = FindObjectOfType<Player>();
        EnemyFollow enemy;
        enemy = FindObjectOfType<EnemyFollow>();
        if (player == null)
            Destroy(enemy.gameObject);
    }

    private void Update()
    {
        KillAllEnemies();
    }
}
