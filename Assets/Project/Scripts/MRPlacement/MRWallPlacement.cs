// Copyright (c) Meta Platforms, Inc. and affiliates.

using Meta.XR.MRUtilityKit;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Places the MR walls in the scene
    /// </summary>
    public partial class MRWallPlacement : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("_transformList")]
        List<Transform> _wallContent;

        [SerializeField]
        List<Transform> _groundCornerContent;

        [SerializeField]
        List<Transform> _groundBorderContent;

        List<Vector3> _occupied = new List<Vector3>();

        [SerializeField]
        private bool _removeLeftoverWallContent;

        [SerializeField, FormerlySerializedAs("_removeLeftovers")]
        private bool _removeLeftoverGroundContent;

        [SerializeField]
        public MRUKAnchor.SceneLabels Labels = ~(MRUKAnchor.SceneLabels)0;

        List<Pose> _wallPosesBefore;
        List<Pose> _groundCornerPosesBefore;
        List<Pose> _groundBorderPosesBefore;

        private OVRCameraRig _rig;
        private Transform _floor;
        private IComparer<Pose> _sorter = new PoseSorter();

        List<Pose> _poses = new List<Pose>();
        List<Pose> _sortedPoses = new List<Pose>();

        private void Awake()
        {
            Time.timeScale = 0.001f; // we slow time to allow the model to load without progressing the scene too much

            _wallPosesBefore = _wallContent.ConvertAll(x => x.transform.GetPose());
            _groundCornerPosesBefore = _groundCornerContent.ConvertAll(x => x.transform.GetPose());
            _groundBorderPosesBefore = _groundBorderContent.ConvertAll(x => x.transform.GetPose());
        }

        private void Start()
        {
            if (MRUK.Instance)
            {
                MRUK.Instance.RegisterSceneLoadedCallback(() => PositionContent());
            }
        }

        void PositionContent()
        {
            MRUKRoom room = MRUK.Instance.GetCurrentRoom();
            for (int i = 0; i < _wallContent.Count; i++)
            {
                PoseOnWall(room, _wallContent[i].gameObject, 0.6f, null, _removeLeftoverWallContent, !_removeLeftoverWallContent);
            }
            var floor = new List<MRUKAnchor> { room.FloorAnchor };
            for (int i = 0; i < _groundCornerContent.Count; i++)
            {
                PoseOnWall(room, _groundCornerContent[i].gameObject, 0.6f, floor, _removeLeftoverGroundContent);
            }
            for (int i = 0; i < _groundBorderContent.Count; i++)
            {
                PoseOnWall(room, _groundBorderContent[i].gameObject, 0.6f, floor, _removeLeftoverGroundContent);
            }
            Time.timeScale = 1;
        }

        public void PoseOnWall(MRUKRoom room, GameObject SpawnObject, float minRadius, List<MRUKAnchor> snapTo = null, bool removeIfUnplaced = false, bool sort = false)
        {
            _poses.Clear();

            var goodY = 1.5f;

            for (int j = 0; j < 1000 && _poses.Count < 250; ++j)
            {
                MRUK.SurfaceType surfaceType = MRUK.SurfaceType.VERTICAL;
                if (room.GenerateRandomPositionOnSurface(surfaceType, minRadius, LabelFilter.Included(Labels), out var spawnPosition, out var spawnNormal))
                {
                    if (snapTo != null) snapTo[Random.Range(0, snapTo.Count)].GetClosestSurfacePosition(spawnPosition, out spawnPosition);
                    if (sort) spawnPosition.y = Mathf.MoveTowards(goodY, spawnPosition.y, 0.4f);

                    var center = snapTo == null ? spawnPosition + spawnNormal * 0.3f : spawnPosition + Vector3.up * 1.5f;
                    if (IsValidSpawnPoint(room, center, spawnPosition, spawnNormal))
                    {
                        var up = Mathf.Abs(spawnNormal.y) > 0.9f ? Random.onUnitSphere : Vector3.up;
                        Quaternion spawnRotation = Quaternion.LookRotation(spawnNormal, up);
                        Pose pose = new Pose(spawnPosition, spawnRotation);
                        if (sort)
                        {
                            _poses.Add(pose);
                        }
                        else
                        {
                            SpawnObject.transform.SetPose(pose);
                            _occupied.Add(pose.position);
                            return;
                        }
                    }
                }
            }

            if (sort && _poses.Count > 0)
            {
                GetOrderedPoses(_sortedPoses, _poses, x => true, _sorter);
                var pose = GetRandomPose(_sortedPoses, 3);
                SpawnObject.transform.SetPose(pose);
                _occupied.Add(pose.position);
            }
            else if (removeIfUnplaced)
            {
                SpawnObject.SetActive(false);
            }
        }

        bool IsValidSpawnPoint(MRUKRoom room, Vector3 center, Vector3 pos, Vector3 normal)
        {
            if (!_occupied.TrueForAll(x => (x - pos).sqrMagnitude > 1f)) return false;
            if (!room.IsPositionInRoom(center)) return false;
            // Ensure the center of the prefab will not spawn inside a scene volume
            if (room.IsPositionInSceneVolume(center)) return false;
            // Also make sure there is nothing close to the surface that would obstruct it
            if (room.Raycast(new Ray(pos, normal), 0.3f, out _)) return false;
            return true;
        }


        private static Pose GetRandomPose(List<Pose> poses, int maxCount)
        {
            return poses.Count > 0 ? poses[Random.Range(0, Math.Min(maxCount, poses.Count))] : default;
        }

        public void GetOrderedPoses(List<Pose> results, List<Pose> input, Predicate<Pose> predicate, IComparer<Pose> comparer)
        {
            results.Clear();
            for (int i = 0; i < input.Count; i++)
            {
                Pose candidatePose = input[i];
                if (!predicate(candidatePose)) continue;

                var index = results.BinarySearch(candidatePose, comparer);
                if (index < 0) index = ~index;
                results.Insert(index, candidatePose);
            }
        }

        class PoseSorter : IComparer<Pose>
        {
            public int Compare(Pose x, Pose y)
            {
                Transform camTransform = Camera.main.transform;
                var camPos = camTransform.position;
                var camFwd = camTransform.forward;

                Vector3 toPlayerX = (camPos - x.position).normalized;
                Vector3 toPlayerY = (camPos - y.position).normalized;
                Vector3 toPlayerXN = toPlayerX.normalized;
                Vector3 toPlayerYN = toPlayerY.normalized;

                var facingX = Vector3.Dot(x.forward, toPlayerXN);
                var facingY = Vector3.Dot(y.forward, toPlayerYN);

                if (Mathf.Abs(facingX - facingY) > 0.1f)
                {
                    return facingY.CompareTo(facingX);
                }

                var inFOVX = Vector3.Dot(-toPlayerXN, camFwd);
                var inFOVY = Vector3.Dot(-toPlayerYN, camFwd);

                if (Mathf.Abs(inFOVX - inFOVY) > 0.707f)
                {
                    return inFOVY.CompareTo(inFOVX);
                }

                return toPlayerX.sqrMagnitude.CompareTo(toPlayerY.sqrMagnitude);
            }
        }
    }
}
