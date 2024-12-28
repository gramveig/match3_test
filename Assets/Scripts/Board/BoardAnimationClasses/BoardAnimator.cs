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

        public void AddGemToAnimation(Gem gem, AnimationType animationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(animationType);
            boardAnimation.AddGemToAnimation(gem);
        }

        public void StartNewAnimationSequence(AnimationType animationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(animationType);
            boardAnimation.StartNewAnimationSequence();
        }

        public void StartNewAnimationSequence(List<List<Gem>> gemsSequence,  AnimationType animationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(animationType);
            boardAnimation.StartNewAnimationSequence(gemsSequence);
        }

        public List<List<Gem>> GetAnimationSequence(AnimationType animationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(animationType);
            return boardAnimation.GetSequence();
        }

        public void AddGemToAnimationSequence(Gem gem, int idx, AnimationType animationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(animationType);
            boardAnimation.AddGemToAnimationSequence(gem, idx);
        }
        
        public void AnimateGemsInAnimation(Action callback, AnimationType animationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(animationType);
            if (boardAnimation.IsAnimations)
                boardAnimation.AnimateGems(callback);
            else
                callback?.Invoke();
        }

        public void AnimateGemsInSequence(Action callback, AnimationType animationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(animationType);
            if (boardAnimation.IsAnimations)
                boardAnimation.AnimateGemsInSequence(callback);
            else
                callback?.Invoke();
        }

        public void OnAnimateGemComplete(Gem gem, AnimationType animationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(animationType);
            boardAnimation.OnGemAnimationEnd(gem);
        }

        public bool IsGemsInAnimation(AnimationType animationType)
        {
            BoardAnimation boardAnimation = _boardAnimations.GetAnimation(animationType);
            return boardAnimation.IsAnimations;
        }
    }
}