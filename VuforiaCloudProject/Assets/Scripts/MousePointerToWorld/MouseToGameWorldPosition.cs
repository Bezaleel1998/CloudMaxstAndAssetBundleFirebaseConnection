using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.AI;

public class MouseToGameWorldPosition : MonoBehaviour
{

    [Header("Camera")]
    [SerializeField] Camera mainCamera;
    [Header("GameObject")]
    public GameObject parentGameObject3D;
    public GameObject parentGameObject2D;
    [SerializeField] GameObject object2DorPrefabs;
    [SerializeField] GameObject object3DorPrefabs;
    [Header("LayerMask")]
    [SerializeField] LayerMask layerMask;//get the layer of the gameobject
    [Header("AINavmesh")]
    [SerializeField] private NavMeshAgent agent;
    [Tooltip("Optional")]
    [SerializeField] private GameObject playerTarget;


    private void Update()
    {

        //Mouse in 2D world
        //MousePositionOn2D(mainCamera, object2DorPrefabs);
        //MousePositionOn3D(mainCamera, object3DorPrefabs, layerMask, parentGameObject3D);

    }

    private void FixedUpdate()
    {
        //AI following player 
        AIFollowMouseClick(agent, playerTarget.transform.position);
    
    }


    #region MousePositionOn2Dand3D

    private void MousePositionOn2D(Camera mainCam, GameObject obj2D)
    {

        Debug.Log(mainCam.ScreenToWorldPoint(Input.mousePosition));
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        obj2D.transform.position = mouseWorldPos;

    }

    private void MousePositionOn3D(Camera mainCam, GameObject obj3D, LayerMask lM, GameObject parent)
    {
        Vector3 mousePos = Mouse.current.position.ReadValue()/**Input.mousePosition**/;
        Ray ray = mainCam.ScreenPointToRay(mousePos);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, float.MaxValue, lM))
        {

            Debug.Log(raycastHit.point);
            if (Mouse.current.leftButton.wasPressedThisFrame /**Input.GetKey(KeyCode.Mouse0)**/)
            {

                //if you want to instantiate the prefabs
                //SpawnAtLocation(raycastHit.point, obj3D, parent);

                //if you want something following your mouse
                AIFollowMouseClick(agent, raycastHit.point);

            }

        }

    }

    #endregion

    #region Spawner

    //Spawn at mouse on click position
    public void SpawnAtLocation(Vector3 spawnPosition, GameObject minionModel, GameObject parent)
    {

        GameObject deployedMinion = Instantiate(minionModel, spawnPosition, Quaternion.identity);
        deployedMinion.transform.parent = parent.transform;

    }

    #endregion

    #region AINavMeshSystem

    //AI Following MouseClick Position
    void AIFollowMouseClick(NavMeshAgent agent, Vector3 targetPosition)
    {

        agent.destination = targetPosition;

    }

    #endregion

}
