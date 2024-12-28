using Match3Test.Board;
using Match3Test.Game.Settings;
using Match3Test.Untility;
using UnityEngine;
using Zenject;

namespace Match3Test.Game
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private GameController gameController;
        [SerializeField] private BoardController boardController;
        [SerializeField] private GameSettings gameSettings;

        public override void InstallBindings()
        {
            SceneDiContainer.Container = Container;

            Container.BindInstance(gameController).AsSingle();
            Container.BindInstance(boardController).AsSingle();
            Container.BindInstance(gameSettings).AsSingle();
        }
    }
}