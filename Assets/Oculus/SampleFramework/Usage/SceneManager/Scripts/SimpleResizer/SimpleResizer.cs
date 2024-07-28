/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;

public class SimpleResizer
{
    public void CreateResizedObject(Vector3 newSize, GameObject parent, SimpleResizable sourcePrefab)
    {
        var prefab = MonoBehaviour.Instantiate(sourcePrefab.gameObject, Vector3.zero, Quaternion.identity);
        prefab.name = sourcePrefab.name;

        var resizable = prefab.GetComponent<SimpleResizable>();
        resizable.NewSize = newSize;
        if (resizable == null)
        {
            Debug.LogError("Resizable component missing.");
            return;
        }

        var resizedMesh = ProcessVertices(resizable, newSize);

        MeshFilter mf = prefab.GetComponent<MeshFilter>();
        mf.sharedMesh = resizedMesh;
        mf.sharedMesh.RecalculateBounds();

        // child it after creation so the bounds math plays nicely
        prefab.transform.parent = parent.transform;
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;

        // cleanup
        MonoBehaviour.Destroy(resizable);
    }

    #region PRIVATE METHODS

    private Mesh ProcessVertices(SimpleResizable resizable, Vector3 newSize)
    {
        Mesh originalMesh = resizable.Mesh;
        Vector3 originalBounds = originalMesh.bounds.size;

        // Force scaling if newSize is smaller than the original mesh
        SimpleResizable.Method methodX = (originalBounds.x < newSize.x)
            ? resizable.ScalingX
            : SimpleResizable.Method.Scale;
        SimpleResizable.Method methodY = (originalBounds.y < newSize.y)
            ? resizable.ScalingY
            : SimpleResizable.Method.Scale;
        SimpleResizable.Method methodZ = (originalBounds.z < newSize.z)
            ? resizable.ScalingZ
            : SimpleResizable.Method.Scale;

        Vector3[] resizedVertices = originalMesh.vertices;

        float pivotX = (1 / resizable.DefaultSize.x) * resizable.PivotPosition.x;
        float pivotY = (1 / resizable.DefaultSize.y) * resizable.PivotPosition.y;
        float pivotZ = (1 / resizable.DefaultSize.z) * resizable.PivotPosition.z;

        for (int i = 0; i < resizedVertices.Length; i++)
        {
            Vector3 vertexPosition = resizedVertices[i];
            vertexPosition.x = CalculateNewVertexPosition(
                methodX,
                vertexPosition.x,
                originalBounds.x,
                newSize.x,
                resizable.PaddingX,
                resizable.PaddingXMax,
                pivotX);

            vertexPosition.y = CalculateNewVertexPosition(
                methodY,
                vertexPosition.y,
                originalBounds.y,
                newSize.y,
                resizable.PaddingY,
                resizable.PaddingYMax,
                pivotY);

            vertexPosition.z = CalculateNewVertexPosition(
                methodZ,
                vertexPosition.z,
                originalBounds.z,
                newSize.z,
                resizable.PaddingZ,
                resizable.PaddingZMax,
                pivotZ);
            resizedVertices[i] = vertexPosition;
        }

        Mesh clonedMesh = MonoBehaviour.Instantiate(originalMesh);
        clonedMesh.vertices = resizedVertices;

        return clonedMesh;
    }

    private float CalculateNewVertexPosition(
        SimpleResizable.Method resizeMethod,
        float currentPosition,
        float currentSize,
        float newSize,
        float padding,
        float paddingMax,
        float pivot)
    {
        float resizedRatio = currentSize / 2
                             * (newSize / 2 * (1 / (currentSize / 2)))
                             - currentSize / 2;

        switch (resizeMethod)
        {
            case SimpleResizable.Method.Adapt:
                if (Mathf.Abs(currentPosition) >= padding)
                    currentPosition = resizedRatio * Mathf.Sign(currentPosition) + currentPosition;
                break;

            case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
                if (currentPosition >= padding)
                    currentPosition = resizedRatio * Mathf.Sign(currentPosition) + currentPosition;
                if (currentPosition <= paddingMax)
                    currentPosition = resizedRatio * Mathf.Sign(currentPosition) + currentPosition;
                break;

            case SimpleResizable.Method.Scale:
                currentPosition = newSize / (currentSize / currentPosition);
                break;

            case SimpleResizable.Method.None:
                break;
        }

        float pivotPos = newSize * (-pivot);
        currentPosition += pivotPos;

        return currentPosition;
    }

    #endregion
}
