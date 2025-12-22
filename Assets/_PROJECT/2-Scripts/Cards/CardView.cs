using System;
using TMPro;
using UnityEngine;
using System.IO;

namespace FXnRXn
{
	/// <summary>
	/// Visual representation of a card in the UI
	/// </summary>
	public class CardView : MonoBehaviour
	{
        # region Properties
        
        /// <summary>
        /// --- Public ---
        /// </summary>
        [TriInspector.Title("UI Elements")] 
        [field: SerializeField] private SpriteRenderer cardBackground;
        [field: SerializeField] private SpriteRenderer cardArt;
        [field: SerializeField] private TMP_Text cardLevel;
        [field: SerializeField] private TMP_Text descriptionTxt;
        [field: SerializeField] private TMP_Text costTxt;
        [field: SerializeField] private GameObject targetArrow;
        
        
        [TriInspector.Title("Colors")]
        [field: SerializeField] private Color attackColor = new Color(0.8f, 0.2f, 0.2f);
        [field: SerializeField] private Color skillColor = new Color(0.2f, 0.6f, 0.8f);
        [field: SerializeField] private Color powerColor = new Color(0.4f, 0.8f, 0.4f);


        /// <summary>
        /// --- Private ---
        /// </summary>
        private Vector3 originalPosition;
        private Vector3 originalScale;

        # endregion


        #region Unity Callback

        private void Start()
        {
	        
        }

        # endregion


        #region Public Properties

        /// <summary>
        /// Create a copy of this card data
        /// </summary>
        public CardView Clone()
        {
	        return Instantiate(this);
        }

        # endregion


        # region Helper Method

        # endregion
	}
}