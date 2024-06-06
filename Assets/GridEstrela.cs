// Declaração das bibliotecas utilizadas
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class GridEstrela : MonoBehaviour
{
    public bool displayGridGizmos = true; // Flag para exibir a grade na visualização da cena
    public LayerMask unwakableMask; // Máscara para identificar áreas não caminháveis
    public Vector2 gridWorldSize; // Tamanho da grade em unidades do mundo
    public float nodeRadius; // Raio de cada nó
    Node[,] grid; // Matriz 2D para armazenar os nós da grade

    float nodeDiameter; // Diâmetro de cada nó
    int gridSizeX, gridSizeY; // Número de nós na grade ao longo dos eixos X e Y

    void Awake() {
        nodeDiameter = nodeRadius * 2; // Calcula o diâmetro de um nó
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); // Calcula o número de nós pelo eixo X
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter); // Calcula o número de nós pelo eixo Y
        CreateGrid(); // Inicializar a grade
    }

    void CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY]; // Cria a matriz da grade
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2; // Calcula o canto inferior esquerdo da grade no espaço do mundo

        // Preencher a grade com nós
        for(int x = 0; x < gridSizeX; x++) {
            for(int y = 0; y < gridSizeY; y++) {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius); // Calcula a posição no mundo do nó
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwakableMask); // Verifica se o nó é caminhável
                grid[x, y] = new Node(walkable, worldPoint, x, y); // Cria o nó e adiciona ele à grade
            }
        }
    }

    // Obtém os nós vizinhos de um dado nó
    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++) {
            for(int y = -1; y <= 1; y++) {
                if(x == 0 && y == 0)
                    continue;
                if(x == -1 && y == -1)
                    continue;
                if(x == -1 && y == 1)
                    continue;
                if(x == 1 && y == -1)
                    continue;
                if(x == 1 && y == 1)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // Garante que o vizinho esteja dentro dos limites da grade
                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    // Obtém o nó correspondente a uma posição no mundo
    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX); // Garante que a porcentagem esteja dentro de [0, 1]
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX); // Calcula a coordenada X na grade
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY); // Calcula a coordenada Y na grade
        return grid[x, y];
    }

    // Desenha a grade na visualização da cena
    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y)); // Desenhar o contorno da grade
        if(grid != null && displayGridGizmos) {
            foreach(Node n in grid) {
                Gizmos.color = n.walkable ? Color.white : Color.red; // Define a cor com base em se o nó é caminhável
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f)); // Desenha o nó
            }
        }  
    }
}
