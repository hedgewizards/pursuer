using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Combat.Inspector
{
    [CustomEditor(typeof(HealthTank))]
    class HealthTankEditor : Editor
    {
        HealthTank self;

        public override void OnInspectorGUI()
        {
            self = target as HealthTank;
            DrawDefaultInspector();

            if ((self.HitBoxes == null || self.HitBoxes.Length == 0) && GUILayout.Button("Set Up"))
            {
                Undo.RecordObject(self, "Set Up");
                PrefabUtility.RecordPrefabInstancePropertyModifications(self);

                self.HitBoxes = self.GetComponentsInChildren<HitBox>();
            }

            var mirrorButton = new GUIContent("Mirror HitBoxes Left -> Right", "Mirrors all hitboxes whose gameobject name ends in .l to the opposite bone (modifying if it exists)");
            if (self.HitBoxes != null && self.HitBoxes.Length > 0 && GUILayout.Button(mirrorButton))
            {
                Undo.SetCurrentGroupName("Mirror Hitboxes");
                int group = Undo.GetCurrentGroup();

                Undo.RecordObject(self, "Set Up");
                PrefabUtility.RecordPrefabInstancePropertyModifications(self);

                var oldHitBoxes = self.HitBoxes;
                List<HitBox> newHitBoxes = new List<HitBox>();

                // match strings that look like bone.name.l or bone.name.r
                Regex mirrorRegex = new Regex(@"^([\w\.]*\.)([lr])$");
                
                foreach(HitBox hitBox in oldHitBoxes)
                {
                    if (hitBox == null) continue;

                    Match hitBoxMirrorMatch = mirrorRegex.Match(hitBox.name);

                    if (hitBoxMirrorMatch.Success)
                    {
                        bool isLeft = hitBoxMirrorMatch.Groups[2].Value == "l";
                        string root = hitBoxMirrorMatch.Groups[1].Value;

                        if (!isLeft)
                        {
                            Undo.DestroyObjectImmediate(hitBox.gameObject);
                        }
                        else
                        {
                            Match boneMirrorMatch = mirrorRegex.Match(hitBox.transform.parent.name);
                            if (boneMirrorMatch.Success)
                            {
                                Transform mirroredBone = findBoneByName(boneMirrorMatch.Groups[1].Value + "r");

                                if (mirroredBone == null) throw new KeyNotFoundException($"Missing Bone {boneMirrorMatch.Groups[1].Value}r");

                                GameObject hitboxPrefab = PrefabUtility.GetCorrespondingObjectFromSource(hitBox.gameObject);

                                GameObject newHitBox;
                                if (hitboxPrefab != null)
                                {
                                    newHitBox = (GameObject)PrefabUtility.InstantiatePrefab(hitboxPrefab, mirroredBone);
                                    newHitBox.name = hitBoxMirrorMatch.Groups[1].Value + "r";

                                    // mirror across X axis
                                    newHitBox.transform.localPosition = new Vector3
                                    {
                                        x = hitBox.transform.localPosition.x * -1,
                                        y = hitBox.transform.localPosition.y,
                                        z = hitBox.transform.localPosition.z,
                                    };
                                    newHitBox.transform.localRotation = hitBox.transform.localRotation;
                                    newHitBox.transform.localScale = hitBox.transform.localScale;

                                    Collider collider = hitBox.GetComponent<Collider>();
                                    if (collider is BoxCollider)
                                    {
                                        BoxCollider boxCollider = collider as BoxCollider;
                                        BoxCollider newBoxCollider = newHitBox.GetComponent<BoxCollider>();

                                        // mirror across X axis
                                        newBoxCollider.center = new Vector3
                                        {
                                            x = boxCollider.center.x * -1,
                                            y = boxCollider.center.y,
                                            z = boxCollider.center.z,
                                        };
                                        newBoxCollider.size = boxCollider.size;
                                    }
                                    else
                                    {
                                        throw new System.NotImplementedException();
                                    }
                                }
                                else
                                {
                                    newHitBox = Instantiate(hitBox.gameObject, mirroredBone, false);
                                }

                                newHitBoxes.Add(hitBox);
                                newHitBoxes.Add(newHitBox.GetComponent<HitBox>());
                                Undo.RegisterCreatedObjectUndo(newHitBox, "mirror hitbox");
                            }
                            else
                            {
                                Debug.LogWarning($"no matching right bone for {hitBox.transform.parent.name}");
                            }
                        }
                    }
                    else
                    {
                        newHitBoxes.Add(hitBox);
                    }
                }

                self.HitBoxes = newHitBoxes.ToArray();
                Undo.CollapseUndoOperations(group);
            }
        }

        public Transform findBoneByName(string name)
        {
            // starting at our root transform, perform a breadth-first search for the correct bone
            Queue<Transform> transforms = new Queue<Transform>();
            transforms.Enqueue(self.transform);
            
            // while we have more transforms to check,
            while (transforms.Count > 0)
            {
                Transform checkTransform = transforms.Dequeue();

                // go through each child of this transform
                for (int n = 0; n < checkTransform.childCount; n++)
                {
                    Transform child = checkTransform.GetChild(n);

                    // if the name matches, return it
                    if (child.name == name) return child;

                    // otherwise add it to our queue
                    transforms.Enqueue(child);
                }
            }

            // we didn't find it. exit
            return null;
        }
    }
}