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
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(InspectorNoteAttribute))]
public class DrawerInspectorNote : DecoratorDrawer
{
    public override void OnGUI( Rect position )
    {
        InspectorNoteAttribute note = attribute as InspectorNoteAttribute;

        // our header is always present
        Rect posLabel = position;
        posLabel.y += 13;
        posLabel.x -= 2;
        posLabel.height += 13;
        EditorGUI.LabelField(posLabel, note.header, EditorStyles.whiteLargeLabel);

        // do we have a message too?
        if (!string.IsNullOrEmpty(note.message))
        {
            Color color = GUI.color;
            Color faded = color;
            faded.a = 0.6f;

            Rect posExplain = posLabel;
            posExplain.y += 15;
            GUI.color = faded;
            EditorGUI.LabelField(posExplain, note.message, EditorStyles.whiteMiniLabel);
            GUI.color = color;
        }

        Rect posLine = position;
        posLine.y += string.IsNullOrEmpty(note.message) ? 30 : 42;
        posLine.height = 1f;
        GUI.Box(posLine, "");
    }

    public override float GetHeight() {
        InspectorNoteAttribute note = attribute as InspectorNoteAttribute;
        return string.IsNullOrEmpty( note.message ) ? 38 : 50;
    }
}

[CustomPropertyDrawer( typeof( InspectorCommentAttribute ) )]
public class DrawerInspectorComment : DecoratorDrawer {
    public override void OnGUI( Rect position ) {
        InspectorCommentAttribute comment = attribute as InspectorCommentAttribute;

        // our header is always present
        Rect posLabel = position;
        //posLabel.y += 13;
        //posLabel.x -= 2;
        //posLabel.height += 13;
        //EditorGUI.LabelField( posLabel, comment.header, EditorStyles.whiteLargeLabel );

        // do we have a message too?
        if ( !string.IsNullOrEmpty( comment.message ) ) {
            Color color = GUI.color;
            Color faded = color;
            faded.a = 0.6f;

            Rect posExplain = posLabel;
            posExplain.y += 15;
            GUI.color = faded;
            EditorGUI.LabelField( posExplain, comment.message, EditorStyles.whiteMiniLabel );
            GUI.color = color;
        }

    }

    public override float GetHeight() {
        InspectorNoteAttribute note = attribute as InspectorNoteAttribute;
        return string.IsNullOrEmpty( note.message ) ? 38 : 50;
    }
}
