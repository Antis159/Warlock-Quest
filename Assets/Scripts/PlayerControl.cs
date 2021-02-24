using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    private Transform playerMoveGO;
    public LayerMask stopMoveMask;
    public LayerMask enemyMask;
    public LayerMask buildingMask;
    public LayerMask sceneSwitchMask;
    [Space]

    public float moveSpeed = 5f;
    private MoveCamera _camera;
    public bool inCombat;
    public bool isDead = false;
    public bool stopMove = false;
    public bool pathFindingMove = false;
    private Pathfinding pathfinding;
    private List<Vector3> movePath = new List<Vector3>();
    public int currentFloorLevel;

    private Vector3 moveDir;
    private GameObject lastEnemy;
    private Vector3 lastEnemyPos;

    //Animations
    private Animator anim;
    public RuntimeAnimatorController ac_Up;
    public RuntimeAnimatorController ac_Right;
    public RuntimeAnimatorController ac_Down;
    public RuntimeAnimatorController ac_Left;
    [Space]

    //Attack
    public GameObject fireOrbAttack;
    [Space]

    //Buttons
    bool upPressed;
    bool rightPressed;
    bool downPressed;
    bool leftPressed;
    [Space]

    //Stats
    public int damage;
    public float attackSpeed;
    public int maxHealth;
    public int currentHealth;
    public int maxMana;
    public int currentMana;

    public Inventory inventory;
    public UIController _UIController;
    public GridStorage gridStorage;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        _camera = Camera.main.GetComponent<MoveCamera>();
        gridStorage = gameObject.GetComponent<GridStorage>();
        anim = GetComponent<Animator>();
        playerMoveGO = transform.GetChild(0).transform;
        playerMoveGO.parent = null;
        currentHealth = maxHealth;
        currentMana = maxMana;
        pathfinding = GetComponent<Pathfinding>();
    }

    void Update()
    {
        CheckHealth_Mana();
        IsDead();
        Move();
    }

    public void ReceiveDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void IsDead()
    {
        if (currentHealth <= 0)
        {
            isDead = true;
            CombatEnd();
        }
        else
            isDead = false;
    }
    public void CombatStart(GameObject enemy)
    {
        _camera.combat = true;

        SetAllButtonFalse();
        lastEnemy = enemy;
        lastEnemyPos = enemy.transform.position;
        inCombat = true;
        enemy.GetComponent<Enemy>().StartCombat();
        StartCoroutine(SimulateCombat());
    }

    public void CombatEnd()
    {
        inCombat = false;
        lastEnemy.GetComponent<Enemy>().EndCombat();
        //if (!IsLastEnemy())
        //    MoveToDeadEnemy();

        //lastEnemy = null;
    }
    IEnumerator SimulateCombat()
    {
        Enemy enemy = lastEnemy.GetComponent<Enemy>();
        while (inCombat)
        {
            yield return new WaitForSeconds(attackSpeed);
            Instantiate(fireOrbAttack, lastEnemyPos, Quaternion.identity);
            enemy.ReceiveDamage(damage);

            if (enemy.GetCurrentHealth() <= 0)
            {
                CombatEnd();
                break;
            }
        }
    }
    public void CheckHealth_Mana()
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (currentMana > maxMana)
            currentMana = maxMana;
    }
    
    public void MoveToDeadEnemy()
    {
        Vector3 dir = lastEnemyPos - transform.position;
        if (dir == Vector3.up)
        {
            UpButton();
            Invoke("UpButton", 0.1f);
        }
        else if (dir == Vector3.right)
        {
            RightButton();
            Invoke("RightButton", 0.1f);
        }
        else if (dir == Vector3.down)
        {
            DownButton(); 
            Invoke("DownButton", 0.1f);
        }
        else
        {
            LeftButton();
            Invoke("LeftButton", 0.1f);
        }
    }

    public void MoveToPoint(Vector3 movePoint)
    {
        if (!stopMove)
        {
            playerMoveGO.position = movePoint;
            transform.position = movePoint;
        }
        else
            Debug.Log("MoveToPoint function didn't work");
    }
    IEnumerator LoadScene(string sceneToLoad)
    {
        gridStorage.StoreGrid();
        _UIController.FadeScreen(0.5f);
        stopMove = true;

        if (SceneManager.GetActiveScene().name == "CaveScene" && sceneToLoad == "CaveScene")
        {
            currentFloorLevel += 1;
        }
        else
            currentFloorLevel = 1;
        _UIController.floorLevel.text = $"Floor {currentFloorLevel}";

        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(sceneToLoad);

        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.1f);

        gridStorage.LoadGrid();

        SceneSwitchGO temp = GameObject.FindGameObjectWithTag("SceneStartPos").GetComponent<SceneSwitchGO>();
        Vector3 movePoint = temp.transform.position + temp.offset;

        SetAllButtonFalse();
        stopMove = false;
        MoveToPoint(movePoint);
    }

    public void Move()
    {
        if (!inCombat)
        {
            if (!stopMove)
            {
                if(!pathFindingMove)
                {
                    if (Vector3.Distance(transform.position, playerMoveGO.position) <= .05f)
                    {
                        if (moveDir != Vector3.zero)
                        {
                            if (Physics2D.OverlapCircle(playerMoveGO.position + moveDir, 0.2f, sceneSwitchMask))
                            {
                                RaycastHit2D hit;
                                Vector3 direction = Vector3.Normalize(moveDir);
                                if (hit = Physics2D.Raycast(playerMoveGO.position, direction))
                                {
                                    SceneSwitchGO temp = hit.collider.GetComponent<SceneSwitchGO>();
                                    StartCoroutine(LoadScene(temp.sceneToLoad));
                                }
                            }
                            //EnemyCollision
                            else if (Physics2D.OverlapCircle(playerMoveGO.position + moveDir, 0.2f, enemyMask))
                            {
                                RaycastHit2D hit;
                                Vector3 direction = Vector3.Normalize(moveDir);
                                if (hit = Physics2D.Raycast(playerMoveGO.position, direction))
                                {
                                    CombatStart(hit.collider.gameObject);
                                }
                                moveDir = Vector3.zero;
                            }
                            //BuildingCollision
                            else if (Physics2D.OverlapCircle(playerMoveGO.position + moveDir, 0.2f, buildingMask))
                            {
                                RaycastHit2D hit;
                                Vector3 direction = Vector3.Normalize(moveDir);
                                if (hit = Physics2D.Raycast(playerMoveGO.position, direction))
                                {
                                    Debug.Log(hit.collider.gameObject.name);
                                }
                                moveDir = Vector3.zero;
                            }
                            //NoCollision
                            else if (!Physics2D.OverlapCircle(playerMoveGO.position + moveDir, 0.2f, stopMoveMask))
                                playerMoveGO.position += moveDir;
                        }
                    }
                    transform.position = Vector3.MoveTowards(transform.position, playerMoveGO.position, moveSpeed * Time.deltaTime);
                }
                else if(pathFindingMove)
                {
                    if (Input.GetMouseButtonDown(0) && !_UIController.IsPointerOverUI() && movePath.Count == 0 && !_UIController.isBuilding)
                    {
                        Vector3 clickedTile = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        if(Physics2D.OverlapCircle(clickedTile, 0.2f, buildingMask) ||
                           Physics2D.OverlapCircle(clickedTile, 0.2f, sceneSwitchMask) ||
                           Physics2D.OverlapCircle(clickedTile, 0.2f, enemyMask))
                        {
                            movePath = pathfinding.GetMovePath(new Vector3(Mathf.RoundToInt(clickedTile.x), Mathf.RoundToInt(clickedTile.y), 0), transform.position, true);
                        }
                        else if (!Physics2D.OverlapCircle(clickedTile, 0.2f, stopMoveMask))
                        {
                            movePath = pathfinding.GetMovePath(new Vector3(Mathf.RoundToInt(clickedTile.x), Mathf.RoundToInt(clickedTile.y), 0), transform.position, false);
                        }
                    }
                    else if(movePath.Count > 0)
                    {
                        if (movePath[movePath.Count - 1] != playerMoveGO.position && moveDir == Vector3.zero)
                        {
                            if (movePath[movePath.Count - 1].x > playerMoveGO.position.x)
                                RightButton();
                            else if (movePath[movePath.Count - 1].x < playerMoveGO.position.x)
                                LeftButton();
                            else if (movePath[movePath.Count - 1].y > playerMoveGO.position.y)
                                UpButton();
                            else
                                DownButton();

                            playerMoveGO.position += moveDir;
                        }
                        else if (Vector3.Distance(transform.position, playerMoveGO.position) == 0)
                        {
                            movePath.RemoveAt(movePath.Count - 1);
                            moveDir = Vector3.zero;
                        }

                        transform.position = Vector3.MoveTowards(transform.position, playerMoveGO.position, moveSpeed * Time.deltaTime);
                    }
                }
               
            }
        }
        else
        {
            //
        }
    }

    public void MoveBuildingCheck()
    {

    }

    public bool IsLastEnemy()
    {
        if (lastEnemy)
            return true;
        else
            return false;
    }
    public Enemy GetLastEnemy()
    {
        return lastEnemy.GetComponent<Enemy>();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public int GetCurrentMana()
    {
        return currentMana;
    }
    public int GetMaxMana()
    {
        return maxMana;
    }
    //Buttons
    #region

    public void FleeButton()
    {
        CombatEnd();
    }
    public void UpButton()
    {
        if (upPressed)
        {
            upPressed = false;
            moveDir = Vector3.zero;
            return;
        }
        upPressed = true;
        if(moveDir == Vector3.zero)
        {
            if (Vector3.Distance(transform.position, playerMoveGO.position) <= .05f)
            {
                moveDir = Vector3.up;
                anim.runtimeAnimatorController = ac_Up;
            }
        }
    }
    public void RightButton()
    {
        if (rightPressed)
        {
            rightPressed = false;
            moveDir = Vector3.zero;
            return;
        }
        rightPressed = true;
        if (moveDir == Vector3.zero)
        {
            if (Vector3.Distance(transform.position, playerMoveGO.position) <= .05f)
            {
                moveDir = Vector3.right;
                anim.runtimeAnimatorController = ac_Right;
            }
        }
    }
    public void DownButton()
    {
        if (downPressed)
        {
            downPressed = false;
            moveDir = Vector3.zero;
            return;
        }
        downPressed = true;
        if (moveDir == Vector3.zero)
        {
            if (Vector3.Distance(transform.position, playerMoveGO.position) <= .05f)
            {
                moveDir = Vector3.down;
                anim.runtimeAnimatorController = ac_Down;
            }
        }
    }
    public void LeftButton()
    {
        if (leftPressed)
        {
            leftPressed = false;
            moveDir = Vector3.zero;
            return;
        }
        leftPressed = true;
        if (moveDir == Vector3.zero)
        {
            if (Vector3.Distance(transform.position, playerMoveGO.position) <= .05f)
            {
                moveDir = Vector3.left;
                anim.runtimeAnimatorController = ac_Left;
            }
        }
    }

    public void SetAllButtonFalse()
    {
        upPressed = false;
        rightPressed = false;
        downPressed = false;
        leftPressed = false;
    }
    #endregion

}
