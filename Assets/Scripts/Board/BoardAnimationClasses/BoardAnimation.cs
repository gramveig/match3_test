using System;
using System.Collections;
using System.Collections.Generic;
using Match3Test.Board.Model;
using Match3Test.Game.Settings;
using UnityEngine;
using Zenject;

namespace Match3Test.Board.BoardAnimationClasses
{
    public abstract class BoardAnimation
    {
        protected BoardAnimator _boardAnimator;
        protected GameSettings _gameSettings;
        protected List<Gem> _gemsToAnimate = new();
        protected List<List<Gem>> _gemsSequence = new();
        protected int _seqCount;
        protected Action _callback;
        protected bool _isSequence;

        [Inject]
        public void Construct(BoardAnimator boardAnimator, GameSettings gameSettings)
        {
            _boardAnimator = boardAnimator;
            _gameSettings = gameSettings;
        }

        public void AddGemToAnimation(Gem gem)
        {
            if (gem == null) return;
            if (gem.GemView == null)
            {
                Debug.LogError("Gem View is null");
                return;
            }

            _isSequence = false;
            _gemsToAnimate.Add(gem);
        }

        public void StartNewAnimationSequence()
        {
            _gemsSequence.Clear();
            _isSequence = true;
            _seqCount = 0;
        }

        public void StartNewAnimationSequence(List<List<Gem>> gemsSequence)
        {
            _gemsSequence = gemsSequence;
            _isSequence = true;
            _seqCount = GetSeqCount();
        }
        
        public void AddGemToAnimationSequence(Gem gem, int idx)
        {
            if (gem == null) return;
            if (gem.GemView == null)
            {
                Debug.LogError("Gem View is null");
                return;
            }

            if (idx >= _gemsSequence.Count) AddIndex(idx);

            _gemsSequence[idx].Add(gem);
            _isSequence = true;
        }

        public void AnimateGems(Action callback)
        {
            _callback = callback;

            foreach (Gem gem in _gemsToAnimate)
                Animate(gem);
        }

        public void AnimateGemsInSequence(Action callback)
        {
            _callback = callback;
            _seqCount = GetSeqCount();
            _boardAnimator.StartCoroutine(SeqEnumerator());
        }

        public void OnGemAnimationEnd(Gem gem)
        {
            if (!_isSequence)
            {
                _gemsToAnimate.Remove(gem);
                if (_gemsToAnimate.Count <= 0) OnAnimationEnd();
            }
            else
            {
                _seqCount--;
                if (_seqCount <= 0) OnAnimationEnd();
            }
        }

        public bool IsAnimations => _isSequence ? GetSeqCount() > 0 : _gemsToAnimate.Count > 0;

        public List<List<Gem>> GetSequence()
        {
            return _gemsSequence;
        }

        protected abstract void Animate(Gem gem);

        //private

        private void OnAnimationEnd()
        {
            _gemsToAnimate.Clear();
            _callback?.Invoke();
            _seqCount = 0;
        }

        private void AddIndex(int idx)
        {
            for (int i = 0; i <= idx; i++)
                if (_gemsSequence.Count - 1 < i)
                    _gemsSequence.Add(new List<Gem>());
        }

        private IEnumerator SeqEnumerator()
        {
            float delayStep = _gameSettings.DelayStep;
            int maxJ = GetMaxNestedListCount();
            for (int j = 0; j < maxJ; j++)
            {
                yield return new WaitForSeconds(delayStep * j);

                foreach (var list in _gemsSequence)
                {
                    if (j >= list.Count) continue;

                    Gem gem = list[j];
                    Animate(gem);
                }
            }
        }
        
        private int GetMaxNestedListCount()
        {
            int maxCount = 0;
            for (int i = 0; i < _gemsSequence.Count; i++)
            {
                int count = _gemsSequence[i].Count;
                if (count > maxCount) maxCount = count;
            }

            return maxCount;
        }

        private int GetSeqCount()
        {
            int totalCount = 0;
            for (int i = 0; i < _gemsSequence.Count; i++)
                totalCount += _gemsSequence[i].Count;

            return totalCount;
        }
    }
}