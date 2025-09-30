using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.TextCore.Text;


public class GameManager : MonoBehaviour
{
    public int stage;
    public Animator stageAnim;
    public Animator clearAnim;
    public Animator fadeAnim;
    public Transform playerPos;

    public string[] enemyObjs;
    public Transform[] spawnPoints;

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;
    public ObjectManager objectManager;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
        StageStart();
    }

    public void StageStart()
    {
        //#. Stage UI Load
        stageAnim.SetTrigger("On");
        stageAnim.GetComponent<Text>().text = "Stage " + stage + "\nStart";
        clearAnim.GetComponent<Text>().text = "Stage " + stage + "\nClear!!";
        //#.Enemy Spawn File Read
        ReadSpawnFile();

        //#.Fade in
        fadeAnim.SetTrigger("In");
    }

    public void StageEnd()
    {
        //#.Clear UI Load
        clearAnim.SetTrigger("On");

        //#.Fade Out
        fadeAnim.SetTrigger("Out");
        //#.Player Repos
        player.transform.position = playerPos.position;

        //#.Stage Increament
        stage++;
        if (stage > 2)
            Invoke("GameOver",6);
        else
            Invoke("StageStart", 5);
    }

    void ReadSpawnFile()
    {

        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;


        UnityEngine.TextAsset textFile = Resources.Load("Stage " + stage.ToString()) as UnityEngine.TextAsset;
        StringReader stringReader = new StringReader(textFile.text); ;

        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);

            if (line == null)
                break;


            Spawn spawnDate = new Spawn();
            spawnDate.delay = float.Parse(line.Split(',')[0]);
            spawnDate.type = line.Split(',')[1];
            spawnDate.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnDate);
        }


        stringReader.Close();


        nextSpawnDelay = spawnList[0].delay;

        


    }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > nextSpawnDelay  && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;      
        }

        // #. UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }


    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;

        }
        

        int enemyPoint = spawnList[spawnIndex].point;

        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;     
        enemyLogic.gameManager = this;  
        enemyLogic.objectManager = objectManager;
        if (enemyPoint == 5 || enemyPoint == 6) // #.Right Spawn
        {
            enemy.transform.Rotate(Vector3.back * 45);
            rigid.linearVelocity = new Vector2(enemyLogic.speed * (-1), -1);
        }
        else if (enemyPoint == 7 || enemyPoint == 8)    // #.Left Spawn
        {
            enemy.transform.Rotate(Vector3.forward * 45);
            rigid.linearVelocity = new Vector2(enemyLogic.speed, -1);
        }
        else    //#. Frong Spawn
        {
            rigid.linearVelocity = new Vector2(0, enemyLogic.speed * (-1));
        }


        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        nextSpawnDelay = spawnList[spawnIndex].delay;
    }
    public void UpdateLifeIcon(int life)
    {
       

        // #.UI Life Init Disable
        for (int i=0; i<3; i++)
        {
            lifeImage[i].color = new Color(1, 1, 1, 0);
        }

        // #.UI Life Active
        for(int i=0; i<life; i++)
        {
            lifeImage[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        

        // #.UI Boom Init Disable
        for (int i = 0; i < 3; i++)
        {
            boomImage[i].color = new Color(1, 1, 1, 0);
        }

        // #.UI Boom Active
        for (int i = 0; i < boom; i++)
        {
            boomImage[i].color = new Color(1, 1, 1, 1);
        }
    }
    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject explosion = objectManager.MakeObj("Explosion");
        Explosion explosionLogic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;
        explosionLogic.StartExplosion(type);
    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

}
