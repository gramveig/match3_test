using Match3Test.Board.BoardAnimationClasses;
using Zenject;

namespace Match3Test.Board
{
    public abstract class BoardState
    {
        protected BoardController _boardController;
        protected BoardAnimator _boardAnimator;

        [Inject]
        public void Construct(BoardController boardController, BoardAnimator boardAnimator)
        {
            _boardController = boardController;
            _boardAnimator = boardAnimator;
        }
        
        public abstract BoardStateId Id { get; }

        public virtual void Start()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual void Finish()
        {
            
        }
    }
}