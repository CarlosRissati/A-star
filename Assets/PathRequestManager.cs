// Declaração das bibliotecas utilizadas
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    // Fila para armazenar solicitações de caminho
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    // Solicitação de caminho atual sendo processada
    PathRequest currentPathRequest; 

    static PathRequestManager instance; // Instância estática do gerenciador de solicitações de caminho
    Pathfinding pathfinding; // Referência ao componente de pathfinding que será usado para encontrar caminhos no jogo.

    bool isProcessingPath; // Verifica se uma solicitação de caminho está sendo processada atualmente

    void Awake() {
        instance = this; // Define a instância como esta instância do script
        pathfinding = GetComponent<Pathfinding>(); // Obtém a referência ao componente de pathfinding no mesmo GameObject
    }

    // Método estático para solicitar um novo caminho
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback){
        // Cria uma nova solicitação de caminho
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        // Adiciona a solicitação à fila de solicitações
        instance.pathRequestQueue.Enqueue(newRequest);
        // Tenta processar a próxima solicitação de caminho
        instance.TryProcessNext();
    }

    // Método que tenta processar a próxima solicitação de caminho
    void TryProcessNext(){
        // Se não estiver processando uma solicitação de caminho e houver solicitações na fila
        if(!isProcessingPath && pathRequestQueue.Count > 0){
            // Define a próxima solicitação de caminho como a solicitação atual
            currentPathRequest = pathRequestQueue.Dequeue();
            // Indica que uma solicitação de caminho está sendo processada
            isProcessingPath = true;
            // Inicia o processo de busca de caminho
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    // Método chamado quando o processo de busca de caminho é concluído
    public void FinishedProcessingPath(Vector3[] path, bool success){
        // Chama o callback associado à solicitação de caminho atual
        currentPathRequest.callback(path, success);
        // Indica que o processamento da solicitação de caminho foi concluído
        isProcessingPath = false;
        // Tenta processar a próxima solicitação de caminho na fila
        TryProcessNext();
    }

    // Estrutura que representa uma solicitação de caminho
    struct PathRequest{
        public Vector3 pathStart; // Ponto de início do caminho
        public Vector3 pathEnd; // Ponto de destino do caminho
        public Action<Vector3[], bool> callback; // Uma referência à função que será chamada quando o caminho 
                                                // é encontrado, passando o caminho encontrado e um indicador de sucesso como parâmetros.

        // Construtor da estrutura
        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback){
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
