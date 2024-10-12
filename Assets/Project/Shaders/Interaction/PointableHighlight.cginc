
float3 PointableHighlight_JointPositions[12];

//https://www.ronja-tutorials.com/post/035-2d-sdf-combination/#union
float PointableHighlight_Merge(float shape1, float shape2)
{
    return min(shape1, shape2);
}

//https://www.ronja-tutorials.com/post/035-2d-sdf-combination/#round
float PointableHighlight_RoundMerge(float shape1, float shape2, float radius)
{
    float2 intersectionSpace = float2(shape1 - radius, shape2 - radius);
    intersectionSpace = min(intersectionSpace, 0);
    float insideDistance = -length(intersectionSpace);
    float simpleUnion = PointableHighlight_Merge(shape1, shape2);
    float outsideDistance = max(simpleUnion, radius);
    return  insideDistance + outsideDistance;
}

float PointableHighlight_DistanceToJoint(float3 positionWS, float mergeRadius)
{
    float distanceToJoint = length(PointableHighlight_JointPositions[0] - positionWS);
    [loop]
    for (int i = 1; i < 2; i++)
    {
        distanceToJoint = PointableHighlight_RoundMerge(distanceToJoint, length(PointableHighlight_JointPositions[i] - positionWS), mergeRadius);
    }
    return distanceToJoint;
}