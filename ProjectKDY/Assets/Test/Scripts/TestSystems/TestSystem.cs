using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoGameSystem {
    private List<TestStringComp> testStrings = new List<TestStringComp>();
    private List<TestFloatComp> testFloats = new List<TestFloatComp>();
    private List<MeshRenderer> testMeshRends = new List<MeshRenderer>();

    public List<TestStringComp> TestStrings {
        get {
            return testStrings;
        }

        set {
            testStrings = value;
        }
    }

    public List<TestFloatComp> TestFloats {
        get {
            return testFloats;
        }

        set {
            testFloats = value;
        }
    }

    public List<MeshRenderer> TestMeshRends {
        get {
            return testMeshRends;
        }

        set {
            testMeshRends = value;
        }
    }

    public override void Init() {
        print("Test System: Initialize!");
        print("Number of target entities: " + SystemEntities.Count);
        PrintAllSystemEntitityComponents();
    }

    public override IEnumerator Execute() {
        print("Test System: Execute");
        PrintAllEntityTestValues();
        yield break;
    }

    private void PrintAllSystemEntitityComponents() {
        for (int i = 0; i < SystemEntities.Count; i++) {
            PrintEntityComponents(SystemEntities[i]);
        }
    }

    private void PrintEntityComponents(MonoEntity ent) {
        for (int i = 0; i < ent.EntityComponents.Count; i++) {
            print(ent.EntityComponents[i]);
        }
    }

    private void PrintAllEntityTestValues() {
        print("Ent Count: " + SystemEntities.Count);
        for (int i = 0; i < SystemEntities.Count; i++) {
            PrintEntityTestValues(i);
        }
    }

    private void PrintEntityTestValues(int entIndex) {
        print("Entity Name - " + SystemEntities[entIndex].name + ", Entity ID - " + SystemEntities[entIndex].Id + ", String Component = " + testStrings[entIndex].TestString);
        print("Entity Name - " + SystemEntities[entIndex].name + ", Entity ID - " + SystemEntities[entIndex].Id + ", Float Component = " + testFloats[entIndex].TestFloat);
        print("Entity Name - " + SystemEntities[entIndex].name + ", Entity ID - " + SystemEntities[entIndex].Id + ", Mesh Render Component Color = " + testMeshRends[entIndex].material.color);
    }

    private void GenerateNewTestEntity() {
        List<System.Type> tmpTypes = new List<System.Type>();
        tmpTypes.AddRange(new System.Type[] { typeof(TestStringComp), typeof(TestFloatComp), typeof(MeshRenderer) });
        MonoEntity tmpEnt = MonoEngine.Instance.CreateEntity(tmpTypes);
        TestStringComp testString = tmpEnt.GetComponent<TestStringComp>();
        testString.TestString = "I am a new procedurally generated entity!";
        TestFloatComp testFloat = tmpEnt.GetComponent<TestFloatComp>();
        testFloat.TestFloat = Random.Range(0.0f, 100.0f);
        MeshRenderer testRend = tmpEnt.GetComponent<MeshRenderer>();
        testRend.material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }
}