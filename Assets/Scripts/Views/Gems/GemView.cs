using System.Collections;
using DG.Tweening;
using Match3Test.Board;
using Match3Test.Board.BoardAnimationClasses;
using Match3Test.Board.Model;
using Match3Test.Game;
using Match3Test.Game.Settings;
using Match3Test.Utility;
using UnityEngine;
using Match3Test.Utility.Pooling;
using Zenject;

namespace Match3Test.Views.Gems
{
    public class GemView : MonoBehaviour, IPooledPrefab<GemView>
    {
        [SerializeField] private GemClass gemClass;
        [SerializeField] private GemColor gemColor;
        [SerializeField] private GemSpecialType gemSpecialType;
        [SerializeField] private GameObject burstAnim;
        [SerializeField] private float burstAnimLength;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private int scoreValue;

        public GemClass GemClass => gemClass;
        public GemColor GemColor => gemColor;
        public GemSpecialType GemSpecialType => gemSpecialType;
        public int ScoreValue => scoreValue;

        private GameController _gameController;
        private BoardController _boardController;
        private BoardAnimator _boardAnimator;
        private GameSettings _gameSettings;
        private Gem _gem;
        private bool _mousePressed;
        private Vector2 _firstTouchPosition;
        private Vector2 _startPosition;
        private Vector2 _endPosition;
        
        private bool _isMoving;
        private float _moveSpeed;
        private float _moveTime;
        private float _moveTimer;
        private readonly PrefabPool<GemView> _prefabPool = new();
        private ParticleSystem _particleSystem;

        [Inject]
        public void Construct(GameController gameController, BoardController boardController, GameSettings gameSettings,
            BoardAnimator boardAnimator)
        {
            _gameController = gameController;
            _boardController = boardController;
            _boardAnimator = boardAnimator; 
            _gameSettings = gameSettings;
        }

        //event functions

        private void Update()
        {
            if (_mousePressed)
                DetectMouseButtonUp();

            if (_isMoving)
                MoveGradually();
        }

        private void OnMouseDown()
        {
            if (_gameController.GameState == GameState.WaitForMove)
            {
                _firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _mousePressed = true;
            }
        }

        //public

        public void Init(Gem gem)
        {
            _gem = gem;
            _moveSpeed = _gameSettings.GemSpeed;
            _mousePressed = false;
            _isMoving = false;
            spriteRenderer.enabled = true;
            _particleSystem = burstAnim.GetComponent<ParticleSystem>();
            burstAnim.SetActive(false);
        }

        public void Move(Vector2Int endPosition)
        {
            _startPosition = transform.position;
            _endPosition = endPosition;
            float distance = (_endPosition - _startPosition).magnitude;
            _moveTime = distance / _moveSpeed;
            _moveTimer = 0;
            _isMoving = true;
        }

        public void Destroy()
        {
            StartCoroutine(DestroyWithAnimation());
        }

        public void Shake()
        {
            float shakeTime = _gameSettings.ShakeTime;
            float jumpPower = _gameSettings.JumpPower;

            _startPosition = transform.position;
            transform.DOJump(_startPosition, jumpPower, 1, shakeTime)
                .onComplete = () => { _boardAnimator.OnAnimateGemComplete(_gem, AnimationType.ShakeGems); };
        }

        //private

        private void DetectMouseButtonUp()
        {
            if (_mousePressed && Input.GetMouseButtonUp(0))
            {
                _mousePressed = false;
                if (_gameController.GameState == GameState.WaitForMove) ProcessSwipe();
            }
        }

        private void ProcessSwipe()
        {
            Vector2 finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float swipeAngle = CalculateAngle(_firstTouchPosition, finalTouchPosition);
            Direction swipeDirection = AngleHelper.AngleToDirection(swipeAngle);

            _boardController.ProcessSwipe(_gem, swipeDirection);
        }

        private float CalculateAngle(Vector2 firstTouchPosition, Vector2 finalTouchPosition)
        {
            float swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
            swipeAngle = swipeAngle * 180 / Mathf.PI;

            return swipeAngle;
        }

        private void MoveGradually()
        {
            if (!_isMoving) return;

            _moveTimer += Time.deltaTime;
            float r = _moveTimer / _moveTime;
            if (r < 1f)
                transform.position = Vector2.Lerp(_startPosition, _endPosition, AnimHelper.EaseInExpo(Mathf.Clamp01(r)));
            else
            {
                transform.position = _endPosition;
                _isMoving = false;
                _boardAnimator.OnAnimateGemComplete(_gem, AnimationType.MoveGems);
            }
        }

        private IEnumerator DestroyWithAnimation()
        {
            spriteRenderer.enabled = false;
            burstAnim.SetActive(true);
            _particleSystem.Play();
            yield return new WaitForSeconds(burstAnimLength);
            burstAnim.SetActive(false);
            _boardAnimator.OnAnimateGemComplete(_gem, AnimationType.DestroyGems);
            ReturnToPool();
        }

        //pooling

        public void InitPool(int prefetchCount = 0, Transform container = null)
        {
            _prefabPool.InitPool(this, prefetchCount, container);
        }

        public GemView GetInstance()
        {
            return _prefabPool.GetInstance(this);
        }

        public void ReturnToPool()
        {
            _prefabPool.ReturnToPool(this);
        }

        public void SetPoolReference(ObjectPool<GemView> pool, Transform poolContainer)
        {
            _prefabPool.SetPoolReference(pool, poolContainer);
        }
    }
}