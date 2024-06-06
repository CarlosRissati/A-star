// Declaração das bilbiotecas utilizadas
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target; // Alvo que a unidade irá seguir
    float speed = 15f; // Velocidade de movimento da unidade
    Vector3[] path; // Caminho para o alvo
    int targetIndex; // Índice do waypoint atual no caminho

    void Start(){
        // Quando o objeto é iniciado, solicita um caminho ao gerenciador de solicitações de caminho
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

   // Função de retorno chamada após encontrar um novo caminho
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful){
        if(pathSuccessful){
            // Se o caminho for encontrado com sucesso, armazena o novo caminho e começa a seguir
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    // Método para seguir o caminho
    IEnumerator FollowPath(){
        Vector3 currentWaypoint = path[0]; // O primeiro waypoint no caminho

        while(true){
            // Loop infinito para seguir o caminho
            if(transform.position == currentWaypoint){
                // Se a unidade atingir o waypoint atual, passa para o próximo
                targetIndex++;
                if(targetIndex >= path.Length){
                    // Se chegou ao final do caminho, termina a execução do método
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            // Move a unidade em direção ao próximo waypoint
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    // Método para desenhar os gizmos no editor
    public void OnDrawGizmos(){
        if(path != null){
            for(int i = targetIndex; i < path.Length; i++){
                // Desenha um cubo em cada waypoint do caminho
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if(i == targetIndex){
                    // Desenha uma linha do objeto atual até o waypoint atual
                    Gizmos.DrawLine(transform.position, path[i]);
                }else{
                    // Desenha uma linha entre os waypoints adjacentes
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
    }

}
