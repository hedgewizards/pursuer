using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSegmentEnemyBuilder : MonoBehaviour
{
    public GameObject HeadPrefab;
    public GameObject SegmentPrefab;
    public GameObject TailPrefab;

    public float BaseSegmentOffset;
    public float BaseTailOffset;

    public float SegmentAnimationOffset;

    public float Scale;

    public float TargetLength;

    public void Build()
    {
        EnemyController enemyController = GetComponent<EnemyController>();
        GameObject head = MakeInScale(HeadPrefab);
        head.transform.localPosition = Vector3.zero;

        EnemySegmentController lastSegment = head.GetComponent<EnemySegmentController>();
        lastSegment.enemyController = enemyController;

        int count = (int)(TargetLength / (BaseSegmentOffset * Scale)) - 1;
        for (int n = 0; n < count + 1; n++)
        {
            GameObject segment = MakeInScale((n == count) ? TailPrefab : SegmentPrefab);
            segment.transform.localPosition = Vector3.back * ((n * BaseSegmentOffset) + ((n == count) ? BaseTailOffset : BaseSegmentOffset)) * Scale;
            EnemySegmentController controller = segment.GetComponent<EnemySegmentController>();
            controller.animationOffset = (SegmentAnimationOffset * n) % 1;
            controller.preferredDistance = ((n == count) ? BaseSegmentOffset : BaseTailOffset) * Scale;
            controller.enemyController = enemyController;
            lastSegment.nextSegment = controller;
            lastSegment = controller;
        }
    }

    public GameObject MakeInScale(GameObject prefab)
    {
        GameObject g = Instantiate<GameObject>(prefab);
        g.transform.parent = transform;
        g.transform.localScale = Vector3.one * Scale;
        return g;
    }
}
