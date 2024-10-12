// Copyright (c) Meta Platforms, Inc. and affiliates.

#if !UNITY_EDITOR && UNITY_ANDROID
#define QUEST
#endif

using System;
using UnityEngine;
#if QUEST
using UnityEngine.Android;
#endif
using static Meta.XR.MRUtilityKit.MRUK;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class MRUKLoader : MonoBehaviour
    {
        private bool _loadSceneCalled;

        public MRUKSettings SceneSettings => Instance.SceneSettings;

        void Start()
        {
#if QUEST
        // If we are going to load from device we need to ensure we have permissions first
        if ((SceneSettings.DataSource == SceneDataSource.Device || SceneSettings.DataSource == SceneDataSource.DeviceWithPrefabFallback) &&
            !Permission.HasUserAuthorizedPermission(OVRPermissionsRequester.ScenePermission))
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += permissionId =>
            {
                Debug.LogWarning("User denied permissions to use scene data");
            };
            callbacks.PermissionGranted += permissionId =>
            {
                // Permissions are now granted and it is safe to try load the scene now
                LoadScene();
            };
            // Note: If the permission request dialog is already active then this call will silently fail
            // and we won't receive the callbacks. So as a work-around there is a code in Update() to mitigate
            // this problem.
            Permission.RequestUserPermission(OVRPermissionsRequester.ScenePermission, callbacks);
        }
        else
#endif
            {
                LoadScene();
            }
        }

        void Update()
        {
            if (!_loadSceneCalled)
            {
#if QUEST
            // This is to cope with the case where the permissions dialog was already opened before we called
            // Permission.RequestUserPermission in Awake() and we don't get the PermissionGranted callback
            if (Permission.HasUserAuthorizedPermission(OVRPermissionsRequester.ScenePermission))
            {
                LoadScene();
            }
#endif
            }
        }

        private async void LoadScene()
        {
            _loadSceneCalled = true;
            var dataSource = SceneSettings.DataSource;
            try
            {
                if (dataSource == SceneDataSource.Device || dataSource == SceneDataSource.DeviceWithPrefabFallback)
                {
                    await Instance.LoadSceneFromDevice(false);
                }

                if (dataSource == SceneDataSource.Prefab || (dataSource == SceneDataSource.DeviceWithPrefabFallback && Instance.Rooms.Count == 0))
                {
                    if (SceneSettings.RoomPrefabs.Length == 0)
                    {
                        Debug.LogWarning($"Failed to load room from prefab because prefabs list is empty");
                        return;
                    }

                    // Clone the roomPrefab, but essentially replace all its content
                    // if -1 or out of range, use a random one
                    var roomIndex = UnityEngine.Random.Range(0, SceneSettings.RoomPrefabs.Length);
                    Debug.Log($"Loading prefab room {roomIndex}");

                    var roomPrefab = SceneSettings.RoomPrefabs[roomIndex];
                    Instance.LoadSceneFromPrefab(roomPrefab);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw;
            }
        }
    }
}
