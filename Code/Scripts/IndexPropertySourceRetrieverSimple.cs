
using UniRx;
using UnityEngine;

public class IndexPropertySourceRetrieverSimple : BaseIndexPropertySource
{
    [SerializeField] private IndexPropertySourceRetrieverModelBase model;
    protected override IntReactiveProperty Property => model.Property;
}