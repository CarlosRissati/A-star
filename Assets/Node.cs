// Declaração das bibliotecas utilizadas
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable; // Verifica se o nó é caminhável
    public Vector3 worldPosition; // Posição do nó no espaço do mundo
    public int gridX; // Coordenada X do nó na grade
    public int gridY; // Coordenada Y do nó na grade

    public int gCost; // Custo do nó desde o nó inicial
    public int hCost; // Custo estimado do nó até o nó final (heurística)
    public Node parent; // Nó pai para reconstruir o caminho

    // Construtor para inicializar um nó
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
        walkable = _walkable; // Define se o nó é caminhável
        worldPosition = _worldPos; // Define a posição do nó no mundo
        gridX = _gridX; // Define a coordenada X do nó na grade
        gridY = _gridY; // Define a coordenada Y do nó na grade
    }

    // Propriedade para obter o custo total (fCost = gCost + hCost)
    public int fCost {
        get {
            return gCost + hCost; // Retorna a soma de gCost e hCost
        }
    }
}
