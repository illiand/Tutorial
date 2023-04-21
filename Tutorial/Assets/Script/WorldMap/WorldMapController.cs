using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldMapController : MonoBehaviour
{
    public GameObject spotsCollection;
    public GameObject mapLayout;
    public GameObject battleLayout;

    public GameObject playerSpot;

    public GameObject finalResultText;

    private Map map = new Map();
    private PlayerStatus playerStatus = new PlayerStatus();

    private int talkProgress = 0;//记录闲聊进度
    private GameObject girl;

    void Start()
    {
      initMap();
      initSpotConnection();

      initColorOnSpot();
      initNextSpotHint();
    }

    public Map getMap()
    {
      return map;
    }

    public PlayerStatus getPlayerStatus()
    {
      return playerStatus;
    }

    //move player from @param curID to @param nextID
    private void playerMoveTo(int curId, int nextID)
    {
      for(int i = 0; i < map.spots[curId].nextSpotIDs.Length; i += 1)
      {
        if(map.spots[curId].nextSpotIDs[i] == nextID)
        {
          playerStatus.currentSpotID = nextID;
          playerStatus.movingCount += 1;
          map.spots[nextID].isTriggered = true;

          initColorOnSpot();
          initNextSpotHint();
                

                triggerEvent();

          return;
        }
      }
    }

    //triggers after moving
    private void triggerEvent()
    {
        if (checkVisitedSpots() == 15)
        {
            setFamiliesPositon();
        }

        if (map.spots[playerStatus.currentSpotID].type == 1)
        {
            //找家人
            GetComponent<TalkController>().layout.GetComponent<GameController>().PlaySceneNow(10);
            return;
        }

        if (playerStatus.movingCount == 1)
        {
            //GetComponent<Talk>().startEvent(0);
            GetComponent<TalkController>().layout.GetComponent<GameController>().PlaySceneNow(2);
            //少女登场跟随主角
            girl.SetActive(true);

        }

        if (playerStatus.movingCount == 3)
        {
            map.obj.SetActive(false);
            battleLayout.SetActive(true);

            for (int i = 0; i < 10; i += 1)
            {
                GetComponent<Controller>().characters[i].SetActive(false);
            }

            GetComponent<Controller>().summon(0, "Alice", "Alice", 2000, 70, 100, 100, 35, new int[] { 1, 2, 3 });
            GetComponent<Controller>().summon(1, "Ada", "Ada", 1200, 100, 200, 30, 40, new int[] { 4, 5 });
            GetComponent<Controller>().summon(2, "Amy", "Amy", 1200, 150, 125, 50, 35, new int[] { 6, 7 });
            summonMonsterNow();
            GetComponent<Controller>().startExistingGame();
        }

        if (playerStatus.movingCount == 4 && (playerStatus.itemRemaining[0] > 0 || (playerStatus.itemRemaining[1] > 0)))
        {
            //food event
            GetComponent<TalkController>().layout.GetComponent<GameController>().PlaySceneNow(9);
            playerStatus.foodEventLeft = Random.Range(2, 4);
        }

        if (playerStatus.movingCount == 5)
        {
            map.obj.SetActive(false);
            battleLayout.SetActive(true);

            summonMonsterNow();
            GetComponent<Controller>().startExistingGame();

            playerStatus.monsterEventLeft = Random.Range(2, 4);
        }

        if (playerStatus.movingCount > 5)
        {
            //BOSS BATTLE
            if (playerStatus.currentSpotID == 20)
            {
                map.obj.SetActive(false);
                battleLayout.SetActive(true);

                GetComponent<Controller>().summon(6, "BOSS", "char_47", 10000, 500, 300, 25, 20, new int[] { 15, 22, 21, 20, 16, 29 });
                GetComponent<Controller>().startExistingGame();


                return;
            }

            if (playerStatus.currentSpotID == 21)
            {
                //ENDING
                finalResultText.SetActive(true);
                finalResultText.GetComponent<TextMeshProUGUI>().text = getFinalResult();
                return;
            }

        if(playerStatus.treasureEventLeft <= 0)
        {
          playerStatus.treasureEventLeft = Random.Range(2, 4);
          GetComponent<TalkController>().layout.GetComponent<GameController>().PlaySceneNow(17);
        }
        else if(playerStatus.foodEventLeft <= 0)
        {
          //food event
          playerStatus.foodEventLeft = Random.Range(2, 4);
          GetComponent<TalkController>().layout.GetComponent<GameController>().PlaySceneNow(9);
        }
        else if(playerStatus.monsterEventLeft <= 0)
        {
          map.obj.SetActive(false);
          battleLayout.SetActive(true);

                summonMonsterNow();
                GetComponent<Controller>().startExistingGame();

                playerStatus.monsterEventLeft = Random.Range(2, 4);
            }
        }

        //少女只在2,4,6步出现 可以看后期怎么改
        if (playerStatus.movingCount%2 == 0)
        {
            playerSpot.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            playerSpot.transform.GetChild(0).GetComponent<Button>().enabled = true;
            Debug.Log("playerSpot.transform.GetChild(0).GetChild(0).gameObject.SetActive(Ture);");


        }
        else
        {
            playerSpot.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            playerSpot.transform.GetChild(0).GetComponent<Button>().enabled = false;
            
            Debug.Log("playerSpot.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);");
        }

      playerStatus.foodEventLeft -= 1;
      playerStatus.monsterEventLeft -= 1;
      playerStatus.treasureEventLeft -= 1;
    }

    //summon monster for enemies
    public void summonMonsterNow()
    {
      int[] monstersToSummon = GetComponent<Controller>().getRandomMonsterPair();

      for(int i = 0; i < monstersToSummon.Length; i += 1)
      {
        GetComponent<Controller>().summonMonster(monstersToSummon[i]);
      }
    }

    //set color whenever move
    private void initColorOnSpot()
    {
      for(int i = 0; i < map.spots.Length; i += 1)
      {
        if(map.spots[i].isTriggered)
        {
          map.spots[i].obj.GetComponent<Image>().color = new Color(1, 1, 0);
        }
        else
        {
          map.spots[i].obj.GetComponent<Image>().color = new Color(1, 0, 0);
        }
      }

      map.spots[playerStatus.currentSpotID].obj.GetComponent<Image>().color = new Color(0, 0, 1);
        playerSpot.transform.position = map.spots[playerStatus.currentSpotID].obj.transform.position;
    }

    //set arraw animation whenever move
    private void initNextSpotHint()
    {
      for(int i = 0; i < map.spots.Length; i += 1)
      {
        map.spots[i].obj.transform.GetChild(1).gameObject.SetActive(false);
      }

      for(int i = 0; i < map.spots[playerStatus.currentSpotID].nextSpotIDs.Length; i += 1)
      {
        int nextID = map.spots[playerStatus.currentSpotID].nextSpotIDs[i];
        map.spots[nextID].obj.transform.GetChild(1).gameObject.SetActive(true);
      }
    }

    private void initMap()
    {
      map.obj = mapLayout;
      map.spots = new Spot[22];

        
        
       

        for (int i = 0; i < spotsCollection.transform.childCount; i += 1)
      {
        Spot curSpot = new Spot();
        curSpot.id = i;
        curSpot.obj = spotsCollection.transform.GetChild(i).gameObject;

        spotsCollection.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.AddListener(
          delegate
          {
            playerMoveTo(playerStatus.currentSpotID, curSpot.id);
          }
        );

        map.spots[i] = curSpot;
        spotsCollection.transform.GetChild(i).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
      }

      map.spots[0].isTriggered = true;


        //开场白结束前少女没出现
        girl = playerSpot.transform.GetChild(0).gameObject;
        girl.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                Debug.Log("talkprogress: " + talkProgress);
                if (talkProgress == 0 && playerStatus.movingCount == 2)
                {

                    talkProgress++;
                    getPlayerStatus().A = talkProgress;
                    GetComponent<TalkController>().layout.GetComponent<GameController>().PlaySceneNow(0);
                    playerSpot.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
                else if (talkProgress == 1 && playerStatus.movingCount >= 4)
                {
                    talkProgress++;
                    getPlayerStatus().A = talkProgress;
                    GetComponent<TalkController>().layout.GetComponent<GameController>().PlaySceneNow(1);
                    playerSpot.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
                else if (talkProgress == 2 && playerStatus.movingCount >= 6)
                {
                    talkProgress++;
                    getPlayerStatus().A = talkProgress;
                    GetComponent<TalkController>().layout.GetComponent<GameController>().PlaySceneNow(3);
                    playerSpot.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
                else if(talkProgress == 3 && playerStatus.movingCount >= 8)
                {
                    talkProgress++;
                    getPlayerStatus().A = talkProgress;
                    GetComponent<TalkController>().layout.GetComponent<GameController>().PlaySceneNow(4);
                    playerSpot.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }

            }
          );
        girl.SetActive(false);

    }

    //init possible move for each spot
    private void initSpotConnection()
    {
      int[][] nextSpotIDsList = new int[22][]
      {
        new int[]{1, 3, 4, 6},
        new int[]{2, 3, 0},
        new int[]{1, 3, 8},
        new int[]{2, 1, 0, 4},
        new int[]{3, 0, 6, 5},

        new int[]{4, 6, 7},
        new int[]{4, 5, 7},
        new int[]{5, 6, 18, 19},
        new int[]{2, 9, 10, 12},
        new int[]{8, 10},

        new int[]{9, 8, 11, 12},
        new int[]{10, 12},
        new int[]{10, 11, 13, 14},
        new int[]{12, 14, 15},
        new int[]{12, 13, 15},

        new int[]{13, 14, 20},
        new int[]{17, 20},
        new int[]{16, 18},
        new int[]{17, 19, 7},
        new int[]{7, 18},

        new int[]{15, 16, 21},
        new int[]{20}
      };

      for(int i = 0; i < map.spots.Length; i += 1)
      {
        map.spots[i].nextSpotIDs = nextSpotIDsList[i];
      }
    }

    private int checkVisitedSpots()
    {
      int visited = 0;

      for(int i = 0; i < map.spots.Length; i += 1)
      {
        if(map.spots[i].isTriggered)
        {
          visited += 1;
        }
      }

      return visited;
    }

    private void setFamiliesPositon()
    {
      ArrayList unVisited = new ArrayList();

      for(int i = 0; i < map.spots.Length; i += 1)
      {
        if(!map.spots[i].isTriggered && map.spots[i].id < 20)
        {
          unVisited.Add(map.spots[i].id);
        }
      }

      for(int i = 0; i < 3; i += 1)
      {
        int posIndex = Random.Range(0, unVisited.Count - 1);
        map.spots[(int)unVisited[posIndex]].type = 1;
        unVisited.RemoveAt(posIndex);
      }
    }

    private string getFinalResult()
    {
      float OResult = playerStatus.O / (float)playerStatus.battleCount;
      float CResult = playerStatus.C / (float)playerStatus.treasureCount;
      float EResult = playerStatus.E / 3f;
      float AResult = playerStatus.A / 8753f;
      float NResult = playerStatus.N / (float)playerStatus.foodCount;
Debug.Log(OResult + " " + CResult + " " + EResult + " " + AResult + " " + NResult);
      return "O: " + getDegree(OResult) + " C: " + getDegree(CResult) + " E: " + getDegree(EResult) + " A: " + getDegree(AResult) + " N: " + getDegree(NResult);
    }

    private string getDegree(float v)
    {
      if(v > 0.667f)
      {
        return "high";
      }
      else if(v > 0.333f)
      {
        return "medium";
      }
      else
      {
        return "low";
      }
    }
}

public class Map
{
  public GameObject obj;
  public Spot[] spots;
}

public class Spot
{
  public int id;
  public GameObject obj;
  public int type;

  //if visited
  public bool isTriggered;

  public int[] nextSpotIDs;
}

public class PlayerStatus
{
  public int currentSpotID;
  public int movingCount;

  public int monsterEventLeft;
  public int foodEventLeft;
  public int treasureEventLeft;

  public int[] itemRemaining;

  public bool[] flag;
  public int battleCount;
  public int treasureCount;
  public int talkCount;
  public int foodCount;

  public float O;
  public float C;
  public float A;
  public float E;
  public float N;

  

  public PlayerStatus()
  {
    //一上来3血药 1蓝药
    itemRemaining = new int[]{3, 1};
    currentSpotID = 0;

    flag = new bool[]{false};
  }
}
