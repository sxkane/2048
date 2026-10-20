using _2048.Scripts.Core;
using UnityEngine;

namespace _2048.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private GameCore _gameCore;

        private void Awake()
        {
            _gameCore = new GameCore();
        }

        private void Start()
        {
            _gameCore.ReStart();
            
            Debug.Log("Game started");
        }
}
}