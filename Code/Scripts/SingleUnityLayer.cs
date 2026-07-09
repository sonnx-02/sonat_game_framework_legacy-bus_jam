using UnityEngine;
 
 [System.Serializable]
 public class SingleUnityLayer
 {
     [SerializeField]
     private int layerIndex = 0;
     public int LayerIndex => layerIndex;

     public void Set(int layer)
     {
         if (layer > 0 && layer < 32)
             layerIndex = layer;
     }
 
     public int Mask => 1 << layerIndex;
 }