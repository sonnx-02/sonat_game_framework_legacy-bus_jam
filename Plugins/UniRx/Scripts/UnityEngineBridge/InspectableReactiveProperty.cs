using System;
using System.Collections.Generic;
using UniRx.InternalUtil;
using UnityEngine;
using UnityEngine.UI;

namespace UniRx
{
    /// <summary>
    /// Inspectable ReactiveProperty.
    /// </summary>
    [Serializable]
    public class IntReactiveProperty : ReactiveProperty<int>
    {
        public IntReactiveProperty()
            : base()
        {
        }

        public IntReactiveProperty(int initialValue)
            : base(initialValue)
        {
        }

        public IntReactiveProperty(int initialValue, string playerPrefKey)
            : base(initialValue)
        {
            Value = PlayerPrefs.GetInt(playerPrefKey, initialValue);
            this.Subscribe(data => PlayerPrefs.SetInt(playerPrefKey, data));
        }

        public IntReactiveProperty(IntReactiveProperty compute1, IntReactiveProperty compute2)
        {
            if (compute1 == null || compute2 == null)
                Debug.LogError("not valid input");
            else
            {
                compute1.Subscribe(data => Compute());
                compute2.Subscribe(data => Compute());
                Compute();
                void Compute()
                {
                    BoolValue = compute1.BoolValue && compute2.BoolValue;
                }
            }
        }
        

        public IntReactiveProperty(IntReactiveProperty[] computes)
        {
            if (computes == null || computes.Length == 0)
                Debug.LogError("not valid input");
            else
            {
                foreach (var compute in computes)
                    compute.Subscribe(data => Compute());
                Compute();
                void Compute()
                {
                    var valid = true;
                    foreach (var compute in computes)
                        if (!compute.BoolValue)
                        {
                            valid = false;
                            break;
                        }

                    BoolValue = valid;
                }
            }
        }

        public IntReactiveProperty( BoolReactiveProperty[] computes)
        {
            if (computes == null || computes.Length == 0)
            {
                Debug.LogError("not valid input");
            }
            else
            {
                foreach (var compute in computes)
                    compute.Subscribe(data => Compute());
                Compute();
                void Compute()
                {
                    var valid = true;
                    foreach (var compute in computes)
                        if (compute.Value)
                        {
                            valid = false;
                            break;
                        }

                    BoolValue = valid;
                }
            }
        }

        public IntReactiveProperty(IntReactiveProperty[] intComputes, BoolReactiveProperty[] computes)
        {
            if (computes == null || computes.Length == 0 || intComputes == null || intComputes.Length == 0)
                Debug.LogError("not valid input");
            else
            {
                foreach (var compute in computes)
                    compute.Subscribe(data => Compute());
                foreach (var compute in intComputes)
                    compute.Subscribe(data => Compute());
                Compute();
                void Compute()
                {
                    var valid = true;
                    foreach (var compute in computes)
                        if (compute.Value)
                        {
                            valid = false;
                            break;
                        }

                    if (valid)
                        foreach (var compute in intComputes)
                            if (compute.BoolValue)
                            {
                                valid = false;
                                break;
                            }

                    BoolValue = valid;
                }
            }
        }

        public IntReactiveProperty(IntReactiveProperty compute1, BoolReactiveProperty compute2)
        {
            if (compute1 == null || compute2 == null)
                Debug.LogError("not valid input");
            else
            {
                compute1.Subscribe(data => Compute());
                compute2.Subscribe(data => Compute());
                Compute();
                void Compute()
                {
                    BoolValue = compute1.BoolValue && compute2.Value;
                }
            }
        }

        public IntReactiveProperty(BoolReactiveProperty compute1, BoolReactiveProperty compute2)
        {
            if (compute1 == null || compute2 == null)
                Debug.LogError("not valid input");
            else
            {
                compute1.Subscribe(data => Compute());
                compute2.Subscribe(data => Compute());
                Compute();
                void Compute()
                {
                    BoolValue = compute1.Value && compute2.Value;
                }
            }
        }


        public bool BoolValue
        {
            get => Value >= 1;
            set => Value = value ? 1 : 0;
        }

        public void SetMax(int max)
        {
            if (Value < max)
                Value = max;
        }
    }

    /// <summary>
    /// Inspectable ReactiveProperty.
    /// </summary>
    [Serializable]
    public class LongReactiveProperty : ReactiveProperty<long>
    {
        public LongReactiveProperty()
            : base()
        {
        }

        public LongReactiveProperty(long initialValue)
            : base(initialValue)
        {
        }
    }


    /// <summary>
    /// Inspectable ReactiveProperty.
    /// </summary>
    [Serializable]
    public class ByteReactiveProperty : ReactiveProperty<byte>
    {
        public ByteReactiveProperty()
            : base()
        {
        }

        public ByteReactiveProperty(byte initialValue)
            : base(initialValue)
        {
        }
    }

    /// <summary>
    /// Inspectable ReactiveProperty.
    /// </summary>
    [Serializable]
    public class FloatReactiveProperty : ReactiveProperty<float>
    {
        public FloatReactiveProperty()
            : base()
        {
        }

        public FloatReactiveProperty(float initialValue)
            : base(initialValue)
        {
        }

        public void BindTo(Slider thisUiSlider)
        {
            thisUiSlider.value = Value;
            thisUiSlider.onValueChanged.AddListener(data => Value = data);
        }
    }

    /// <summary>
    /// Inspectable ReactiveProperty.
    /// </summary>
    [Serializable]
    public class DoubleReactiveProperty : ReactiveProperty<double>
    {
        public DoubleReactiveProperty()
            : base()
        {
        }

        public DoubleReactiveProperty(double initialValue)
            : base(initialValue)
        {
        }
    }

    /// <summary>
    /// Inspectable ReactiveProperty.
    /// </summary>
    [Serializable]
    public class StringReactiveProperty : ReactiveProperty<string>
    {
        public StringReactiveProperty()
            : base()
        {
        }

        public StringReactiveProperty(string initialValue)
            : base(initialValue)
        {
        }
    }

    /// <summary>
    /// Inspectable ReactiveProperty.
    /// </summary>
    [Serializable]
    public class BoolReactiveProperty : ReactiveProperty<bool>
    {
        public BoolReactiveProperty()
            : base()
        {
        }

        public BoolReactiveProperty(bool initialValue)
            : base(initialValue)
        {
        }

        public BoolReactiveProperty(IntReactiveProperty compute1, BoolReactiveProperty compute2)
        {
            if (compute1 == null || compute2 == null)
                Debug.LogError("not valid input");
            else
            {
                compute1.Subscribe(data => Compute());
                compute2.Subscribe(data => Compute());
                Compute();

                void Compute()
                {
                    Value = compute1.BoolValue && compute2.Value;
                }
            }
        }

        public BoolReactiveProperty(BoolReactiveProperty compute1, BoolReactiveProperty compute2)
        {
            if (compute1 == null || compute2 == null)
                Debug.LogError("not valid input");
            else
            {
                compute1.Subscribe(data => Compute());
                compute2.Subscribe(data => Compute());
                Compute();

                void Compute()
                {
                    Value = compute1.Value && compute2.Value;
                }
            }
        }

        public BoolReactiveProperty(IntReactiveProperty compute)
        {
            compute.Subscribe(data => Value = data != 0);
        }

        public BoolReactiveProperty(IntReactiveProperty[] computes)
        {
            if (computes == null || computes.Length == 0)
                Debug.LogError("not valid input");
            else
            {
                foreach (var compute in computes)
                    compute.Subscribe(data => Compute());
                Compute();

                void Compute()
                {
                    var valid = true;
                    foreach (var compute in computes)
                        if (compute.BoolValue)
                        {
                            valid = false;
                            break;
                        }

                    Value = valid;
                }
            }
        }

        public BoolReactiveProperty(BoolReactiveProperty[] computes)
        {
            if (computes == null || computes.Length == 0)
                Debug.LogError("not valid input");
            else
            {
                foreach (var compute in computes)
                    compute.Subscribe(data => Compute());
                Compute();

                void Compute()
                {
                    var valid = true;
                    foreach (var compute in computes)
                        if (compute.Value)
                        {
                            valid = false;
                            break;
                        }

                    Value = valid;
                }
            }
        }

        public BoolReactiveProperty(IntReactiveProperty[] intComputes, BoolReactiveProperty[] computes)
        {
            if (computes == null || computes.Length == 0 || intComputes == null || intComputes.Length == 0)
                Debug.LogError("not valid input");
            else
            {
                foreach (var compute in computes)
                    compute.Subscribe(data => Compute());
                foreach (var compute in intComputes)
                    compute.Subscribe(data => Compute());
                Compute();

                void Compute()
                {
                    var valid = true;
                    foreach (var compute in computes)
                        if (compute.Value)
                        {
                            valid = false;
                            break;
                        }

                    if (valid)
                        foreach (var compute in intComputes)
                            if (compute.BoolValue)
                            {
                                valid = false;
                                break;
                            }

                    Value = valid;
                }
            }
        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class Vector2ReactiveProperty : ReactiveProperty<Vector2>
    {
        public Vector2ReactiveProperty()
        {
        }

        public Vector2ReactiveProperty(Vector2 initialValue)
            : base(initialValue)
        {
        }

        protected override IEqualityComparer<Vector2> EqualityComparer
        {
            get { return UnityEqualityComparer.Vector2; }
        }
    }
    
    [Serializable]
    public class Vector2IntReactiveProperty : ReactiveProperty<Vector2Int>
    {
        public Vector2IntReactiveProperty()
        {
        }

        public Vector2IntReactiveProperty(Vector2Int initialValue)
            : base(initialValue)
        {
        }

        protected override IEqualityComparer<Vector2Int> EqualityComparer
        {
            get { return UnityEqualityComparer.Vector2Int; }
        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class Vector3ReactiveProperty : ReactiveProperty<Vector3>
    {
        public Vector3ReactiveProperty()
        {
        }

        public Vector3ReactiveProperty(Vector3 initialValue)
            : base(initialValue)
        {
        }

        protected override IEqualityComparer<Vector3> EqualityComparer
        {
            get { return UnityEqualityComparer.Vector3; }
        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class Vector4ReactiveProperty : ReactiveProperty<Vector4>
    {
        public Vector4ReactiveProperty()
        {
        }

        public Vector4ReactiveProperty(Vector4 initialValue)
            : base(initialValue)
        {
        }

        protected override IEqualityComparer<Vector4> EqualityComparer
        {
            get { return UnityEqualityComparer.Vector4; }
        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class ColorReactiveProperty : ReactiveProperty<Color>
    {
        public ColorReactiveProperty()
        {
        }

        public ColorReactiveProperty(Color initialValue)
            : base(initialValue)
        {
        }

        protected override IEqualityComparer<Color> EqualityComparer
        {
            get { return UnityEqualityComparer.Color; }
        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class RectReactiveProperty : ReactiveProperty<Rect>
    {
        public RectReactiveProperty()
        {
        }

        public RectReactiveProperty(Rect initialValue)
            : base(initialValue)
        {
        }

        protected override IEqualityComparer<Rect> EqualityComparer
        {
            get { return UnityEqualityComparer.Rect; }
        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class AnimationCurveReactiveProperty : ReactiveProperty<AnimationCurve>
    {
        public AnimationCurveReactiveProperty()
        {
        }

        public AnimationCurveReactiveProperty(AnimationCurve initialValue)
            : base(initialValue)
        {
        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class BoundsReactiveProperty : ReactiveProperty<Bounds>
    {
        public BoundsReactiveProperty()
        {
        }

        public BoundsReactiveProperty(Bounds initialValue)
            : base(initialValue)
        {
        }

        protected override IEqualityComparer<Bounds> EqualityComparer
        {
            get { return UnityEqualityComparer.Bounds; }
        }
    }

    /// <summary>Inspectable ReactiveProperty.</summary>
    [Serializable]
    public class QuaternionReactiveProperty : ReactiveProperty<Quaternion>
    {
        public QuaternionReactiveProperty()
        {
        }

        public QuaternionReactiveProperty(Quaternion initialValue)
            : base(initialValue)
        {
        }

        protected override IEqualityComparer<Quaternion> EqualityComparer
        {
            get { return UnityEqualityComparer.Quaternion; }
        }
    }
}