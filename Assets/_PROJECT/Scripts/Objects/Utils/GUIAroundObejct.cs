using UnityEngine;

namespace ZFGinc.Objects.Utils
{
    public static class GUIAroundObejct
    {
        public static Vector2[] ExntentPoints;

        public static Rect GuiRect3DObject(Renderer render)
        {
            Vector3 center = render.bounds.center;
            Vector3 extents = render.bounds.extents;

            ExntentPoints = new Vector2[8]
            {
                WorldToGUIPoint(new Vector3(center.x-extents.x, center.y-extents.y, center.z-extents.z)),
                WorldToGUIPoint(new Vector3(center.x+extents.x, center.y-extents.y, center.z-extents.z)),
                WorldToGUIPoint(new Vector3(center.x-extents.x, center.y-extents.y, center.z+extents.z)),
                WorldToGUIPoint(new Vector3(center.x+extents.x, center.y-extents.y, center.z+extents.z)),
                WorldToGUIPoint(new Vector3(center.x-extents.x, center.y+extents.y, center.z-extents.z)),
                WorldToGUIPoint(new Vector3(center.x+extents.x, center.y+extents.y, center.z-extents.z)),
                WorldToGUIPoint(new Vector3(center.x-extents.x, center.y+extents.y, center.z+extents.z)),
                WorldToGUIPoint(new Vector3(center.x+extents.x, center.y+extents.y, center.z+extents.z))
            };

            Vector2 min = ExntentPoints[0];
            Vector2 max = ExntentPoints[0];

            foreach (Vector2 v in ExntentPoints)
            {
                min = Vector2.Min(min, v);
                max = Vector2.Max(max, v);
            }

            float width = max.x - min.x;
            float height = max.y - min.y;

            Rect result = new Rect(
                min.x,
                min.y,
                width,
                height
            );

            return result;
        }

        public static Rect GuiRect2DObject(GameObject gameObject)
        {
            Vector3[] vertices = gameObject.GetComponent<MeshFilter>().mesh.vertices;

            float x1 = float.MaxValue, y1 = float.MaxValue, x2 = 0.0f, y2 = 0.0f;

            foreach (Vector3 vert in vertices)
            {
                Vector2 tmp = WorldToGUIPoint(gameObject.transform.TransformPoint(vert));

                if (tmp.x < x1) x1 = tmp.x;
                if (tmp.x > x2) x2 = tmp.x;
                if (tmp.y < y1) y1 = tmp.y;
                if (tmp.y > y2) y2 = tmp.y;
            }

            Rect bbox = new Rect(x1, y1, x2 - x1, y2 - y1);
            return bbox;
        }

        public static Bounds GetMaxBounds(GameObject gameObject)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(gameObject.transform.position, Vector3.zero);
            var b = renderers[0].bounds;
            foreach (Renderer r in renderers)
            {
                b.Encapsulate(r.bounds);
            }
            return b;
        }

        public static Vector2 WorldToGUIPoint(Vector3 world)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(world);
            screenPoint.y = (float)Screen.height - screenPoint.y;

            if (screenPoint.y > Screen.height)
            {
                screenPoint.y = Screen.height;
            }

            if (screenPoint.y < 0)
            {
                screenPoint.y = 0;
            }

            return screenPoint;
        }
    }
}
