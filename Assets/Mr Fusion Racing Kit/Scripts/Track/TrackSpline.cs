using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGSK
{
    public class TrackSpline : MonoBehaviour
    {
        public List<Transform> nodes = new List<Transform>();

        public bool loop = true;
        public bool smoothRoute = false;
        public bool visible = true;
        public bool showNodeIndexes;
        [Range(100,1000)]public float smoothness = 100;
        public Color color = Color.green;

        private int numPoints;
        private Vector3[] points;
        public float[] distances;
        public float length;

        private Transform[] Waypoints
        {
            get { return nodes.ToArray(); }
        }

        //this being here will save GC allocs
        private int p0n;
        private int p1n;
        private int p2n;
        private int p3n;

        private float i;
        private Vector3 P0;
        private Vector3 P1;
        private Vector3 P2;
        private Vector3 P3;


        public struct RoutePoint
        {
            public Vector3 position;
            public Vector3 direction;


            public RoutePoint(Vector3 position, Vector3 direction)
            {
                this.position = position;
                this.direction = direction;
            }
        }


        void Awake()
        {
            if (Waypoints.Length > 1)
            {
                CachePositionsAndDistances();
            }

            numPoints = Waypoints.Length;
        }


        public RoutePoint GetRoutePoint(float dist)
        {
            // position and direction
            Vector3 p1 = GetRoutePosition(dist);
            Vector3 p2 = GetRoutePosition(dist + 0.1f);
            Vector3 delta = p2 - p1;
            return new RoutePoint(p1, delta.normalized);
        }


        public Vector3 GetRoutePosition(float dist)
        {
            int point = 0;

            if (length == 0)
            {
                length = distances[distances.Length - 1];
            }

            dist = loop ? Mathf.Repeat(dist, length) : Mathf.PingPong(dist, length);

            while (distances[point] < dist)
            {
                point++;
            }

            // get nearest two points, ensuring points NOT wrap-around start & end of circuit
            p1n = loop ? ((point - 1) + numPoints) % numPoints : (point - 1) >= 0 ? point - 1 : 0;
            p2n = point;

            // found point numbers, now find interpolation value between the two middle points
            i = Mathf.InverseLerp(distances[p1n], distances[p2n], dist);

            if (smoothRoute)
            {
                // smooth catmull-rom calculation between the two relevant points
                // get indices for the surrounding 2 points, because
                // four points are required by the catmull-rom function
                p0n = loop ? ((point - 2) + numPoints) % numPoints : point - 2 >= 0 ? point - 2 : 0;
                p3n = loop ? (point + 1) % numPoints : point + 1 < points.Length ? point + 1 : point;

                if(loop)
                {
                    p2n = p2n % numPoints;
                }

                P0 = points[p0n];
                P1 = points[p1n];
                P2 = points[p2n];
                P3 = points[p3n];

                return CatmullRom(P0, P1, P2, P3, i);
            }
            else
            {
                // simple linear lerp between the two points:
                p1n = loop ? ((point - 1) + numPoints) % numPoints : point - 1 >= 0 ? point - 1 : point;
                p2n = point;

                return Vector3.Lerp(points[p1n], points[p2n], i);
            }
        }


        public float GetDistanceAtPosition(Vector3 position, int crntNearestNodeID, int prevNearestNodeID)
        {
            if (length == 0)
            {
                length = distances[distances.Length - 1];
            }

            int nearestNodeID = prevNearestNodeID;

            int i0;
            int i1;
            Vector3 p0;
            Vector3 v0;
            Vector3 v1;
            Vector3 n;
            float lerp = 0;
            float result = 0f;

            if(loop)
            {
                i0 = nearestNodeID;
                i1 = (nearestNodeID + 1) % numPoints;
                p0 = position - nodes[i0].position;
                v0 = nodes[i0].position;
                v1 = nodes[i1].position;
                n = v1 - v0;

                if (Vector3.Dot(p0, n) > 0)
                {
                    lerp = Vector3.Dot(n, position - v0) / n.sqrMagnitude;
                    result = Mathf.Lerp(distances[i0], distances[i1] + (i1 == 0 ? length : 0), lerp);

                    if (lerp > 0.5f) nearestNodeID = i1;
                }
                else
                {
                    i0 = (nearestNodeID - 1 + numPoints) % numPoints;
                    i1 = nearestNodeID;
                    v0 = nodes[i0].position;
                    v1 = nodes[i1].position;
                    n = v1 - v0;

                    lerp = Vector3.Dot(n, position - v0) / n.sqrMagnitude;
                    result = Mathf.Lerp(distances[i0], distances[i1] + (i1 == 0 ? length : 0), lerp);
                }

                crntNearestNodeID = nearestNodeID;
            }
            else
            {
                i0 = nearestNodeID == numPoints ? numPoints - 1 : nearestNodeID;
                i1 = nearestNodeID + 1;
                p0 = position - nodes[i0].position;
                v0 = nodes[i0].position;
                v1 = nodes[i1].position;
                n = v1 - v0;

                if (Vector3.Dot(p0, n) > 0)
                {
                    // nearest node is backward and consistent!
                    lerp = Vector3.Dot(n, position - v0) / n.sqrMagnitude;
                    result = Mathf.Lerp(distances[i0], distances[i1] + (i1 == 0 ? length : 0), lerp);

                    if (lerp > 0.5f) nearestNodeID = i1;
                }
                else
                {
                    // nearest node is forward!
                    i0 = (nearestNodeID - 1 + numPoints) % numPoints;
                    i1 = nearestNodeID;
                    v0 = nodes[i0].position;
                    v1 = nodes[i1].position;
                    n = v1 - v0;

                    lerp = Vector3.Dot(n, position - v0) / n.sqrMagnitude;
                    result = Mathf.Lerp(distances[i0], distances[i1] + (i1 == 0 ? length : 0), lerp);
                }

                crntNearestNodeID = nearestNodeID;
            }

            return result;
        }


        private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
        {
            // comments are no use here... it's the catmull-rom equation.
            // Un-magic this, lord vector!
            return 0.5f *
                   ((2 * p1) + (-p0 + p2) * i + (2 * p0 - 5 * p1 + 4 * p2 - p3) * i * i +
                    (-p0 + 3 * p1 - 3 * p2 + p3) * i * i * i);
        }


        void CachePositionsAndDistances()
        {
            float accumulateDistance = 0;

            if (loop)
            {
                points = new Vector3[Waypoints.Length + 1];
                distances = new float[Waypoints.Length + 1];

                for (int i = 0; i < points.Length; ++i)
                {
                    var t1 = Waypoints[(i) % Waypoints.Length];
                    var t2 = Waypoints[(i + 1) % Waypoints.Length];
                    if (t1 != null && t2 != null)
                    {
                        Vector3 p1 = t1.position;
                        Vector3 p2 = t2.position;
                        points[i] = Waypoints[i % Waypoints.Length].position;
                        distances[i] = accumulateDistance;
                        accumulateDistance += (p1 - p2).magnitude;
                    }
                }
            }
            else
            {
                points = new Vector3[Waypoints.Length];
                distances = new float[Waypoints.Length];

                for (int i = 0; i < points.Length; ++i)
                {
                    var t1 = Waypoints[i % Waypoints.Length];
                    var t2 = Waypoints[(i + 1) % Waypoints.Length];
                    if (t1 != null && t2 != null)
                    {
                        Vector3 p1 = t1.position;
                        Vector3 p2 = t2.position;
                        points[i] = Waypoints[i % Waypoints.Length].position;
                        distances[i] = accumulateDistance;
                        accumulateDistance += (p1 - p2).magnitude;
                    }
                }
            }
        }


        public void AdjustNodeRotation()
        {
            if (nodes.Count > 1)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (i < nodes.Count - 1)
                    {
                        //look at the next node
                        nodes[i].transform.LookAt(nodes[i + 1].transform);
                    }
                    else
                    {
                        //look in the direction the second last node
                        nodes[i].transform.LookAt(nodes[nodes.Count - 2].transform);
                        nodes[i].transform.Rotate(0, 180, 0);                       
                    }
                }
            }
        }


        public void GetChildNodes()
        {
            nodes.Clear();
            nodes = GetAllChildren();
        }


        public int GetNodeIndexAtDistance(float dist)
        {
            for (int i = 0; i < distances.Length - 1; i++)
            {
                if (dist < distances[i])
                    return i;
            }

            return 0;
        }


        public List<Transform> GetAllChildren()
        {
            List<Transform> temp = new List<Transform>();

            temp = transform.GetComponentsInChildren<Transform>().ToList();
            temp.RemoveAt(0);

            return temp;
        }


        public List<TrackNode> GetTrackNodes()
        {
            List<TrackNode> temp = new List<TrackNode>();

            temp = transform.GetComponentsInChildren<TrackNode>().ToList();

            return temp;
        }


        public List<RacingLineNode> GetRaceLineNodes()
        {
            List<RacingLineNode> temp = new List<RacingLineNode>();

            temp = transform.GetComponentsInChildren<RacingLineNode>().ToList();

            return temp;
        }


        void OnDrawGizmos()
        {
            if(visible)
            {
                DrawGizmos();
            }
        }


        public virtual void DrawGizmos()
        {
            if (Waypoints.Length > 1)
            {
                Gizmos.color = color;
                Vector3 prev = Waypoints[0].position;

                numPoints = Waypoints.Length;
                CachePositionsAndDistances();               
                length = distances[distances.Length - 1];

                for (int i = 0; i < transform.childCount; i++)
                {
                    Gizmos.DrawWireSphere(transform.GetChild(i).position, 0.25f);
                }

                if (smoothRoute)
                {
                    if (loop)
                    {
                        for (float dist = 0; dist < length; dist += length / smoothness)
                        {
                            Vector3 next = GetRoutePosition(dist + 1);
                            Gizmos.DrawLine(prev, next);
                            prev = next;
                        }
                        Gizmos.DrawLine(prev, Waypoints[0].position);
                    }
                    else
                    {
                        for (float dist = 0; dist < length - 1; dist += length / smoothness)
                        {
                            Vector3 next = GetRoutePosition(dist + 1);
                            Gizmos.DrawLine(prev, next);
                            prev = next;
                        }
                    }
                }
                else
                {
                    if (loop)
                    {
                        for (int n = 0; n < Waypoints.Length; ++n)
                        {
                            Vector3 next = Waypoints[(n + 1) % Waypoints.Length].position;
                            Gizmos.DrawLine(prev, next);
                            prev = next;
                        }
                    }
                    else
                    {
                        for (int n = 0; n < Waypoints.Length - 1; ++n)
                        {
                            Vector3 next = Waypoints[(n + 1)].position;
                            Gizmos.DrawLine(prev, next);
                            prev = next;
                        }
                    }
                }
            }
        }
    }
}