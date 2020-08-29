using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class LevelDrawer
    {
        [CustomPropertyDrawer(typeof(Int2dArray))]
        public class Int2dArrayEditor : PropertyDrawer {

			Vector2 intSize = new Vector2(30,15);
			Vector2 labelSize = new Vector2 (100, 15);
			Color cellColor = new Color (0.8f, 0.8f, 0.8f);

			public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
				return 45 + property.FindPropertyRelative ("y").intValue * 16;
			}

			public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
				EditorGUI.BeginProperty (position, label, property);

				EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);

				GUIStyle style = new GUIStyle(GUIStyle.none);
				style.alignment = TextAnchor.MiddleRight;
				style.padding = new RectOffset (0, 1, 0, 0);

				int x = property.FindPropertyRelative ("x").intValue;
				int y = property.FindPropertyRelative ("y").intValue;
				Rect rectL = new Rect (position.min + new Vector2(0, 20), labelSize);
				Rect rectX = new Rect (position.min + new Vector2(120, 20), intSize);
				Rect rectY = new Rect (position.min + new Vector2(150, 20), intSize);

				EditorGUI.LabelField (rectL, "Dimensions");
				EditorGUI.IntField (rectX, x, style);
				EditorGUI.IntField (rectY, y, style);

				SerializedProperty serArray = property.FindPropertyRelative ("m");
				float pad = position.xMax - (x + 1) * 31;
				if (pad < position.xMin) pad = position.xMin;

				Vector2 start = new Vector2 (pad, position.yMin + 40f);

				Vector2 cellSize = new Vector2 (31, 16);
				EditorGUI.DrawRect (new Rect (start, new Vector2 (cellSize.x * x + 1, cellSize.y * y + 1)), Color.black);

				start += Vector2.one;
				int n = 0;
				for (int i = 0; i < y; ++i) {
					for (int j = 0; j < x; ++j) {
						Vector2 offset = new Vector2 (cellSize.x * j, cellSize.y * i);
						Rect rectV = new Rect (start + offset, intSize);

						SerializedProperty intProp = serArray.GetArrayElementAtIndex (n);
						int value = intProp.intValue;
						if (intProp.intValue == 0)
							EditorGUI.DrawRect (rectV, cellColor);
						else
							EditorGUI.DrawRect (rectV, Color.red);
						intProp.intValue = EditorGUI.IntField (rectV, intProp.intValue, style);
						++n;
					}
				}

				EditorGUI.EndProperty ();
			}
		}
    }
}