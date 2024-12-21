using System.Collections;
using Match3Test.Board;
using Match3Test.Board.Model;
using Match3Test.Game;
using Match3Test.Utility;
using UnityEngine;
using Match3Test.Utility.Pooling;

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
        private Gem _gem;
        private bool _mousePressed;
        private Vector2 _firstTouchPosition;
        private Vector2 _startPosition;
        private Vector2 _endPosition;
        
        private bool _isMoving;
        private float _moveSpeed;
        private float _moveTime;
        private float _moveTimer;
        private float _sqDistThreshold;
        private readonly PrefabPool<GemView> _prefabPool = new();
        private ParticleSystem _particleSystem;


        const float DistanceThreshold = 0.01f;
        
        public void Init(Gem gem)
        {
            _gem = gem;
            _gameController = GameController.Instance;
            _boardController = BoardController.Instance;
            _moveSpeed = _gameController.GameSettings.GemSpeed;
            _sqDistThreshold = DistanceThreshold * DistanceThreshold;
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
                _boardController.OnMoveGemComplete();
            }
        }

        private IEnumerator DestroyWithAnimation()
        {
            spriteRenderer.enabled = false;
            burstAnim.SetActive(true);
            _particleSystem.Play();
            yield return new WaitForSeconds(burstAnimLength);
            _boardController.OnBurstGemComplete();
            burstAnim.SetActive(false);
            ReturnToPool();
        }

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