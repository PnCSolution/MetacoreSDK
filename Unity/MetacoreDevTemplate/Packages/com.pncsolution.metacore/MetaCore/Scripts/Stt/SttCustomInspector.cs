#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using kr.co.pncsolution.Stt;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


[CustomEditor(typeof(SttComponent))]
public class SttTestCustomInspector: Editor{

    private string TAG = "SttTestCustomInspector ::";

    public SttComponent _component;


    private void OnEnable()
    {
        Debug.Log(TAG+" [method]OnEnable()...[START]");
        // target은 Editor에 있는 변수로 선택한 오브젝트를 받아옴.
        if (AssetDatabase.Contains(target))
        {
            _component = null;
        }
        else
        {
            // target은 Object형이므로 Enemy로 형변환
            _component = (SttComponent)target;
        }
        Debug.Log(TAG+" [method]OnEnable()...[END]");
    }

    public override void OnInspectorGUI(){
        // base.OnInspectorGUI();
        // Debug.Log(TAG+" [method]OnInspectorGUI()...[START]");

        GUI.enabled = false;
        EditorGUILayout.ObjectField("script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);    
        GUI.enabled = true;

        // GUIContent result_comment = new GUIContent("STT Result", "Command recognition result.");
        // GUILayout.Space(10);
        // EditorGUILayout.TextField(result_comment, _component.result_string);
        // GUI.enabled = true;

        // _component.result_string = (string)EditorGUILayout.TextField(result_title, _component.result_string);

        // _component.result_gameObject = (GameObject)EditorGUILayout.ObjectField("결과 객체", _component.result_gameObject, typeof(GameObject), true);

        // _component._command = (CommandString)EditorGUILayout.EnumPopup("명령어 종류", _component._command);

        // _component._target_gameObject = (TargetGameObject)EditorGUILayout.EnumPopup("Game Object", _component._target_gameObject);

        // switch (_component._target_gameObject){
        //     case TargetGameObject.GameObjectClass:
        //         GUILayout.Space(10);
        //         serializedObject.Update();
        //         EditorGUILayout.PropertyField(serializedObject.FindProperty("_target_gameObjectArray"), true);
        //         serializedObject.ApplyModifiedProperties();
        //         break;
        //     case TargetGameObject.ButtonClass: 
        //         GUILayout.Space(10);
        //         serializedObject.Update();
        //         EditorGUILayout.PropertyField(serializedObject.FindProperty("DataClassArray"), true);
        //         serializedObject.ApplyModifiedProperties();
        //         break;
        //     default:
        //         break;
        // }

        GUILayout.Space(10);
        serializedObject.Update();

        // EditorGUILayout.PropertyField(serializedObject.FindProperty("command_string_array"), new GUIContent("Command Strings", "사용할 음성 명령어를 입력하시오. 두 단어의 조합으로 입력하시오."), true);

        // GUILayout.Space(10);


        // EditorGUILayout.LabelField("Game Object List", EditorStyles.boldLabel);
        // EditorGUILayout.LabelField(
        //     new GUIContent("Game Objects List", "hello, world"), EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_gameobjects_list"), new GUIContent("Game Objects List", "hello, world"), true);

        GUILayout.Space(10);

        // EditorGUILayout.LabelField(
        //     new GUIContent("Buttons List", "hello, world"), EditorStyles.boldLabel);

        // EditorGUILayout.PropertyField(serializedObject.FindProperty("_buttons_list"), new GUIContent("Buttons List", "hello, world"), true);

        serializedObject.ApplyModifiedProperties();
        // serializedObject.Update();

        // GUILayout.Space(20);

        // serializedObject.Update();
        // EditorGUILayout.PropertyField(serializedObject.FindProperty("_testListGameObject"), true);
        // serializedObject.ApplyModifiedProperties();

        // GUILayout.Space(20);

        // serializedObject.Update();

        // SerializedProperty temp = serializedObject.FindProperty("_testListGameObject");
        // EditorGUILayout.PropertyField(temp, true);

        // for(int i = 0; i<temp.arraySize; i++){
        //     int result = temp.GetArrayElementAtIndex(i).FindPropertyRelative("_target").intValue;
        //     // Debug.Log("result = "+result);
        //     switch(result){
        //         case (int)TargetGameObject.GameObjectClass:
        //             GUILayout.Space(10);

        //             var obj = temp.GetArrayElementAtIndex(i).FindPropertyRelative("targetButton");
                                        
        //             break;
        //         case (int)TargetGameObject.ButtonClass:
        //             GUILayout.Space(10);

        //             break;
        //         default:
        //             break;
        //     }
        // }

        // serializedObject.ApplyModifiedProperties();

    }

}

#endif