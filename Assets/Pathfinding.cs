// Declaração das bibliotecas utilizadas
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    GridEstrela grid; // Referência para o componente GridEstrela
    // public Transform seeker, target; // Transformações do buscador e do alvo (comentadas)

    // public List<Node> path_follow; // Lista para seguir o caminho (comentada)

    PathRequestManager requestManager; // Referência para o componente PathRequestManager

    void Awake() {
        requestManager = GetComponent<PathRequestManager>(); // Inicializa o PathRequestManager
        grid = GetComponent<GridEstrela>(); // Inicializa o GridEstrela
    }

    public Boolean ended = false; // Indica se o processo de busca de caminho terminou

    // void Update() {
    //     FindPath(seeker.position, target.position); // Chama a função FindPath a cada frame (comentada)
    // }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
        StartCoroutine(FindPath(startPos, targetPos)); // Inicia a busca de caminho
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
        Node startNode = grid.NodeFromWorldPoint(startPos); // Obtém o nó inicial a partir da posição no mundo
        Node targetNode = grid.NodeFromWorldPoint(targetPos); // Obtém o nó alvo a partir da posição no mundo

        Vector3[] waypoints = new Vector3[0]; // Inicializa os waypoints
        bool pathSuccess = false; // Indica se o caminho foi encontrado com sucesso

        // Verifica se os nós inicial e alvo são caminháveis
        if (startNode.walkable && targetNode.walkable) { 

            List<Node> openSet = new List<Node>(); // Conjunto de nós a serem avaliados
            HashSet<Node> closeSet = new HashSet<Node>(); // Conjunto de nós já avaliados

            openSet.Add(startNode); // Adiciona o nó inicial ao conjunto aberto

            while (openSet.Count > 0) { // Loop até que não haja mais nós no conjunto aberto
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++) {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
                        currentNode = openSet[i]; // Seleciona o nó com o menor fCost
                    }
                }

                openSet.Remove(currentNode); // Remove o nó atual do conjunto aberto
                closeSet.Add(currentNode); // Adiciona o nó atual ao conjunto fechado

                if (currentNode == targetNode) { // Verifica se o nó atual é o nó alvo
                    pathSuccess = true;
                    // RetracePath(startNode, targetNode); // Reconstitui o caminho
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode)) { // Avalia os vizinhos do nó atual
                    if (!neighbour.walkable || closeSet.Contains(neighbour)) {
                        continue; // Ignora os nós não caminháveis ou já avaliados
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        neighbour.gCost = newMovementCostToNeighbour; // Atualiza o custo g do vizinho
                        neighbour.hCost = GetDistance(neighbour, targetNode); // Atualiza o custo h do vizinho
                        neighbour.parent = currentNode; // Define o nó pai do vizinho

                        if (!openSet.Contains(neighbour)) {
                            openSet.Add(neighbour); // Adiciona o vizinho ao conjunto aberto
                        }
                    }
                }
            }
        }
        yield return null; // Pausa a corrotina até o próximo frame

        if (pathSuccess) {
            waypoints = RetracePath(startNode, targetNode); // Reconstitui o caminho se ele for encontrado
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess); // Notifica o PathRequestManager sobre o fim do processamento
    }

    Vector3[] RetracePath(Node startNode, Node endNode) { 
        List<Node> path = new List<Node>(); // Cria a lista de nós em um array de vetores
        Node currentNode = endNode;

        while (currentNode != startNode) { // Reconstitui o caminho de trás para frente
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path); // Simplifica o caminho
        Array.Reverse(waypoints); // Inverte a ordem dos waypoints
        // path_follow = path; // Define o caminho a seguir
        // grid.path = path; // Define o caminho na grade 
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path) { // Cria a lista de nós em um array de vetores
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++) {
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if (directionNew != directionOld) {
                waypoints.Add(path[i].worldPosition); // Adiciona pontos de virada ao caminho simplificado
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB) { //  Define a estrutura básica do método para calular a distância
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY) {
            return 14 * dstY + 10 * (dstX - dstY); // Cálculo da distância utilizando a heurística de distância diagonal
        } else {
            return 14 * dstX + 10 * (dstY - dstX); // Cálculo da distância utilizando a heurística de distância diagonal
        }
    }
}
