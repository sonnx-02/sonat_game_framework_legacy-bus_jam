using System;
using System.Collections.Generic;
using System.Reflection;
using Sonat;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;




public static class FrameworkExtensions
{
	public static Vector2 WorldToPositionInCanvas(Vector3 worldPosition)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			var canvasSize2 = Object.FindObjectOfType<ScreenRoot>().GetComponent<RectTransform>().sizeDelta;
			Vector2 viewportPosition2 = Camera.main.WorldToViewportPoint(worldPosition);
			return  new Vector2(
				((viewportPosition2.x * canvasSize2.x) - (canvasSize2.x * 0.5f)),
				((viewportPosition2.y * canvasSize2.y) - (canvasSize2.y * 0.5f)));
		}
#endif
		
		var canvasSize = RootView.rootView.screenRoot.CanvasSize;
		Vector2 viewportPosition = RootView.rootView.gameController.cam.WorldToViewportPoint(worldPosition);
		return  new Vector2(
			((viewportPosition.x * canvasSize.x) - (canvasSize.x * 0.5f)),
			((viewportPosition.y * canvasSize.y) - (canvasSize.y * 0.5f)));
	}
}