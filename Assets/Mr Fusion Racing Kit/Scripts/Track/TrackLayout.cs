using UnityEngine;
using System.Collections;
using RGSK;

public class TrackLayout : TrackSpline
{
    public float defaultTrackWidth = 10;
    public bool showSegments = true;
    private TrackNode[] trackNodes;
    private RacingLineNode[] racingLineNodes;

    //Target projection
    public float minTargetDistance = 10;
    public float maxTargetDistance = 50;

    //Automatic node speed values
    public float minSpeed = 50;
    public float maxSpeed = 100;
    public float cautionAngle = 50;


    void Start()
    {
        trackNodes = GetTrackNodes().ToArray();
        foreach (TrackNode n in trackNodes)
        {
            n.distanceAtNode = GetDistanceAtNode(n.transform);
        }

        racingLineNodes = GetRaceLineNodes().ToArray();
    }


    public void CalculateNodeSpeeds()
    {
        AdjustNodeRotation();

        racingLineNodes = GetRaceLineNodes().ToArray();

        for (int i = 0; i < racingLineNodes.Length; i++)
        {
            if (i > 0)
            {
                //Compare the angle between the previous to the next node 
                Vector3 direction = racingLineNodes[i].transform.position
                                    - racingLineNodes[i - 1].transform.position;

                float angle = Vector3.Angle(direction, racingLineNodes[i].transform.forward);
                float ratio = Mathf.InverseLerp(0, cautionAngle, angle);
                float nodeSpeed = Mathf.Lerp(maxSpeed, minSpeed, ratio);
                racingLineNodes[i - 1].targetSpeed = nodeSpeed;
            }
        }

        racingLineNodes[racingLineNodes.Length - 1].targetSpeed = racingLineNodes[racingLineNodes.Length - 2].targetSpeed;
    }


    public float GetLeftWidth(int index)
    {
        return trackNodes[index].leftWidth;
    }


    public float GetRightWidth(int index)
    {
        return trackNodes[index].rightWidth;
    }


    public float GetDistanceAtNode(Transform node)
    {
        int index = nodes.IndexOf(node);
        return distances[index];
    }


    public float GetSpeedAtNode(int index)
    {
        return racingLineNodes[index].targetSpeed;
    }


    public override void DrawGizmos()
    {
        base.DrawGizmos();
        DrawTrackLimits();
    }


    void DrawTrackLimits()
    {
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            if (!transform.GetChild(i).GetComponent<TrackNode>()) return;

            Gizmos.DrawLine(transform.GetChild(i).position + (transform.GetChild(i).right * transform.GetChild(i).GetComponent<TrackNode>().rightWidth),
                           transform.GetChild(i + 1).position + (transform.GetChild(i + 1).right * transform.GetChild(i + 1).GetComponent<TrackNode>().rightWidth));

            Gizmos.DrawLine(transform.GetChild(i).position + (-transform.GetChild(i).right * transform.GetChild(i).GetComponent<TrackNode>().leftWidth),
               transform.GetChild(i + 1).position + (-transform.GetChild(i + 1).right * transform.GetChild(i + 1).GetComponent<TrackNode>().leftWidth));

            if (loop)
            {
                Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position + (transform.GetChild(transform.childCount - 1).right * transform.GetChild(transform.childCount - 1).GetComponent<TrackNode>().rightWidth),
                transform.GetChild(0).position + (transform.GetChild(0).right * transform.GetChild(0).GetComponent<TrackNode>().rightWidth));

                Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position + (-transform.GetChild(transform.childCount - 1).right * transform.GetChild(transform.childCount - 1).GetComponent<TrackNode>().leftWidth),
               transform.GetChild(0).position + (-transform.GetChild(0).right * transform.GetChild(0).GetComponent<TrackNode>().leftWidth));
            }
        }
    }
}
