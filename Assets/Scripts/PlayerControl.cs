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
    private string lastScene;
    public bool inCombat;
    public bool isDead = false;
    public bool stopMove = false;
    public bool pathFindingMove = false;
    private Pathfinding pathfinding;
    private List<Vector3> movePath = new List<Vector3>();
    public int currentFloorLevel;

    public Vector3 moveDir;
    private GameObject lastEnemy;
    private Vector3 lastEnemyPos;
    private CombatSimulate lastCS;

    //Animations
    private Animator anim;
    public RuntimeAnimatorController ac_Up;
    public RuntimeAnimatorController ac_Right;
    public RuntimeAnimatorController ac_Down;
    public RuntimeAnimatorController ac_Left;
    [Space]

    //Attack
    public GameObject combatSimulatorObj;
    public GameObject weaponAttackParticles;
    [Space]

    public GameObject[] allOrbs;

    [Space]
    //Buttons
    bool upPressed;
    bool rightPressed;
    bool downPressed;
    bool leftPressed;
    [Space]

    public int buildingsUnlocked;
    [Space]

    //Stats
    public float weaponDamage;
    public float attackSpeed;
    public int maxHealth;
    public int currentHealth;
    public int maxMana;
    public int currentMana;

    public Inventory inventory;
    public UIController _UIController;

    //Weapon
    public GameObject weapon;

    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer weaponSpriteRenderer;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        _camera = Camera.main.GetComponent<MoveCamera>();
        anim = GetComponent<Animator>();
        playerMoveGO = transform.GetChild(0).transform;
        playerMoveGO.parent = null;
        currentHealth = maxHealth;
        currentMana = maxMana;
        pathfinding = GetComponent<Pathfinding>();
        playerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckHealth_Mana();
        Move();
        WeaponPosCheck();
    }

    public void WeaponPosCheck()
    {
        if (transform.GetChild(0).GetComponentInChildren<Weapon>())
        {
            transform.GetChild(0).GetComponentInChildren<Weapon>().transform.localPosition = Vector3.zero;
        }
    }
    public void ReceiveDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void EquipWeapon(GameObject orb)
    {
        if (transform.GetChild(0).GetComponentInChildren<Weapon>())
        {
            Destroy(transform.GetChild(0).GetComponentInChildren<Weapon>().gameObject);
        }
        GameObject startWeapon = Instantiate(orb, Vector3.zero, Quaternion.identity);
        startWeapon.transform.parent = transform.GetChild(0);
        weapon = startWeapon;
        Weapon tempWeapon = orb.GetComponent<Weapon>();
        attackSpeed = tempWeapon.attackSpeed;
        weaponDamage = tempWeapon.attackDamage;
        weaponAttackParticles = tempWeapon.attackParticles;
        weaponSpriteRenderer = weapon.GetComponent<SpriteRenderer>();
    }

    public void Death()
    {
        if (currentHealth <= 0)
        {
            isDead = true;
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
        CombatSimulate CS = Instantiate(combatSimulatorObj).GetComponent<CombatSimulate>();
        lastCS = CS;
        CS.setEnemy(lastEnemy.GetComponent<Enemy>());
        CS.setPlayer(this);
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

    public void LoadWeapon(int weaponInt)
    {
        EquipWeapon(allOrbs[weaponInt]);
    }
    IEnumerator LoadScene(string sceneToLoad)
    {
        _UIController.FadeScreen(0.5f);
        stopMove = true;
        lastScene = SceneManager.GetActiveScene().name;

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

        SetAllButtonFalse();
        stopMove = false;

        SceneSwitchGO temp = GameObject.FindGameObjectWithTag("SceneStartPos").GetComponent<SceneSwitchGO>();
        if (temp.sceneToLoad == lastScene)
            MoveToPoint(temp.transform.position + temp.offset);
        else
            MoveToPoint(Vector3.zero);
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
                            if (!MoveSceneObjCheck())
                                if (!MoveEnemyCheck())
                                    if (!MoveBuildingCheck())
                                        MoveTerrainCheck();
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

                            if (movePath.Count == 1)
                            {
                                if (!MoveSceneObjCheck())
                                    if (!MoveEnemyCheck())
                                        MoveBuildingCheck();
                            }

                            MoveTerrainCheck();
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

    public bool MoveEnemyCheck()
    {
        if (Physics2D.OverlapCircle(playerMoveGO.position + moveDir, 0.2f, enemyMask))
        {
            if(transform.GetChild(0).GetComponentInChildren<Weapon>())
            {
                RaycastHit2D hit;
                Vector3 direction = Vector3.Normalize(moveDir);
                if (hit = Physics2D.Raycast(playerMoveGO.position, direction))
                {
                    CombatStart(hit.collider.gameObject);
                }
                moveDir = Vector3.zero;
                return true;
            }
            else
            {
                _UIController.GetComponent<TutorialWriter>().NoWeaponText();
                moveDir = Vector3.zero;
                return true;
            }
        }
        return false;
    }

    public bool MoveSceneObjCheck()
    {
        if (Physics2D.OverlapCircle(playerMoveGO.position + moveDir, 0.2f, sceneSwitchMask))
        {
            RaycastHit2D hit;
            Vector3 direction = Vector3.Normalize(moveDir);
            if (hit = Physics2D.Raycast(playerMoveGO.position, direction))
            {
                SceneSwitchGO temp = hit.collider.GetComponent<SceneSwitchGO>();
                StartCoroutine(LoadScene(temp.sceneToLoad));
                return true;
            }
        }
        return false;
    }

    public bool MoveBuildingCheck()
    {
        if (Physics2D.OverlapCircle(playerMoveGO.position + moveDir, 0.2f, buildingMask))
        {
            RaycastHit2D hit;
            Vector3 direction = Vector3.Normalize(moveDir);
            if (hit = Physics2D.Raycast(playerMoveGO.position, direction))
            {
                if(pathFindingMove)
                    movePath.RemoveAt(movePath.Count - 1);
                hit.collider.gameObject.GetComponent<BuildingControl>().BuildingAction(_UIController);
            }
            moveDir = Vector3.zero;
            return true;
        }
        return false;
    }

    public bool MoveTerrainCheck()
    {
        if (!Physics2D.OverlapCircle(playerMoveGO.position + moveDir, 0.2f, stopMoveMask))
        {
            playerMoveGO.position += moveDir;
            return true;
        }
        return false;
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
                WeaponUnderPlayerSortingOrderCheck(true);
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
                WeaponUnderPlayerSortingOrderCheck(false);
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
                WeaponUnderPlayerSortingOrderCheck(false);
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
                WeaponUnderPlayerSortingOrderCheck(false);
            }
        }
    }

    public void ResetAllButtons()
    {
        upPressed = false;
        leftPressed = false;
        rightPressed = false;
        downPressed = false;
    }

    public void WeaponUnderPlayerSortingOrderCheck(bool isBehindPlayer)
    {
        if(transform.GetChild(0).GetComponentInChildren<Weapon>())
        {
            if (isBehindPlayer)
            {
                weaponSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder - 1;
                weapon.GetComponentInChildren<ParticleSystemRenderer>().sortingOrder = weaponSpriteRenderer.sortingOrder;
            }
            else if (weaponSpriteRenderer.sortingOrder != playerSpriteRenderer.sortingOrder + 1)
            {
                weaponSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder + 1;
                weapon.GetComponentInChildren<ParticleSystemRenderer>().sortingOrder = weaponSpriteRenderer.sortingOrder;
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

    #region
    public void FleeStart()
    {
        lastCS.Flee();
    }
    public void Fireball()
    {
        lastCS.SkillFireBall();
    }

    public void Firerain()
    {
        lastCS.SkillFireRain();
    }
    #endregion
}
