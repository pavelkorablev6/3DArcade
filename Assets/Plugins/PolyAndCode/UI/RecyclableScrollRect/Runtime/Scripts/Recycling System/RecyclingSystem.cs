//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com

using System.Collections;
using UnityEngine;

namespace PolyAndCode.UI
{
    /// <summary>
    /// Absract Class for creating a Recycling system.
    /// </summary>
    public abstract class RecyclingSystem
    {
        public IRecyclableScrollRectDataSource DataSource;

        protected RectTransform _viewport, _content;
        protected RectTransform _prototypeCell;
        protected bool _isGrid;

        protected float _minPoolCoverage = 1.5f; // The recyclable pool must cover (viewPort * _poolCoverage) area.
        protected int _minPoolSize = 10; // Cell pool must have a min size
        protected float _recyclingThreshold = .2f; //Threshold for recycling above and below viewport

        public abstract IEnumerator InitCoroutine(System.Action onInitialized = null);

        public abstract Vector2 OnValueChangedListener(Vector2 direction);
    }
}
