using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    GridEstrela grid;
    // public Transform seeker, target;

    // public List<Node> path_follow;

    PathRequestManager requestManager;


    void Awake() {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<GridEstrela>();
    }

    public Boolean ended = false;

    // void Update() {
    //     FindPath(seeker.position, target.position);
    // }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos){
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        if(startNode.walkable && targetNode.walkable){

            List<Node> openSet = new List<Node>();
            HashSet<Node> closeSet = new HashSet<Node>();

            openSet.Add(startNode);


            while(openSet.Count > 0){
                Node currentNode = openSet[0];
                for(int i = 1; i < openSet.Count; i++){
                    if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost){
                        currentNode = openSet[i];
                    }
                }


                openSet.Remove(currentNode);
                closeSet.Add(currentNode);

                if(currentNode == targetNode){
                    pathSuccess = true;
                    // RetracePath(startNode, targetNode);
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode)){
                    if(!neighbour.walkable || closeSet.Contains(neighbour)){
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)){
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if(!openSet.Contains(neighbour)){
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess){
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode){
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode){
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        // path_follow = path;
        // grid.path = path;
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path){
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for(int i = 1; i < path.Count; i++){
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if(directionNew != directionOld){
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB){
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(dstX > dstY){
            return 14 * dstY + 10 * (dstX - dstY);
        }else{
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}
