using System;
using System.Collections.Generic;
using Match3Test.Board.Model;
using Match3Test.Game.Settings;
using UnityEngine;
using Zenject;

namespace Match3Test.Board.BoardAnimationClasses
{
    public class BoardAnimator : MonoBehaviour
    {
        private BoardAnimations _boardAnimations = new();

        public void AddGemToAnimation(Gem gem, GemAnimationType gemAnimationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(gemAnimationType);
            boardAnimation.AddGemToAnimation(gem);
        }

        public void StartNewAnimationSequence(GemAnimationType gemAnimationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(gemAnimationType);
            boardAnimation.StartNewAnimationSequence();
        }

        public void StartNewAnimationSequence(List<List<Gem>> gemsSequence,  GemAnimationType gemAnimationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(gemAnimationType);
            boardAnimation.StartNewAnimationSequence(gemsSequence);
        }

        public List<List<Gem>> GetAnimationSequence(GemAnimationType gemAnimationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(gemAnimationType);
            return boardAnimation.GetSequence();
        }

        public void AddGemToAnimationSequence(Gem gem, int idx, GemAnimationType gemAnimationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(gemAnimationType);
            boardAnimation.AddGemToAnimationSequence(gem, idx);
        }
        
        public void AnimateGemsInAnimation(Action callback, GemAnimationType gemAnimationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(gemAnimationType);
            if (boardAnimation.IsAnimations)
                boardAnimation.AnimateGems(callback);
            else
                callback?.Invoke();
        }

        public void AnimateGemsInSequence(Action callback, GemAnimationType gemAnimationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(gemAnimationType);
            if (boardAnimation.IsAnimations)
                boardAnimation.AnimateGemsInSequence(callback);
            else
                callback?.Invoke();
        }

        public void OnAnimateGemComplete(Gem gem, GemAnimationType gemAnimationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(gemAnimationType);
            boardAnimation.OnGemAnimationEnd(gem);
        }

        public bool IsGemsInAnimation(GemAnimationType gemAnimationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(gemAnimationType);
            return boardAnimation.IsAnimations;
        }
    }
}