using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.Splines;

namespace FXnRXn
{
	public class DeckManager : Singleton<DeckManager>
	{
		# region Properties

		/// <summary>
		/// --- Public ---
		/// </summary>
		[TriInspector.Title("Spline")] 
		[field: SerializeField] private SplineContainer splineContainer;
		
		[TriInspector.Title("UI Elements")] 
		[field: SerializeField] private List<CardView> MasterDeck = new List<CardView>();
		[TriInspector.HideInEditMode][field: SerializeField] private List<CardView> DrawPile = new List<CardView>();
		
		[TriInspector.Title("Card Settings")]
		[field: SerializeField] private float cardSpacing = 0.1f; // Adjustable spacing between cards
		[field: SerializeField] private float cardDepthOffset = 0.01f; // Z-offset for card layering
		
		# endregion


		# region Unity Callback

		private void Start()
		{
			Initialize().Forget();
		}

		# endregion


		# region Public Properties

		public async UniTaskVoid Initialize()
		{
			if (splineContainer == null) splineContainer = GetComponentInChildren<SplineContainer>();
			if(MasterDeck.Count == 0 || splineContainer == null) return;
			
			
			DrawPile.Clear();
			foreach (CardView card in MasterDeck)
			{
				CardView cardClone = await DrawCard(card, Vector3.zero, Quaternion.identity);
				await AddCard(cardClone);
			}
		}

		public async UniTask<CardView> DrawCard(CardView cardView, Vector3 position, Quaternion rotation)
		{
			CardView card = cardView.Clone();
			card.transform.localScale = Vector3.zero;
			await PrimeTween.Tween.Scale(card.transform, new Vector3(0.32f,0.32f,0.32f), 0.15f, Ease.InOutBounce);
			return card;
		}

		public async UniTask AddCard(CardView card)
		{
			DrawPile.Add(card);
			await UpdateCardPosition(0.15f);
		}
		
		

		# endregion


		# region Private Properties

		private async UniTask UpdateCardPosition(float duration)
		{
			if(DrawPile.Count == 0 || splineContainer == null) return;
			
			Spline spline = splineContainer.Spline;
			int cardCount = DrawPile.Count;
			float centerPosition = 0.5f;
			float totalSpacing = (cardCount - 1) * cardSpacing;
			float startPosition = centerPosition - (totalSpacing / 2f);
			
			List<UniTask> tweenTasks = new List<UniTask>();
			for (int i = 0; i < cardCount; i++)
			{
				float normalizedPosition = startPosition + (i * cardSpacing);
				normalizedPosition = Mathf.Clamp01(normalizedPosition);
				Vector3 splinePosition = spline.EvaluatePosition(normalizedPosition);
				Vector3 tangent = spline.EvaluateTangent(normalizedPosition);
				Vector3 upVector = spline.EvaluateUpVector(normalizedPosition);
				Quaternion rotation = Quaternion.LookRotation(-upVector, Vector3.Cross(-upVector, tangent).normalized);
				Vector3 finalPosition = transform.position + splinePosition + (i * cardDepthOffset * Vector3.back);
				tweenTasks.Add(PrimeTween.Tween.Position(DrawPile[i].transform, finalPosition, duration).ToUniTask());
				tweenTasks.Add(PrimeTween.Tween.Rotation(DrawPile[i].transform, rotation.eulerAngles, duration).ToUniTask());
			}
			await UniTask.WhenAll(tweenTasks);
		}

		# endregion


		# region Helper Method

		# endregion
	}
}